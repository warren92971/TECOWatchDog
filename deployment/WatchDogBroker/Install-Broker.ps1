#requires -Version 7.4
#requires -RunAsAdministrator
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot/Common.ps1"
$root = 'C:\WatchDogBroker'
try {
    # Dedicated fresh machine only; never replace another installation.
    if (Test-Path -LiteralPath $root) { throw "$root already exists. Stop and inspect the previous installation before retrying." }
    if (Get-Service '*Rabbit*' -ErrorAction SilentlyContinue) { throw 'Existing RabbitMQ service detected. No changes made.' }
    $existing = Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*' -ErrorAction SilentlyContinue |
        Where-Object { $_.DisplayName -match 'RabbitMQ|Erlang' }
    if ($existing -or $env:ERLANG_HOME -or $env:RABBITMQ_BASE -or $env:RABBITMQ_CONFIG_FILE) { throw 'Existing Erlang/RabbitMQ installation or environment found. No changes made.' }
    if (Get-NetTCPConnection -LocalPort 8883 -State Listen -ErrorAction SilentlyContinue) { throw 'Port 8883 is occupied. Stop the temporary TcpListener first.' }
    if ([Environment]::OSVersion.Version.Build -lt 17763) { throw 'Windows 10 1809 / Server 2019 or newer x64 required.' }
    Write-Host 'Dedicated WatchDog RabbitMQ installer. TLS 8883; external endpoint 140.116.234.108:18688.'
    $external = Read-Host 'Certificate external IP/DNS [140.116.234.108]'
    if (-not $external) { $external='140.116.234.108' }
    if ([Uri]::CheckHostName($external) -eq [UriHostNameType]::Unknown) { throw 'Enter an IP or DNS name only, without port or URL.' }
    $scope = Read-Host 'Firewall allowed source IP/CIDR [Any; school firewall still applies]'
    if (-not $scope) { $scope='Any' }
    $users = @()
    foreach ($entry in @(@('watchdog_admin','administrator'),@('watchdog_pub',''),@('teco_sub',''))) {
        $p1 = Read-Host "Set password for $($entry[0]) (at least 12 characters)" -AsSecureString
        $p2 = Read-Host 'Repeat password' -AsSecureString
        $plain = [Net.NetworkCredential]::new('', $p1).Password
        if ($plain.Length -lt 12 -or $plain -cne [Net.NetworkCredential]::new('', $p2).Password) { throw 'Passwords must match and contain at least 12 characters.' }
        $users += @{name=$entry[0]; password_hash=(Get-RabbitPasswordHash $plain); hashing_algorithm='rabbit_password_hashing_sha256'; tags=$entry[1]}
        $plain=$null; $p1.Dispose(); $p2.Dispose()
    }
    $manifest = Get-Content "$PSScriptRoot/downloads.json" -Raw | ConvertFrom-Json
    $cache = Join-Path $PSScriptRoot 'cache'
    New-Item -ItemType Directory -Path $cache -Force | Out-Null
    foreach ($name in 'Erlang','RabbitMQ') {
        Write-Host "Downloading and verifying $name..."
        Get-VerifiedDownload $manifest.$name (Join-Path $cache "$name.exe")
    }
    New-Item -ItemType Directory -Path $root | Out-Null
    # Restrict private keys, definition password hashes and runtime to SYSTEM/admins.
    & icacls.exe $root /inheritance:r /grant:r '*S-1-5-18:(OI)(CI)F' '*S-1-5-32-544:(OI)(CI)F' | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Cannot restrict installation directory permissions.' }
    foreach ($dir in 'certs','client','data','logs','runtime') { New-Item -ItemType Directory "$root/$dir" | Out-Null }
    $addresses = @(Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.IPAddress -notlike '169.254.*' } | Select-Object -ExpandProperty IPAddress)
    New-BrokerCertificates "$root/certs" (@($external,'localhost','127.0.0.1',$env:COMPUTERNAME)+$addresses)
    Copy-Item "$root/certs/ca.crt" "$root/client/ca.crt"
    $permissions = @(
        @{user='watchdog_admin';vhost='watchdog';configure='.*';write='.*';read='.*'},
        @{user='watchdog_pub';vhost='watchdog';configure='^$';write='^amq\.topic$';read='^$'},
        @{user='teco_sub';vhost='watchdog';configure='^mqtt-subscription-.*$';write='^mqtt-subscription-.*$';read='^(amq\.topic|mqtt-subscription-.*)$'}
    )
    $definitions = @{users=$users;vhosts=@(@{name='watchdog'});permissions=$permissions;topic_permissions=@(
        @{user='watchdog_pub';vhost='watchdog';exchange='amq.topic';write='^teco\.watchdog\..+$';read='^$'},
        @{user='teco_sub';vhost='watchdog';exchange='amq.topic';write='^$';read='^teco\.watchdog\..+$'}
    )}
    $definitions | ConvertTo-Json -Depth 12 | Set-Content "$root/definitions.json" -Encoding utf8NoBOM
    @'
