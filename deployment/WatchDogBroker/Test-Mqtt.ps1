#requires -Version 7.4
param(
    [string]$BrokerHost='140.116.234.108',
    [int]$Port=18688,
    [string]$CaFile="$PSScriptRoot/ca.crt",
    [string]$UserName='watchdog_pub'
)
$ErrorActionPreference='Stop'
$tcp=$null; $tls=$null
try {
    $ca=[Security.Cryptography.X509Certificates.X509Certificate2]::CreateFromPem([IO.File]::ReadAllText($CaFile))
    $secret=Read-Host "Password for $UserName" -AsSecureString
    $tcp=[Net.Sockets.TcpClient]::new()
    $task=$tcp.ConnectAsync($BrokerHost,$Port)
    if (-not $task.Wait(10000)) { throw 'TCP connection timed out.' }
    $task.GetAwaiter().GetResult()
    Write-Host 'TCP connected.'
    $tls=[Net.Security.SslStream]::new($tcp.GetStream(),$false)
    $policy=[Security.Cryptography.X509Certificates.X509ChainPolicy]::new()
    $policy.TrustMode=[Security.Cryptography.X509Certificates.X509ChainTrustMode]::CustomRootTrust
    [void]$policy.CustomTrustStore.Add($ca)
    $policy.RevocationMode=[Security.Cryptography.X509Certificates.X509RevocationMode]::NoCheck
    $options=[Net.Security.SslClientAuthenticationOptions]::new()
    $options.TargetHost=$BrokerHost
    $options.CertificateChainPolicy=$policy
    $options.EnabledSslProtocols=[Security.Authentication.SslProtocols]::Tls12 -bor [Security.Authentication.SslProtocols]::Tls13
    $cancel=[Threading.CancellationTokenSource]::new(10000)
    try { $tls.AuthenticateAsClientAsync($options,$cancel.Token).GetAwaiter().GetResult() } finally { $cancel.Dispose() }
    Write-Host "TLS verified (CA trust + endpoint identity): $($tls.SslProtocol)"
    $tls.ReadTimeout=10000; $tls.WriteTimeout=10000
    function Encode-String([string]$Value) {
        $bytes=[Text.Encoding]::UTF8.GetBytes($Value)
        if ($bytes.Length -gt 65535) { throw 'MQTT string too long.' }
        [byte[]](@([byte]($bytes.Length -shr 8),[byte]($bytes.Length -band 255))+$bytes)
    }
    $plain=[Net.NetworkCredential]::new('',$secret).Password
    [byte[]]$body=@(0,4,77,81,84,84,4,194,0,30)+
        (Encode-String ('watchdog-check-'+[Guid]::NewGuid().ToString('N')))+
        (Encode-String $UserName)+(Encode-String $plain)
    $plain=$null; $secret.Dispose()
    $remaining=$body.Length; $length=[Collections.Generic.List[byte]]::new()
    do {
        $digit=$remaining % 128; $remaining=[int][Math]::Floor($remaining/128)
        if ($remaining -gt 0) { $digit=$digit -bor 128 }
        $length.Add([byte]$digit)
    } while ($remaining -gt 0)
    [byte[]]$packet=@(16)+$length.ToArray()+$body
    $tls.Write($packet,0,$packet.Length)
    [Array]::Clear($packet); [Array]::Clear($body)
    $reply=[byte[]]::new(4); $offset=0
    while ($offset -lt 4) {
        $count=$tls.Read($reply,$offset,4-$offset)
        if ($count -eq 0) { throw 'Broker closed connection before CONNACK.' }
        $offset+=$count
    }
    if ($reply[0] -ne 32 -or $reply[1] -ne 2) { throw 'Unexpected MQTT response.' }
    if ($reply[3] -ne 0) { throw "MQTT login rejected, CONNACK code $($reply[3])." }
    $tls.Write([byte[]]@(224,0),0,2)
    Write-Host 'PASS: TCP, TLS certificate verification and MQTT login succeeded. No message was published.' -ForegroundColor Green
} catch {
    Write-Host "FAIL: $_" -ForegroundColor Red
    exit 1
} finally {
    if ($tls) { $tls.Dispose() }
    if ($tcp) { $tcp.Dispose() }
    if ($ca) { $ca.Dispose() }
}
