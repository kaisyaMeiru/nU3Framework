# nU3 Framework - Interface Design Guide

## 개요

nU3 Framework는 SOLID 원칙을 따르며, 특히 ISP(Interface Segregation Principle)를 준수하여 인터페이스를 설계했습니다.

## 인터페이스 구조

### 계층 구조

```
IWorkForm (복합 인터페이스)
├── IScreenIdentifier (화면 식별)
├── ILifecycleAware (생명주기 관리)
├── IWorkContextProvider (컨텍스트 관리)
└── IResourceManager (리소스 정리)
```

## 인터페이스 상세

### 1. IScreenIdentifier - 화면 식별

```csharp
public interface IScreenIdentifier
{
    string ScreenId { get; }      // 화면 고유 ID
    string ScreenTitle { get; }   // 화면 표시 제목
}
```

**책임**: 화면의 식별 정보 제공

**사용 시나리오**:
- 메뉴 시스템과 화면 매핑
- 탭 제목 표시
- 로깅 및 오딧
- 화면 검색 및 활성화

**구현 예시**:
```csharp
public class PatientRegistrationControl : BaseWorkControl
{
    public override string ScreenId => "EMR_PATIENT_REG_001";
    public override string ScreenTitle => "환자 등록";
}
```

### 2. ILifecycleAware - 생명주기 관리

```csharp
public interface ILifecycleAware
{
    void OnActivated();     // 활성화 시
    void OnDeactivated();   // 비활성화 시
    bool CanClose();        // 닫기 전 확인
}
```

**책임**: 화면의 활성화/비활성화 상태 관리

**사용 시나리오**:
- 화면 포커스 획득/상실
- 리소스 최적화 (비활성화 시 타이머 중지 등)
- 닫기 전 변경사항 확인

**구현 예시**:
```csharp
protected override void OnScreenActivated()
{
    _refreshTimer?.Start();
    RefreshData();
}

protected override void OnScreenDeactivated()
{
    _refreshTimer?.Stop();
    SaveTemporaryData();
}

protected override bool OnBeforeClose()
{
    if (HasUnsavedChanges())
    {
        var result = MessageBox.Show("저장하지 않은 변경사항이 있습니다...");
        return result == DialogResult.Yes;
    }
    return true;
}
```

### 3. IWorkContextProvider - 컨텍스트 관리

```csharp
public interface IWorkContextProvider
{
    void InitializeContext(WorkContext context);  // 초기화
    void UpdateContext(WorkContext context);      // 업데이트
    WorkContext GetContext();                     // 조회
}
```

**책임**: 작업 컨텍스트 전달 및 관리

**사용 시나리오**:
- 환자 정보 전달
- 검사 정보 전달
- 사용자 정보 및 권한 전달
- 모듈 간 데이터 공유

**구현 예시**:
```csharp
protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
{
    // 환자 정보 수신
    if (newContext.CurrentPatient != null)
    {
        LoadPatientData(newContext.CurrentPatient.PatientId);
    }

    // 권한에 따라 UI 설정
    btnSave.Enabled = newContext.Permissions.CanUpdate;
    btnDelete.Enabled = newContext.Permissions.CanDelete;
}

protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
{
    // 컨텍스트 변경 시 UI 업데이트
    if (oldContext.CurrentPatient?.PatientId != newContext.CurrentPatient?.PatientId)
    {
        ReloadData();
    }
}
```

### 4. IResourceManager - 리소스 정리

```csharp
public interface IResourceManager
{
    void ReleaseResources();  // 리소스 정리
}
```

**책임**: 리소스 정리 및 메모리 관리

**사용 시나리오**:
- 모듈 언로드
- 메모리 최적화
- 리소스 누수 방지

**구현 예시**:
```csharp
protected override void OnReleaseResources()
{
    // 타이머 정지
    _refreshTimer?.Stop();
    
    // 진행 중인 작업 취소
    _cancellationTokenSource?.Cancel();
    
    // 이벤트 구독 해제
    _eventAggregator?.Unsubscribe<PatientSelectedEvent>(OnPatientSelected);
    
    // 데이터베이스 연결 종료
    _connection?.Close();
}
```

### 5. IWorkForm - 복합 인터페이스

```csharp
public interface IWorkForm : 
    IScreenIdentifier,
    ILifecycleAware,
    IWorkContextProvider,
    IResourceManager
{
    // 위 4개 인터페이스를 조합
}
```

**책임**: 완전한 작업 화면 계약 정의

