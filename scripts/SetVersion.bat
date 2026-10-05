@echo off
setlocal EnableExtensions

rem =========================
rem Arguments
rem =========================
set "TARGET=%~1"
set "VERSION=%~2"

if "%TARGET%"=="" (
    echo [ERROR] Target is required.
    echo Usage: SetVersion.bat ^<Target^> ^<Version^>
    echo Example: SetVersion.bat Protocol 1.3.2
    exit /b 1
)

if "%VERSION%"=="" (
    echo [ERROR] Version is required.
    echo Usage: SetVersion.bat ^<Target^> ^<Version^>
    echo Example: SetVersion.bat Protocol 1.3.2
    exit /b 1
)

rem =========================
rem Paths
rem =========================
set "SCRIPT_DIR=%~dp0"

for %%I in ("%SCRIPT_DIR%..") do (
    set "ROOT=%%~fI"
)

set "CSPROJ=%ROOT%\src\YuJanggi.%TARGET%.csproj"
set "VERSION_FILE=%ROOT%\src\Version.cs"
set "UPM_PACKAGE=%ROOT%\upm\package.json"

rem =========================
rem Validate Version Format
rem =========================
powershell -NoProfile -Command ^
"if ($env:VERSION -notmatch '^\d+\.\d+\.\d+([\-+][0-9A-Za-z.-]+)?$') { exit 1 }"

if errorlevel 1 (
    echo [ERROR] Invalid version format: %VERSION%
    echo Example: 1.3.2
    exit /b 1
)

echo.
echo =========================
echo Set Version
echo =========================
echo Target: %TARGET%
echo Version: %VERSION%

rem =========================
rem Validate Required Files
rem =========================
for %%F in ("%CSPROJ%" "%VERSION_FILE%" "%UPM_PACKAGE%") do (
    if not exist "%%~F" (
        echo [ERROR] Required file not found:
        echo %%~F
        exit /b 1
    )
)

rem =========================
rem Update Version.cs
rem =========================
echo.
echo Updating Version.cs...

powershell -NoProfile -Command ^
"$path = $env:VERSION_FILE; ^
$version = $env:VERSION; ^
$text = [System.IO.File]::ReadAllText($path); ^
$pattern = 'public\s+const\s+string\s+Current\s*=\s*""[^""]+""\s*;'; ^
if ($text -notmatch $pattern) { throw 'Version.Current not found.' }; ^
$replacement = 'public const string Current = ""' + $version + '"";'; ^
$updated = [regex]::Replace($text, $pattern, $replacement, 1); ^
[System.IO.File]::WriteAllText($path, $updated)"

if errorlevel 1 (
    echo [ERROR] Failed to update Version.cs.
    exit /b 1
)

rem =========================
rem Update Version.cs
rem =========================
echo.
echo Updating Version.cs...

powershell -NoProfile -Command "$path=$env:VERSION_FILE; $version=$env:VERSION; $q=[char]34; $found=$false; $lines=Get-Content -LiteralPath $path; $lines=$lines | ForEach-Object { if($_ -match '^\s*public\s+const\s+string\s+Current\s*='){ $found=$true; '        public const string Current = ' + $q + $version + $q + ';' } else { $_ } }; if(-not $found){ throw 'Version.Current not found.' }; Set-Content -LiteralPath $path -Value $lines -Encoding UTF8"

if errorlevel 1 (
    echo [ERROR] Failed to update Version.cs.
    exit /b 1
)

rem =========================
rem Update csproj
rem =========================
echo Updating YuJanggi.%TARGET%.csproj...

powershell -NoProfile -Command "$path=$env:CSPROJ; $version=$env:VERSION; $text=[System.IO.File]::ReadAllText($path); if($text -notmatch '<Version>[^<]+</Version>'){ throw 'Version element not found.' }; $updated=[regex]::Replace($text, '<Version>[^<]+</Version>', '<Version>' + $version + '</Version>', 1); [System.IO.File]::WriteAllText($path,$updated)"

if errorlevel 1 (
    echo [ERROR] Failed to update csproj.
    exit /b 1
)

rem =========================
rem Update package.json
rem =========================
echo Updating package.json...

powershell -NoProfile -Command "$path=$env:UPM_PACKAGE; $version=$env:VERSION; $json=Get-Content -LiteralPath $path -Raw | ConvertFrom-Json; $json.version=$version; $json | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $path -Encoding UTF8"

if errorlevel 1 (
    echo [ERROR] Failed to update package.json.
    exit /b 1
)

rem =========================
rem Completed
rem =========================
echo.
echo =========================
echo Version update completed
echo =========================
echo Target: %TARGET%
echo Version: %VERSION%
echo.
echo Updated:
echo - %VERSION_FILE%
echo - %CSPROJ%
echo - %UPM_PACKAGE%

exit /b 0