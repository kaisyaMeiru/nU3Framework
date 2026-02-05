# nU3 Bootstrapper - 업데이트 패치 모듈

## 개요

`nU3.Bootstrapper`는 Framework 컴포넌트와 화면 모듈을 자동으로 업데이트하고 MainShell을 실행하는 런처입니다.

## 주요 기능

### 1. Framework 컴포넌트 업데이트 (신규)

```
┌─────────────────────────────────────────────────────────────┐
│                    Bootstrapper 시작                         │
│                          │                                   │
│                    DB 초기화                                 │
│                          │                                   │
│              Framework 컴포넌트 확인                         │
│         (nU3.Core.dll, DevExpress.*.dll 등)                 │
│                          │                                   │
│           ┌──────────────┴──────────────┐                   │
│           │                              │                   │
│     업데이트 있음                   최신 버전                │
│           │                              │                   │
│     UI 표시 + 다운로드                   │                   │
│           │                              │                   │
│           └──────────────┬──────────────┘                   │
│                          │                                   │
│              화면 모듈 업데이트 (기존)                       │
│                          │                                   │
│                  MainShell 실행                              │
└─────────────────────────────────────────────────────────────┘
```

### 2. 업데이트 UI

```
┌─────────────────────────────────────────────────────────────┐
│ ?? Framework 컴포넌트 업데이트                              │
├─────────────────────────────────────────────────────────────┤
│ 다운로드 중... (3/10)                                       │
│ [??????????????????????????????] 30%                        │
│ DevExpress.XtraEditors                                      │
├─────────────────────────────────────────────────────────────┤
│ 상태 │ 컴포넌트            │ 버전     │ 크기   │ 유형      │
│──────┼────────────────────┼──────────┼────────┼───────────│
│ ?   │ nU3.Core           │ 1.0.0.0  │ 245 KB │ Core      │
│ ?   │ nU3.Core.UI        │ 1.0.0.0  │ 189 KB │ Core      │
│ ??   │ DevExpress.XtraEdi │ 23.2.9   │ 2.1 MB │ Library   │
│ ?   │ Oracle.Managed...  │ 21.12.0  │ 8.5 MB │ Library   │
├─────────────────────────────────────────────────────────────┤
│                        [ 취소 ]                              │
└─────────────────────────────────────────────────────────────┘
```

## 파일 구조

```
nU3.Bootstrapper/
├── Program.cs              # 메인 진입점
├── ComponentLoader.cs      # Framework 컴포넌트 로더 (신규)
├── ModuleLoader.cs         # 화면 모듈 로더 (기존)
├── UpdateProgressForm.cs   # 업데이트 UI (신규)
├── Seeder.cs               # 개발용 테스트 데이터
└── nU3.Bootstrapper.csproj
```

## 클래스 설명

### ComponentLoader

Framework 컴포넌트 (DLL, EXE) 업데이트 담당:

```csharp
var loader = new ComponentLoader(dbManager, installPath);

// 1. 업데이트 확인
var updates = loader.CheckForUpdates();

// 2. 필수 컴포넌트 누락 확인
var missing = loader.GetMissingRequiredComponents();

// 3. 전체 업데이트
var result = loader.UpdateAll();

// 4. 진행 이벤트
loader.UpdateProgress += (s, e) => 
{
    Console.WriteLine($"{e.Phase}: {e.ComponentName} ({e.PercentComplete}%)");
};
```

### UpdateProgressForm

업데이트 진행 UI:

```csharp
using var form = new UpdateProgressForm();

// 업데이트 목록 초기화
form.InitializeUpdateList(updates);

// 진행 상태 업데이트
loader.UpdateProgress += (s, e) => form.UpdateProgress(e);

// 결과 표시
form.ShowResult(result);

form.ShowDialog();
```

### ModuleLoader (기존)

화면 모듈 (DLL with screens) 업데이트:

```csharp
var moduleLoader = new ModuleLoader();
moduleLoader.EnsureDatabaseInitialized();
moduleLoader.CheckAndLoadModules(shellPath);
```

## 실행 흐름

### 1. 정상 시나리오

```
1. nU3.Bootstrapper.exe 실행
   ↓
2. DB 초기화 (SYS_COMPONENT_MST, SYS_MODULE_MST 등)
   ↓
3. Shell 경로 찾기 (nU3.MainShell.exe)
   ↓
4. Framework 컴포넌트 업데이트 확인
   - 서버 버전 vs 로컬 버전 비교
   - 해시 비교로 변경 감지
   ↓
5. 업데이트 있으면 UI 표시 + 다운로드
   - 서버 저장소 → 캐시 → 설치 경로
   ↓
6. 화면 모듈 업데이트
   ↓
7. MainShell 실행
```

