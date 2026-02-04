@echo off
setlocal enabledelayedexpansion

REM Publish all projects to a single directory under the solution root.
REM Usage:
REM   publish-all.bat [Debug|Release] [outputDir]
REM Examples:
REM   publish-all.bat Debug
REM   publish-all.bat Release Publish

set CONFIG=%~1
if "%CONFIG%"=="" set CONFIG=Release

set OUTROOT=%~2
if "%OUTROOT%"=="" set OUTROOT=Publish

set SOLUTIONDIR=%~dp0
if "%SOLUTIONDIR%"=="" set SOLUTIONDIR=.

set OUTDIR=%SOLUTIONDIR%%OUTROOT%\%CONFIG%

echo Publishing to: %OUTDIR%
if not exist "%OUTDIR%" mkdir "%OUTDIR%"

set PROJECTS=
set PROJECTS=!PROJECTS! "nU3.Shell\nU3.Shell.csproj"
set PROJECTS=!PROJECTS! "nU3.Tools.Deployer\nU3.Tools.Deployer.csproj"
set PROJECTS=!PROJECTS! "nU3.Bootstrapper\nU3.Bootstrapper.csproj"
REM Module projects (DLL) that should be deployed alongside the apps
set PROJECTS=!PROJECTS! "Modules\ADM\nU3.Modules.ADM.AD.Deployer\nU3.Modules.ADM.AD.Deployer.csproj"

for %%P in (!PROJECTS!) do (
  echo.
  echo === Publishing %%~P ===
  dotnet publish "%%~P" -c %CONFIG% -o "%OUTDIR%" --no-self-contained
  if errorlevel 1 (
    echo Publish failed for %%~P
    exit /b 1
  )
)

echo.
echo Publish completed: %OUTDIR%
exit /b 0
