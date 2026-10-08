$ErrorActionPreference = 'Stop'
try {
    $principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'Right-click Start-Install.cmd and choose Run as administrator.'
    }
    if (-not [Environment]::Is64BitOperatingSystem) { throw 'Windows x64 is required.' }
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    $manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'downloads.json') -Raw | ConvertFrom-Json
    $cache = Join-Path $PSScriptRoot 'cache'
    New-Item -ItemType Directory -Path $cache -Force | Out-Null
    $zip = Join-Path $cache 'PowerShell.zip'
    if (-not (Test-Path -LiteralPath $zip)) {
        Write-Host 'Downloading Microsoft PowerShell runtime...'
        Invoke-WebRequest $manifest.PowerShell.url -OutFile $zip -UseBasicParsing
    }
    if ((Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash -ne $manifest.PowerShell.sha256) {
        throw 'PowerShell SHA256 mismatch. Remove cache/PowerShell.zip and retry.'
    }
    $runtime = Join-Path $cache 'powershell'
    Expand-Archive -LiteralPath $zip -DestinationPath $runtime -Force
    & (Join-Path $runtime 'pwsh.exe') -NoProfile -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot 'Install-Broker.ps1')
    if ($LASTEXITCODE -ne 0) { throw 'Installation failed. Read the error above and README.md.' }
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
