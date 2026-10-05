@echo off
setlocal EnableExtensions

rem =========================
rem Arguments
rem =========================
set "TARGET=%~1"

if "%TARGET%"=="" (
    echo [ERROR] Target is required.
    echo Usage: BuildPackage.bat ^<Target^>
    echo Example: BuildPackage.bat Protocol
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
set "PREPARE_UPM=%SCRIPT_DIR%Prepare-Upm.ps1"
set "NUGET_OUTPUT=%ROOT%\artifacts\nuget"

rem =========================
rem Validate Required Files
rem =========================
if not exist "%CSPROJ%" (
    echo [ERROR] Project file not found:
    echo %CSPROJ%
    exit /b 1
)

if not exist "%PREPARE_UPM%" (
    echo [ERROR] Prepare-Upm.ps1 not found:
    echo %PREPARE_UPM%
    exit /b 1
)

rem =========================
rem Prepare Output Directory
rem =========================
if not exist "%NUGET_OUTPUT%" (
    mkdir "%NUGET_OUTPUT%"

    if errorlevel 1 (
        echo [ERROR] Failed to create NuGet output directory.
        exit /b 1
    )
)

echo.
echo =========================
echo Build Package
echo =========================
echo Target: %TARGET%
echo.

rem =========================
rem Build NuGet Package
rem =========================
echo Building NuGet package...

dotnet pack "%CSPROJ%" ^
    --configuration Release ^
    --output "%NUGET_OUTPUT%"

if errorlevel 1 (
    echo [ERROR] NuGet package build failed.
    exit /b 1
)

rem =========================
rem Prepare UPM Package
rem =========================
echo.
echo Preparing UPM package...

powershell -NoProfile -ExecutionPolicy Bypass ^
    -File "%PREPARE_UPM%"

if errorlevel 1 (
    echo [ERROR] UPM package preparation failed.
    exit /b 1
)

rem =========================
rem Completed
rem =========================
echo.
echo =========================
echo Package build completed
echo =========================
echo Target: %TARGET%
echo.
echo NuGet:
echo - %NUGET_OUTPUT%
echo.
echo UPM:
echo - %ROOT%\upm

exit /b 0