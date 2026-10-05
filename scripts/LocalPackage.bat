@echo off
setlocal EnableExtensions

rem =========================
rem Input
rem =========================
set "TARGET="
set "VERSION="

set /p TARGET=Target: 
set /p VERSION=Version: 

if "%TARGET%"=="" (
    echo [ERROR] Target is required.
    exit /b 1
)

if "%VERSION%"=="" (
    echo [ERROR] Version is required.
    exit /b 1
)

rem =========================
rem Paths
rem =========================
set "SCRIPT_DIR=%~dp0"

set "SET_VERSION=%SCRIPT_DIR%SetVersion.bat"
set "BUILD_PACKAGE=%SCRIPT_DIR%BuildPackage.bat"

rem =========================
rem Validate Scripts
rem =========================
if not exist "%SET_VERSION%" (
    echo [ERROR] SetVersion.bat not found:
    echo %SET_VERSION%
    exit /b 1
)

if not exist "%BUILD_PACKAGE%" (
    echo [ERROR] BuildPackage.bat not found:
    echo %BUILD_PACKAGE%
    exit /b 1
)

rem =========================
rem Set Version
rem =========================
echo.
echo =========================
echo Set Version
echo =========================
echo Target : %TARGET%
echo Version: %VERSION%

call "%SET_VERSION%" "%TARGET%" "%VERSION%"

if errorlevel 1 (
    echo.
    echo [ERROR] SetVersion failed.
    pause
    exit /b 1
)

rem =========================
rem Build Package
rem =========================
echo.
echo =========================
echo Build Package
echo =========================

call "%BUILD_PACKAGE%" "%TARGET%"

if errorlevel 1 (
    echo.
    echo [ERROR] BuildPackage failed.
    pause
    exit /b 1
)

rem =========================
rem Completed
rem =========================
echo.
echo =========================
echo Package completed
echo =========================
echo Target : %TARGET%
echo Version: %VERSION%

pause
exit /b 0