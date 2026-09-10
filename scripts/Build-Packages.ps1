param(
    [Parameter(Mandatory=$true)][ValidatePattern('^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$')][string]$Version
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
Push-Location $root
try {
    $revision = git rev-parse HEAD
    if ($LASTEXITCODE -ne 0) { throw 'Cannot read source commit' }
    $output = Join-Path $root "artifacts/$Version"
    if (Test-Path $output) { throw "Output already exists: $output. Use a clean checkout." }
    dotnet run --project Tests/ProtocolChecks.csproj -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Protocol checks failed' }
    dotnet pack src/YuJanggiCommon/YuJanggiCommon.csproj -c Release "-p:Version=$Version" "-p:RepositoryCommit=$revision" -p:ContinuousIntegrationBuild=true -o "$output/nuget"
    if ($LASTEXITCODE -ne 0) { throw 'Pack failed' }
    $package = "$output/upm"
    New-Item -ItemType Directory "$package/Runtime" -Force | Out-Null
    Copy-Item src/YuJanggiCommon/bin/Release/netstandard2.1/YuJanggiCommon.dll "$package/Runtime/"
    Copy-Item packaging/YuJanggiCommon.dll.meta "$package/Runtime/"
    Copy-Item packaging/README.md "$package/README.md"
    @{
        name='com.seokjinyoo.yujanggi.protocol'; version=$Version
        displayName='YuJanggi Protocol'; unity='6000.3'
        description='Shared contracts. Requires host-provided System.Text.Json runtime; see README.'
    } | ConvertTo-Json | Set-Content "$package/package.json" -Encoding utf8
    @{
        repository='YuJanggi.Protocol'; version=$Version; commit=$revision
        framework='netstandard2.1'
        sha256=(Get-FileHash "$package/Runtime/YuJanggiCommon.dll" -Algorithm SHA256).Hash
    } | ConvertTo-Json | Set-Content "$package/protocol-version.json" -Encoding utf8
    $archive = "$output/com.seokjinyoo.yujanggi.protocol-$Version.tgz"
    # Unity tarballs require a package/ prefix.
    $staging = "$output/tar/package"
    New-Item -ItemType Directory $staging -Force | Out-Null
    Copy-Item "$package/*" $staging -Recurse
    tar -czf $archive -C "$output/tar" package
    if ($LASTEXITCODE -ne 0) { throw 'UPM archive failed' }
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $nupkg = Get-Item "$output/nuget/YuJanggi.Protocol.$Version.nupkg"
    $zip = [IO.Compression.ZipFile]::OpenRead($nupkg.FullName)
    try {
        foreach ($tfm in @('net10.0','netstandard2.1')) {
            if (-not $zip.GetEntry("lib/$tfm/YuJanggiCommon.dll")) { throw "Missing $tfm DLL" }
        }
    } finally { $zip.Dispose() }
    Write-Host "Packages verified: $output"
} finally { Pop-Location }
