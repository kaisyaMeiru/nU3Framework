# BaseWorkControl - 컨텍스트 전달 및 리소스 관리 가이드

## 개요

BaseWorkControl이 동적 로드/언로드를 지원하며, 작업 컨텍스트 전달 및 리소스 관리 기능을 제공합니다.

## 주요 기능

### 1. 작업 컨텍스트 (WorkContext)

모듈 간 데이터 전달을 위한 표준화된 컨텍스트

#### 컨텍스트에 포함되는 정보
- **CurrentPatient**: 현재 선택된 환자 정보
- **CurrentExam**: 현재 선택된 검사 정보  
- **CurrentAppointment**: 현재 선택된 예약 정보
- **CurrentUser**: 현재 로그인한 사용자 정보
- **Permissions**: 모듈별 사용자 권한 정보
- **Parameters**: 부모 모듈에서 전달된 파라미터
- **AdditionalData**: 확장 가능한 추가 데이터

### 2. 권한 관리 (ModulePermissions)

각 모듈별 세밀한 권한 제어

#### 기본 권한
- `CanRead`: 조회 권한
- `CanCreate`: 생성 권한
- `CanUpdate`: 수정 권한
- `CanDelete`: 삭제 권한
- `CanPrint`: 인쇄 권한
- `CanExport`: 내보내기 권한
- `CanApprove`: 승인 권한
- `CanCancel`: 취소 권한
- `CustomPermissions`: 사용자 정의 권한

### 3. 리소스 관리

동적 언로드를 지원하는 명시적 리소스 정리

#### 리소스 관리 기능
- **RegisterDisposable()**: Disposable 리소스 자동 등록
- **ReleaseResources()**: 명시적 리소스 정리 (탭 닫기 시 호출)
- **OnReleaseResources()**: 파생 클래스에서 정리 로직 구현
- **CancellationToken**: 비동기 작업 취소 지원

### 4. 생명주기 관리

화면의 활성화/비활성화 상태 관리

#### 생명주기 메서드
- `OnScreenActivated()`: 화면 활성화 시
- `OnScreenDeactivated()`: 화면 비활성화 시
- `OnBeforeClose()`: 닫기 전

## 사용 방법

### 1. 기본 구조

```csharp
using System;
using nU3.Core.UI;
using nU3.Core.Context;
using nU3.Models;

public class MyWorkControl : BaseWorkControl
{
    public override string ScreenId => "MY_SCREEN_001";
    public override string ScreenTitle => "내 작업 화면";

    public MyWorkControl()
    {
        InitializeLayout();
    }

    protected override void InitializeLayout()
    {
        // UI 초기화
    }
}
```

### 2. 컨텍스트 사용

```csharp
protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
{
    base.OnContextInitialized(oldContext, newContext);

    // 환자 정보 사용
    if (newContext.CurrentPatient != null)
    {
        var patientId = newContext.CurrentPatient.PatientId;
        var patientName = newContext.CurrentPatient.PatientName;
        LoadPatientData(patientId);
    }

    // 사용자 정보 사용
    if (newContext.CurrentUser != null)
    {
        var userId = newContext.CurrentUser.UserId;
        var userName = newContext.CurrentUser.UserName;
    }

    // 파라미터 사용
    var mode = newContext.GetParameter<string>("Mode", "View");
    var recordId = newContext.GetParameter<int>("RecordId", 0);
}

protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
{
    base.OnContextChanged(oldContext, newContext);

    // 컨텍스트 변경 시 UI 업데이트
    UpdateUI();
}
```

### 3. 권한 확인

```csharp
private void BtnSave_Click(object sender, EventArgs e)
{
    // 간단한 권한 확인
    if (!CanUpdate)
    {
        MessageBox.Show("수정 권한이 없습니다.");
        return;
    }

    SaveData();
}

private void InitializeUI()
{
    // 권한에 따라 버튼 활성화
    btnSave.Enabled = CanUpdate;
    btnDelete.Enabled = CanDelete;
    btnPrint.Enabled = CanPrint;
    btnExport.Enabled = CanExport;
}
```

### 4. 리소스 관리

```csharp
public class MyWorkControl : BaseWorkControl
{
    private Timer _refreshTimer;
    private SqlConnection _connection;
    private CancellationTokenSource _searchCancellation;

    public MyWorkControl()
    {
        // 타이머 생성 및 자동 등록
        _refreshTimer = new Timer();
        _refreshTimer.Interval = 5000;
        _refreshTimer.Tick += RefreshTimer_Tick;
        RegisterDisposable(_refreshTimer); // 자동 정리 등록

        // 데이터베이스 연결 (수동 정리 필요)
        _connection = new SqlConnection(connectionString);
        RegisterDisposable(_connection); // 자동 정리 등록
    }

    protected override void OnReleaseResources()
    {
        base.OnReleaseResources();

        // 진행 중인 작업 취소
        _searchCancellation?.Cancel();
        _searchCancellation?.Dispose();
        _searchCancellation = null;

        // 타이머 정지
        _refreshTimer?.Stop();

        // 이벤트 구독 해제
        if (_refreshTimer != null)
            _refreshTimer.Tick -= RefreshTimer_Tick;

        LogInfo("Resources released");
    }
}
```

