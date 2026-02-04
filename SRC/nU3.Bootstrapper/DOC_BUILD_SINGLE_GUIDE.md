# 단일 실행 파일 빌드 가이드

## 개요
nU3.Bootstrapper 프로젝트를 단일 실행 파일(실행 가능한 EXE)로 빌드하기 위한 가이드입니다.

## 배치 파일

### 1. Release 빌드 (프로덕션)
```batch
build_single.bat
```

- 설정: Release
- 최적화: 활성화
- ReadyToRun: 활성화
- 출력: `publish/` 폴더

### 2. Debug 빌드 (개발)
```batch
build_single_debug.bat
```

- 설정: Debug
- 최적화: 비활성화
- ReadyToRun: 비활성화
- 출력: `publish_debug/` 폴더

## 빌드 옵션

| 옵션 | 설명 |
|------|------|
| `--runtime win-x64` | Windows x64 런타임용 빌드 |
| `--self-contained true` | 런타임 포함 (별도 설치 불필요) |
| `-p:PublishSingleFile=true` | 모든 종속성을 단일 EXE로 패키징 |
| `-p:IncludeNativeLibrariesForSelfExtract=true` | 네이티브 라이브러리 포함 |
| `-p:EnableCompressionInSingleFile=true` | 압축 활성화 (파일 크기 감소) |
| `-p:PublishReadyToRun=true` | JIT 미리 컴파일 (빠른 시작) |

## 출력 구조

```
publish/
├── nU3.Bootstrapper.exe    # 단일 실행 파일 (약 70-100MB)
└── (기타 파일 없음)
```

## 사용 방법

1. 배치 파일을 더블 클릭하여 실행
2. 빌드 완료 후 출력 폴더에서 `nU3.Bootstrapper.exe` 실행
3. LOG 폴더가 자동 생성되고 일일 로그 파일이 기록됨

## 주의사항

### appsettings.json 처리
단일 파일 배포 시 `appsettings.json`은 실행 파일 내부에 포함됩니다:

- **Release/Debug 빌드**: 리소스에 포함 → 자동 로드
- **수정 필요 시**: 실행 파일과 같은 폴더에 `appsettings.json` 배치 → 외부 파일 우선 적용

### 파일 크기
단일 실행 파일 크기는 약 70-100MB입니다 (런타임 포함):

- **Compression 활성화**: 약 30-40% 감소
- **ReadyToRun 활성화**: 추가 10-15MB (빠른 시작)
- **Debug 빌드**: 더 큰 사이즈 (디버그 정보 포함)

### 실행 시간
첫 실행 시:
- **Release**: 압축 해제 + ReadyToRun → 약 5-10초
- **Debug**: 압축 해제만 → 약 3-5초

## 수동 빌드 명령어

### Release
```bash
cd nU3.Bootstrapper
dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:PublishReadyToRun=true --output publish
```

### Debug
```bash
cd nU3.Bootstrapper
dotnet publish --configuration Debug --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:PublishReadyToRun=false --output publish_debug
```

## 문제 해결

### "appsettings.json을 찾을 수 없습니다" 오류
- 단일 파일 배포 시 정상 (리소스에서 로드됨)
- 외부 `appsettings.json` 배치 시 우선 적용

### 빌드 실패
- `dotnet restore` 수행 후 다시 빌드
- Visual Studio에서 솔루션 열고 빌드

### 실행 시 로그 확인
- `LOG/` 폴더에서 일일 로그 파일 확인
- 파일명: `yyyyMMdd.log`