**사용 시나리오**:
- 모든 기능이 필요한 표준 작업 화면
- MDI 자식 폼
- 동적 로드 모듈

## 선택적 구현

### 부분 구현 예시

일부 기능만 필요한 경우 특정 인터페이스만 구현할 수 있습니다:

#### 1. 간단한 읽기 전용 화면

```csharp
public class SimpleReadOnlyControl : UserControl, IScreenIdentifier, ILifecycleAware
{
    public string ScreenId => "SIMPLE_VIEW_001";
    public string ScreenTitle => "간단한 조회 화면";

    public void OnActivated()
    {
        RefreshData();
    }

    public void OnDeactivated()
    {
        // 특별한 처리 없음
    }

    public bool CanClose()
    {
        return true; // 항상 닫기 허용
    }
}
```

#### 2. 컨텍스트만 필요한 화면

```csharp
public class ContextAwareControl : UserControl, IWorkContextProvider
{
    private WorkContext _context;

    public void InitializeContext(WorkContext context)
    {
        _context = context;
        DisplayPatientInfo(context.CurrentPatient);
    }

    public void UpdateContext(WorkContext context)
    {
        _context = context;
        DisplayPatientInfo(context.CurrentPatient);
    }

    public WorkContext GetContext()
    {
        return _context?.Clone();
    }
}
```

#### 3. 리소스 관리가 필요한 화면

```csharp
public class ResourceIntensiveControl : UserControl, IResourceManager
{
    private Timer _timer;
    private HttpClient _httpClient;

    public ResourceIntensiveControl()
    {
        _timer = new Timer();
        _httpClient = new HttpClient();
    }

    public void ReleaseResources()
    {
        _timer?.Dispose();
        _httpClient?.Dispose();
    }
}
```

## BaseWorkControl과의 관계

`BaseWorkControl`은 `IWorkForm`을 완전히 구현한 기본 클래스입니다:

```csharp
public class BaseWorkControl : UserControl, IWorkForm
{
    // IScreenIdentifier 구현
    public virtual string ScreenId { get; protected set; }
    public virtual string ScreenTitle { get; protected set; }

    // ILifecycleAware 구현
    public virtual void OnActivated() { ... }
    public virtual void OnDeactivated() { ... }
    public virtual bool CanClose() { ... }

    // IWorkContextProvider 구현
    public virtual void InitializeContext(WorkContext context) { ... }
    public virtual void UpdateContext(WorkContext context) { ... }
    public WorkContext GetContext() { ... }

    // IResourceManager 구현
    public virtual void ReleaseResources() { ... }
}
```

### 파생 클래스는 필요한 메서드만 오버라이드

```csharp
public class MyWorkControl : BaseWorkControl
{
    // IScreenIdentifier 구현
    public override string ScreenId => "MY_SCREEN_001";
    public override string ScreenTitle => "내 화면";

    // 필요한 생명주기 메서드만 오버라이드
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        RefreshData();
    }

    // 필요한 컨텍스트 메서드만 오버라이드
    protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
    {
        base.OnContextInitialized(oldContext, newContext);
        LoadPatientData(newContext.CurrentPatient);
    }

    // 필요한 리소스 정리만 추가
    protected override void OnReleaseResources()
    {
        base.OnReleaseResources();
        _myTimer?.Stop();
    }
}
```

## SOLID 원칙 준수

### 1. Single Responsibility Principle (SRP)

각 인터페이스는 하나의 책임만 가짐:
- `IScreenIdentifier`: 화면 식별만
- `ILifecycleAware`: 생명주기 관리만
- `IWorkContextProvider`: 컨텍스트 관리만
- `IResourceManager`: 리소스 정리만

### 2. Open/Closed Principle (OCP)

새로운 기능 추가 시 기존 코드 수정 없이 확장 가능:
```csharp
// 새로운 인터페이스 추가
public interface IDataValidator
{
    bool ValidateData();
}

// 기존 인터페이스는 수정하지 않음
public interface IWorkFormEx : IWorkForm, IDataValidator
{
}
```

### 3. Liskov Substitution Principle (LSP)

모든 파생 클래스는 기본 클래스로 대체 가능:
```csharp
IWorkForm workForm = new PatientRegistrationControl();
ILifecycleAware lifecycle = new PatientRegistrationControl();
IWorkContextProvider contextProvider = new PatientRegistrationControl();
```

### 4. Interface Segregation Principle (ISP)

