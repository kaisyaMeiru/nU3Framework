@echo off
setlocal enabledelayedexpansion

:: 1. Find MSBuild
set "MSBUILD_PATH="
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
    set "MSBUILD_PATH=%%i"
)

if "!MSBUILD_PATH!"=="" (
    echo [ERROR] MSBuild.exe not found!
    pause
    exit /b 1
)

echo [INFO] Found MSBuild: "!MSBUILD_PATH!"

:: 2. Build Project
echo [INFO] Building nU3.ModuleCreator.Vsix...
"!MSBUILD_PATH!" "nU3.ModuleCreator.Vsix.csproj" /t:Rebuild /p:Configuration=Release  /v:m

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build Failed!
    pause
    exit /b 1
)

echo.
echo [SUCCESS] Build Completed!
echo VSIX file is at: bin\Release\nU3.ModuleCreator.Vsix.vsix
pause
