param([Parameter(Mandatory=$true)][string]$ServerRoot, [Parameter(Mandatory=$true)][string]$UnityRoot)
$ErrorActionPreference = 'Stop'
$source = Join-Path $PSScriptRoot 'src/YuJanggiCommon/YuJanggiCommon.csproj'
$revision = git -C $PSScriptRoot rev-parse HEAD
if ($LASTEXITCODE -ne 0) { throw '소스 커밋 확인 실패' }
if (git -C $PSScriptRoot status --porcelain) { throw '소스 변경을 먼저 커밋하세요.' }
foreach ($root in @($ServerRoot,$UnityRoot)) {
    if (-not (Test-Path -LiteralPath $root)) { throw "저장소가 없습니다: $root" }
}
dotnet build $source -c Release
if ($LASTEXITCODE -ne 0) { throw '공용 DLL 빌드 실패' }
$targets = @(
    @{Root=$ServerRoot; Path='lib/YuJanggiCommon'; Framework='net10.0'},
    @{Root=$UnityRoot; Path='Assets/Plugins/YuJanggiCommon'; Framework='netstandard2.1'}
)
foreach ($target in $targets) {
    $destination = Join-Path $target.Root $target.Path
    New-Item -ItemType Directory -Path $destination -Force | Out-Null
    $binary = Join-Path $PSScriptRoot ("src/YuJanggiCommon/bin/Release/" + $target.Framework + "/YuJanggiCommon.dll")
    Copy-Item -LiteralPath $binary -Destination (Join-Path $destination 'YuJanggiCommon.dll') -Force
    @{repository='YuJanggi.Protocol'; commit=$revision; framework=$target.Framework; sha256=(Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash} |
        ConvertTo-Json | Set-Content -LiteralPath (Join-Path $destination 'protocol-version.json') -Encoding utf8
}
