@echo off
setlocal

REM ================================================
REM nU3.Bootstrapper 단일 실행 파일 배치 파일 (Debug)
REM ================================================

REM 설정
set PROJECT_DIR=%~dp0
set PROJECT_FILE=nU3.Bootstrapper.csproj
set OUTPUT_DIR=%PROJECT_DIR%publish_debug
set CONFIGURATION=Debug

echo ================================================
echo nU3.Bootstrapper 단일 실행 파일 빌드 (Debug)
echo ================================================
echo.

REM 프로젝트 디렉토리로 이동
cd /d "%PROJECT_DIR%"

if not exist "%PROJECT_FILE%" (
    [1m[31m오류[0m: %PROJECT_FILE% 파일을 찾을 수 없습니다.
    echo 현재 디렉토리: %CD%
    pause
    exit /b 1
)

echo [1m[32m1단계[0m: 출력 디렉토리 정리...
if exist "%OUTPUT_DIR%" (
    rd /s /q "%OUTPUT_DIR%"
)

echo [1m[32m2단계[0m: NuGet 패키지 복원...
dotnet restore "%PROJECT_FILE%" --no-cache
if %errorlevel% neq 0 (
    [1m[31m오류[0m: NuGet 패키지 복원 실패
    pause
    exit /b 1
)

echo.
echo [1m[32m3단계[0m: 단일 실행 파일로 게시 중...
echo.
echo   - 설정: %CONFIGURATION%
echo   - 런타임: win-x64
echo   - 단일 파일: 활성화
echo   - 자체 포함: 활성화
echo   - 출력 경로: %OUTPUT_DIR%
echo.

dotnet publish "%PROJECT_FILE%" ^
    --configuration %CONFIGURATION% ^
    --runtime win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:PublishReadyToRun=false ^
    --output "%OUTPUT_DIR%"

if %errorlevel% neq 0 (
    [1m[31m오류[0m: 게시 실패
    pause
    exit /b 1
)

echo.
echo ================================================
echo [1m[32m빌드 완료![0m
echo ================================================
echo.
echo 출력 경로: %OUTPUT_DIR%
echo.
echo 실행 파일 목록:
dir /b "%OUTPUT_DIR%\*.exe"
echo.

REM 최종 실행 파일 표시
for %%f in ("%OUTPUT_DIR%\*.exe") do (
    echo.
    echo [1m[36m실행 파일: %%f[0m
    for %%s in ("%%f") do echo   크기: %%~zs bytes
)

echo.
echo ================================================
echo 끝내려면 아무 키나 누르세요...
pause > nul