### 2. 업데이트 취소 시

- 필수 컴포넌트 누락 여부 확인
- 누락 시: 경고 메시지 표시 후 종료
- 누락 없음: Shell 실행 계속

### 3. 오류 발생 시

- 파일 잠금: 3회 재시도 후 실패 처리
- 네트워크 오류: 실패 목록에 추가, 계속 진행
- 필수 컴포넌트 실패: 경고 후 종료

## 경로 설정

### 서버 저장소 (배포 원본)

```
%AppData%\nU3.Framework\ServerStorage\Components\
├── Framework\
│   ├── nU3.Core.dll
│   └── nU3.Core.UI.dll
├── DevExpress\
│   └── DevExpress.XtraEditors.dll
└── Oracle\
    └── Oracle.ManagedDataAccess.dll
```

### 캐시 (다운로드 영역)

```
%AppData%\nU3.Framework\Cache\Components\
├── Framework\
├── DevExpress\
└── Oracle\
```

### 설치 경로 (런타임)

```
C:\Program Files\nU3\              ← Shell 위치 기준
├── nU3.MainShell.exe
├── nU3.Core.dll                   ← InstallPath: ""
├── nU3.Core.UI.dll
├── plugins\                       ← InstallPath: "plugins"
│   └── MyPlugin.dll
└── Modules\                       ← 화면 모듈 (ModuleLoader)
    └── EMR\IN\
        └── nU3.Modules.EMR.IN.Worklist.dll
```

## 이벤트 모델

### UpdateProgress 이벤트

```csharp
public class ComponentUpdateEventArgs : EventArgs
{
    public UpdatePhase Phase { get; set; }      // Checking, Downloading, Installing, Completed, Failed
    public string ComponentId { get; set; }
    public string ComponentName { get; set; }
    public int CurrentIndex { get; set; }
    public int TotalCount { get; set; }
    public int PercentComplete { get; set; }
    public string ErrorMessage { get; set; }
}
```

### UpdatePhase 열거형

| Phase | 설명 |
|-------|------|
| `Checking` | 업데이트 확인 중 |
| `Downloading` | 서버에서 캐시로 다운로드 |
| `Installing` | 캐시에서 설치 경로로 복사 |
| `Completed` | 업데이트 완료 |
| `Failed` | 업데이트 실패 |

## 배포 시나리오

### 개발 환경

```bash
# Bootstrapper 빌드 후 실행
dotnet run --project nU3.Bootstrapper

# 자동으로:
# 1. DB 초기화
# 2. 테스트 데이터 시드 (DEBUG 모드)
# 3. 컴포넌트 업데이트 확인 (서버 저장소 비어있으면 스킵)
# 4. MainShell 실행
```

### 운영 환경

```
배포 패키지:
├── nU3.Bootstrapper.exe      ← 사용자 실행
├── nU3.MainShell.exe
├── nU3.Core.dll
├── nU3.Core.UI.dll
├── nU3.Data.dll
├── nU3.Models.dll
└── ... (기타 필수 DLL)

서버 저장소 (중앙 관리):
\\server\nU3.Framework\ServerStorage\Components\
├── Framework\...
├── DevExpress\...
└── ...
```

## 참고

- 컴포넌트 배포: `nU3.Tools.Deployer` → "Framework 컴포넌트" 탭
- DB 스키마: `SYS_COMPONENT_MST`, `SYS_COMPONENT_VER`
- 관련 가이드: `INSTALL_PATH_GUIDE.md`, `COMPONENT_DEPLOY_GUIDE.md`

---

## ?⑥씪 ?ㅽ뻾 ?뚯씪 鍮뚮뱶

### 鍮좊Ⅸ ?쒖옉

```batch
# Release 鍮뚮뱶 (?꾨줈?뺤뀡)
build_single.bat

# Debug 鍮뚮뱶 (媛쒕컻)
build_single_debug.bat
```

### 異쒕젰

```
publish/
?쒋?? nU3.Bootstrapper.exe    # ?⑥씪 ?ㅽ뻾 ?뚯씪 (??70-100MB)
?붴?? (湲고? ?뚯씪 ?놁쓬)
```

### 湲곕뒫

- ?⑥씪 EXE ?뚯씪濡?紐⑤뱺 醫낆냽???ы븿
- ?고????ы븿 (蹂꾨룄 .NET ?ㅼ튂 遺덊븘??
- `appsettings.json` 由ъ냼???ы븿
- LOG ?대뜑???먮룞 濡쒓퉭

### ?먯꽭???뺣낫

?먯꽭???댁슜? `BUILD_SINGLE_GUIDE.md` 李몄“