클라이언트는 사용하지 않는 인터페이스에 의존하지 않음:
```csharp
// 생명주기만 필요한 경우
void ManageLifecycle(ILifecycleAware control)
{
    control.OnActivated();
    // IWorkContextProvider나 IResourceManager는 불필요
}

// 컨텍스트만 필요한 경우
void SetContext(IWorkContextProvider control, WorkContext context)
{
    control.InitializeContext(context);
    // ILifecycleAware나 IResourceManager는 불필요
}
```

### 5. Dependency Inversion Principle (DIP)

구체적인 구현이 아닌 추상화에 의존:
```csharp
public class TabManager
{
    public void ActivateTab(ILifecycleAware control)
    {
        control.OnActivated();
        // BaseWorkControl이 아닌 ILifecycleAware에 의존
    }

    public void SetContext(IWorkContextProvider control, WorkContext context)
    {
        control.InitializeContext(context);
        // BaseWorkControl이 아닌 IWorkContextProvider에 의존
    }
}
```

## 사용 가이드라인

### 1. 전체 기능이 필요한 경우

```csharp
// BaseWorkControl 상속 (권장)
public class MyControl : BaseWorkControl
{
    public override string ScreenId => "MY_001";
    public override string ScreenTitle => "내 화면";
}
```

### 2. 부분 기능만 필요한 경우

```csharp
// 필요한 인터페이스만 구현
public class MyControl : UserControl, IScreenIdentifier, ILifecycleAware
{
    // 필요한 부분만 구현
}
```

### 3. 커스텀 기본 클래스가 필요한 경우

```csharp
// 커스텀 기본 클래스 생성
public class MyBaseControl : DevExpress.XtraEditors.XtraUserControl, IWorkForm
{
    // IWorkForm 완전 구현
}

// 커스텀 기본 클래스 사용
public class MyControl : MyBaseControl
{
    // 커스텀 기능 활용
}
```

## 모범 사례

### 1. 인터페이스 타입으로 의존성 선언

```csharp
// ? Good
public void ProcessScreen(ILifecycleAware screen)
{
    screen.OnActivated();
}

// ? Bad
public void ProcessScreen(BaseWorkControl screen)
{
    screen.OnActivated();
}
```

### 2. 필요한 인터페이스만 요구

```csharp
// ? Good - 생명주기만 필요
public void ActivateControl(ILifecycleAware control)
{
    control.OnActivated();
}

// ? Bad - 불필요한 인터페이스 요구
public void ActivateControl(IWorkForm control)
{
    control.OnActivated();
}
```

### 3. 인터페이스 조합으로 유연성 확보

```csharp
// ? Good - 필요한 기능 조합
public class LightweightControl : UserControl, 
    IScreenIdentifier, 
    ILifecycleAware
{
    // 리소스 관리나 컨텍스트가 불필요한 간단한 화면
}

// ? Good - 전체 기능
public class FullFeaturedControl : BaseWorkControl
{
    // 모든 기능이 필요한 복잡한 화면
}
```

## 확장 예시

### 새로운 인터페이스 추가

```csharp
// 데이터 검증 인터페이스
public interface IDataValidator
{
    bool ValidateData();
    List<string> GetValidationErrors();
}

// 인쇄 기능 인터페이스
public interface IPrintable
{
    void Print();
    bool CanPrint();
}

// 조합하여 사용
public class AdvancedWorkControl : BaseWorkControl, IDataValidator, IPrintable
{
    public bool ValidateData() { ... }
    public List<string> GetValidationErrors() { ... }
    public void Print() { ... }
    public bool CanPrint() => CanPrint; // 권한 활용
}
```

## 마이그레이션 가이드

### 기존 코드를 새 인터페이스 구조로 마이그레이션

#### Before (단일 인터페이스)
```csharp
public interface IWorkForm
{
    string ScreenId { get; }
    void OnActivated();
    void InitializeContext(WorkContext context);
    void ReleaseResources();
}
```

#### After (분리된 인터페이스)
```csharp
public interface IWorkForm : 
    IScreenIdentifier,      // ScreenId
    ILifecycleAware,        // OnActivated
    IWorkContextProvider,   // InitializeContext
    IResourceManager        // ReleaseResources
{
}
```

### 기존 구현체는 변경 없이 호환

```csharp
// 기존 코드 - 변경 불필요
public class MyControl : BaseWorkControl
{
    public override string ScreenId => "MY_001";
    // ... 기존 구현 그대로 사용
}

// 새 코드 - 인터페이스 분리 활용
public void ManageLifecycle(ILifecycleAware control)
{
    control.OnActivated(); // 기존 MyControl도 작동
}
```

## 라이선스

? 2024 nU3 Framework
