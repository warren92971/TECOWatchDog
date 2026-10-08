function Get-VerifiedDownload($Entry, [string]$Destination) {
    if (-not (Test-Path -LiteralPath $Destination)) {
        Invoke-WebRequest -Uri $Entry.url -OutFile $Destination
    }
    if ((Get-FileHash -LiteralPath $Destination -Algorithm SHA256).Hash -ne $Entry.sha256) {
        throw "SHA256 mismatch: $Destination. Delete this download and retry."
    }
}

function Get-RabbitPasswordHash([string]$Password) {
    $salt = [Security.Cryptography.RandomNumberGenerator]::GetBytes(4)
    $digest = [Security.Cryptography.SHA256]::HashData([byte[]]($salt + [Text.Encoding]::UTF8.GetBytes($Password)))
    [Convert]::ToBase64String([byte[]]($salt + $digest))
}

function New-BrokerCertificates([string]$Directory, [string[]]$Names) {
    $now = [DateTimeOffset]::UtcNow.AddMinutes(-10)
    $caKey = [Security.Cryptography.RSA]::Create(3072)
    $serverKey = [Security.Cryptography.RSA]::Create(3072)
    try {
        $caReq = [Security.Cryptography.X509Certificates.CertificateRequest]::new('CN=WatchDog Private CA', $caKey, 'SHA256', [Security.Cryptography.RSASignaturePadding]::Pkcs1)
        $caReq.CertificateExtensions.Add([Security.Cryptography.X509Certificates.X509BasicConstraintsExtension]::new($true,$false,0,$true))
        $caReq.CertificateExtensions.Add([Security.Cryptography.X509Certificates.X509KeyUsageExtension]::new([Security.Cryptography.X509Certificates.X509KeyUsageFlags]::KeyCertSign -bor [Security.Cryptography.X509Certificates.X509KeyUsageFlags]::CrlSign,$true))
        $ca = $caReq.CreateSelfSigned($now,$now.AddYears(5))
        $req = [Security.Cryptography.X509Certificates.CertificateRequest]::new('CN=WatchDog MQTT Broker', $serverKey, 'SHA256', [Security.Cryptography.RSASignaturePadding]::Pkcs1)
        $san = [Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder]::new()
        foreach ($name in $Names | Select-Object -Unique) {
            $ip = $null
            if ([Net.IPAddress]::TryParse($name,[ref]$ip)) { $san.AddIpAddress($ip) } else { $san.AddDnsName($name) }
        }
        $req.CertificateExtensions.Add($san.Build())
        $req.CertificateExtensions.Add([Security.Cryptography.X509Certificates.X509BasicConstraintsExtension]::new($false,$false,0,$true))
        $req.CertificateExtensions.Add([Security.Cryptography.X509Certificates.X509KeyUsageExtension]::new([Security.Cryptography.X509Certificates.X509KeyUsageFlags]::DigitalSignature -bor [Security.Cryptography.X509Certificates.X509KeyUsageFlags]::KeyEncipherment,$true))
        $eku = [Security.Cryptography.OidCollection]::new()
        [void]$eku.Add([Security.Cryptography.Oid]::new('1.3.6.1.5.5.7.3.1'))
        $req.CertificateExtensions.Add([Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension]::new($eku,$false))
        $serial = [Security.Cryptography.RandomNumberGenerator]::GetBytes(16)
        $serial[0] = $serial[0] -band 0x7f
        $server = $req.Create($ca,$now,$now.AddDays(365),$serial)
        [IO.File]::WriteAllText((Join-Path $Directory 'ca.crt'),$ca.ExportCertificatePem())
        [IO.File]::WriteAllText((Join-Path $Directory 'ca.key'),$caKey.ExportPkcs8PrivateKeyPem())
        [IO.File]::WriteAllText((Join-Path $Directory 'server.crt'),$server.ExportCertificatePem())
        [IO.File]::WriteAllText((Join-Path $Directory 'server.key'),$serverKey.ExportPkcs8PrivateKeyPem())
        $server.Dispose(); $ca.Dispose()
    } finally { $caKey.Dispose(); $serverKey.Dispose() }
}