listeners.tcp = none
management.tcp.ip = 127.0.0.1
management.tcp.port = 15672
distribution.listener.interface = 127.0.0.1
definitions.import_backend = local_filesystem
definitions.local.path = C:/WatchDogBroker/definitions.json
mqtt.listeners.tcp = none
mqtt.listeners.ssl.default = 8883
mqtt.allow_anonymous = false
mqtt.vhost = watchdog
mqtt.exchange = amq.topic
ssl_options.cacertfile = C:/WatchDogBroker/certs/ca.crt
ssl_options.certfile = C:/WatchDogBroker/certs/server.crt
ssl_options.keyfile = C:/WatchDogBroker/certs/server.key
ssl_options.verify = verify_none
ssl_options.fail_if_no_peer_cert = false
ssl_options.versions.1 = tlsv1.2
ssl_options.versions.2 = tlsv1.3
log.file = C:/WatchDogBroker/logs/rabbit.log
'@ | Set-Content "$root/rabbitmq.conf" -Encoding utf8NoBOM
    '[rabbitmq_management,rabbitmq_mqtt].' | Set-Content "$root/enabled_plugins" -Encoding ascii
    $variables = @{
        ERLANG_HOME="$root\erlang"; RABBITMQ_BASE="$root\data";
        RABBITMQ_CONFIG_FILE="$root\rabbitmq"; RABBITMQ_ENABLED_PLUGINS_FILE="$root\enabled_plugins";
        ERL_EPMD_ADDRESS='127.0.0.1'
    }
    foreach ($key in $variables.Keys) {
        [Environment]::SetEnvironmentVariable($key,$variables[$key],'Machine')
        [Environment]::SetEnvironmentVariable($key,$variables[$key],'Process')
    }
    Write-Host 'Installing Erlang...'
    $proc=Start-Process "$cache/Erlang.exe" -ArgumentList @('/S',"/D=$root\erlang") -Wait -PassThru -WindowStyle Hidden
    if ($proc.ExitCode -ne 0 -or -not (Test-Path "$root/erlang/bin/erl.exe")) { throw "Erlang installation failed: $($proc.ExitCode)" }
    Write-Host 'Installing RabbitMQ...'
    $proc=Start-Process "$cache/RabbitMQ.exe" -ArgumentList @('/S',"/D=$root\rabbitmq-server") -Wait -PassThru -WindowStyle Hidden
    if ($proc.ExitCode -ne 0) { throw "RabbitMQ installation failed: $($proc.ExitCode)" }
    Set-Service RabbitMQ -StartupType Automatic
    Start-Service RabbitMQ
    Write-Host 'Waiting up to 120 seconds for MQTTS...'
    $ready=$false
    for ($i=0;$i -lt 60;$i++) {
        if (Get-NetTCPConnection -LocalPort 8883 -State Listen -ErrorAction SilentlyContinue) { $ready=$true; break }
        Start-Sleep -Seconds 2
    }
    if (-not $ready) { throw "Broker did not start. Inspect $root\logs\rabbit.log and Windows services." }
    New-NetFirewallRule -Name 'WatchDog-MQTTS-8883' -DisplayName 'WatchDog MQTTS TCP 8883' -Direction Inbound -Action Allow -Protocol TCP -LocalPort 8883 -RemoteAddress $scope -Profile Any | Out-Null
    Copy-Item "$PSScriptRoot/Test-Mqtt.ps1" "$root/client/Test-Mqtt.ps1"
    # Portable runtime for diagnostics; no change to system PowerShell.
    Copy-Item "$PSHOME/*" "$root/runtime" -Recurse -Force
    @"
External MQTTS: ${external}:18688 (school NAT to this host:8883)
Local MQTTS: localhost:8883
Topic: teco/watchdog/status (all allowed topics start with teco/watchdog/)
Publisher: watchdog_pub; subscriber: teco_sub
CA public certificate: ca.crt
Management on SERVER only: http://127.0.0.1:15672 user watchdog_admin
TLS server certificate expires in 365 days. Renew before expiry; retain original CA.
ClientId must be unique. Use QoS 1. Credentials are the passwords entered at setup.
"@ | Set-Content "$root/client/Connection.txt"
    Write-Host 'SUCCESS: Broker installed and TCP 8883 is listening.' -ForegroundColor Green
    Write-Host "Next: run $root\runtime\pwsh.exe -File $root\client\Test-Mqtt.ps1 -BrokerHost localhost -Port 8883"
    Write-Host 'Only share client/ca.crt and connection details. Never share certs/*.key or definitions.json.'
} catch {
    Write-Host "INSTALLATION STOPPED: $_" -ForegroundColor Red
    Write-Host 'Partial installation is kept for diagnosis. Do not delete folders or overwrite an existing installation.'
    exit 1
}