### 5. 비동기 작업과 취소 토큰

```csharp
private async void BtnSearch_Click(object sender, EventArgs e)
{
    try
    {
        // CancellationToken 사용 (언로드 시 자동 취소)
        var results = await SearchDataAsync(keyword, CancellationToken);
        DisplayResults(results);
    }
    catch (OperationCanceledException)
    {
        LogInfo("Search cancelled");
    }
    catch (Exception ex)
    {
        LogError("Search failed", ex);
    }
}

private async Task<List<Patient>> SearchDataAsync(string keyword, CancellationToken cancellationToken)
{
    // 취소 토큰을 사용한 비동기 작업
    var results = await _patientService.SearchAsync(keyword, cancellationToken);
    return results;
}
```

### 6. 생명주기 관리

```csharp
protected override void OnScreenActivated()
{
    base.OnScreenActivated();

    // 화면 활성화 시
    _refreshTimer?.Start();
    RefreshData();
    LogInfo("Screen activated");
}

protected override void OnScreenDeactivated()
{
    base.OnScreenDeactivated();

    // 화면 비활성화 시
    _refreshTimer?.Stop();
    SaveTemporaryData();
    LogInfo("Screen deactivated");
}

protected override bool OnBeforeClose()
{
    // 닫기 전 확인
    if (HasUnsavedChanges())
    {
        var result = MessageBox.Show(
            "저장하지 않은 변경사항이 있습니다. 닫으시겠습니까?",
            "확인",
            MessageBoxButtons.YesNo);

        return result == DialogResult.Yes;
    }

    return true;
}
```

## 인터페이스 기반 설계

MainShellForm은 인터페이스 기반으로 모듈을 관리합니다:

```csharp
// 컨텍스트 초기화
if (content is IWorkContextProvider contextProvider)
{
    var context = CreateWorkContext();
    contextProvider.InitializeContext(context);
}

// 생명주기 관리
if (control is ILifecycleAware lifecycleAware)
{
    lifecycleAware.OnActivated();
}

// 리소스 정리
if (control is IResourceManager resourceManager)
{
    resourceManager.ReleaseResources();
}
```

## 모범 사례

### 1. 항상 권한 확인

```csharp
private void PerformSensitiveOperation()
{
    if (!CanUpdate)
    {
        MessageBox.Show("권한이 없습니다.");
        LogAudit(AuditAction.Update, "Entity", "123", "Permission denied");
        return;
    }

    DoUpdate();
    LogAudit(AuditAction.Update, "Entity", "123", "Updated successfully");
}
```

### 2. 리소스는 항상 등록

```csharp
public MyWorkControl()
{
    // IDisposable 리소스는 항상 등록
    _timer = new Timer();
    RegisterDisposable(_timer);

    _connection = new SqlConnection();
    RegisterDisposable(_connection);

    _client = new HttpClient();
    RegisterDisposable(_client);
}
```

### 3. 비동기 작업은 취소 토큰 사용

```csharp
private async Task LoadDataAsync()
{
    try
    {
        // CancellationToken 항상 사용
        var data = await _service.GetDataAsync(CancellationToken);
        DisplayData(data);
    }
    catch (OperationCanceledException)
    {
        // 정상적인 취소
        LogInfo("Operation cancelled");
    }
}
```

### 4. 로깅 활용

```csharp
protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
{
    base.OnContextInitialized(oldContext, newContext);

    LogInfo($"Context initialized - User: {newContext.CurrentUser?.UserId}");
    
    if (newContext.CurrentPatient != null)
    {
        LogAudit(AuditAction.Read, "Patient", newContext.CurrentPatient.PatientId);
    }
}
```

## 주의사항

1. **컨텍스트는 불변으로 취급**: Clone()을 사용하여 새 인스턴스 생성
2. **권한 확인 필수**: 모든 민감한 작업 전에 권한 확인
3. **리소스 정리**: RegisterDisposable() 또는 OnReleaseResources() 구현
4. **비동기 작업**: CancellationToken 반드시 사용
5. **로깅**: 중요한 작업은 반드시 로깅 및 오딧

## 라이선스

? 2024 nU3 Framework
