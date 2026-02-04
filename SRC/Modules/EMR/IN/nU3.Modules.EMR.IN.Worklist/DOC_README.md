# 환자 리스트 및 상세정보 모듈 - 사용 가이드

## 개요

환자 목록과 상세정보를 표시하는 두 개의 모듈을 통해 EventBus 기반의 모듈 간 통신을 시연합니다.

## 구현된 모듈

### 1. PatientListControl (환자 목록)
- **ProgramID**: `EMR_PATIENT_LIST_001`
- **역할**: 이벤트 발행자 (Publisher)
- **기능**:
  - 환자 리스트 표시 (DevExpress GridControl)
  - 환자 선택 시 다른 모듈로 이벤트 전파
  - 검색 및 필터링 기능
  - 더블클릭 시 상세화면 열기

### 2. PatientDetailControl (환자 상세정보)
- **ProgramID**: `EMR_PATIENT_DETAIL_001`
- **역할**: 이벤트 구독자 (Subscriber)
- **기능**:
  - 환자 기본 정보 표시
  - 연락처 정보 표시
  - 수신된 이벤트 로그 표시
  - 다른 모듈에서 환자 선택 시 자동 갱신

### 3. SampleWorkControl (샘플 작업화면)
- **ProgramID**: `EMR_SAMPLE_001`
- **역할**: 이벤트 구독자 + 테스트
- **기능**:
  - 환자 선택 이벤트 구독
  - 이벤트 로그 표시
  - 테스트 이벤트 발행 버튼

## 통신 흐름

```
┌─────────────────────┐
│ PatientListControl  │ (환자 목록)
│   (이벤트 발행자)    │
└──────────┬──────────┘
           │ 환자 클릭
           │
           ↓ Publish(PatientSelectedEvent)
           │
┌──────────┴──────────────────────────────┐
│         EventAggregator                 │
│           (EventBus)                    │
└──────────┬────────────┬─────────────────┘
           │            │
           ↓            ↓
┌──────────────────┐  ┌──────────────────┐
│ PatientDetail    │  │ SampleWork       │
│ Control          │  │ Control          │
│ (자동 갱신)       │  │ (자동 갱신)       │
└──────────────────┘  └──────────────────┘
```

## 사용 방법

### 1. 모듈 열기

Deployer를 통해 메뉴에 등록하거나, 직접 프로그램 ID로 열기:

```
메뉴: EMR > 환자 목록
ProgramID: EMR_PATIENT_LIST_001

메뉴: EMR > 환자 상세정보
ProgramID: EMR_PATIENT_DETAIL_001

메뉴: EMR > 샘플 작업화면
ProgramID: EMR_SAMPLE_001
```

### 2. 테스트 시나리오

#### 시나리오 1: 환자 목록 → 상세정보 자동 갱신

1. **환자 목록** 열기
2. **환자 상세정보** 열기
3. **환자 목록**에서 환자 클릭
4. **환자 상세정보**가 자동으로 갱신됨 ?

```
[환자 목록]
    ↓ 환자 클릭 (예: 김철수)
    ↓
[EventBus]
    ↓ PatientSelectedEvent
    ↓
[환자 상세정보]
    ↓ OnPatientSelected() 호출
    ↓ 김철수 정보 자동 표시
```

#### 시나리오 2: MainShell 테스트 환자 선택

1. **환자 상세정보** 열기
2. **시스템 메뉴** > **환자 선택 테스트** 클릭
3. 모든 열려있는 모듈에 테스트 환자 정보 전파됨 ?

#### 시나리오 3: 여러 모듈 동시 갱신

1. **환자 목록** 열기
2. **환자 상세정보** 열기
3. **샘플 작업화면** 열기
4. **환자 목록**에서 환자 클릭
5. 모든 모듈이 동시에 갱신됨 ?

```
[환자 목록] → 이벤트 발행
    ↓
[EventBus] → 브로드캐스트
    ↓
    ├─→ [환자 상세정보] → 갱신됨
    ├─→ [샘플 작업화면] → 갱신됨
    └─→ [기타 열려있는 모듈들] → 갱신됨
```

## 코드 예시

### 발행자 (PatientListControl)

```csharp
// 환자 선택 시
private void GridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
{
    var selectedPatient = GetSelectedPatient();
    if (selectedPatient != null)
    {
        // 1. 자신의 컨텍스트 업데이트
        var newContext = Context.Clone();
        newContext.CurrentPatient = selectedPatient;
        UpdateContext(newContext);

        // 2. 다른 모듈에 이벤트 발행
        EventBus?.GetEvent<PatientSelectedEvent>()
            .Publish(new PatientSelectedEventPayload
            {
                Patient = selectedPatient,
                Source = ProgramID // "EMR_PATIENT_LIST_001"
            });
    }
}
```

### 구독자 (PatientDetailControl)

```csharp
// 화면 활성화 시 이벤트 구독
protected override void OnScreenActivated()
{
    base.OnScreenActivated();

    EventBus?.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected);
}

// 이벤트 수신 처리
private void OnPatientSelected(object payload)
{
    if (payload is not PatientSelectedEventPayload evt)
        return;

    // 자기 자신이 발행한 이벤트는 무시
    if (evt.Source == ProgramID)
        return;

    // 환자 정보 표시
    DisplayPatientInfo(evt.Patient);

    // 컨텍스트 업데이트
    var newContext = Context.Clone();
    newContext.CurrentPatient = evt.Patient;
    UpdateContext(newContext);
}
```

## 주요 기능

### PatientListControl

#### 1. 환자 검색
```csharp
private void BtnSearch_Click(object sender, EventArgs e)
{
    var keyword = txtSearch.Text?.Trim();
    var filtered = _patients.Where(p =>
        p.PatientName.Contains(keyword) ||
        p.PatientId.Contains(keyword)).ToList();

    gridControl.DataSource = filtered;
}
```

#### 2. 환자 선택 전파
```csharp
private void PublishPatientSelected(PatientInfoDto patient)
{
    EventBus?.GetEvent<PatientSelectedEvent>()
        .Publish(new PatientSelectedEventPayload
        {
            Patient = patient,
            Source = ProgramID
        });

    LogInfo($"?? PatientSelectedEvent published: {patient.PatientName}");
}
```

#### 3. 더블클릭 시 상세화면 열기
```csharp
private void GridView_DoubleClick(object sender, EventArgs e)
{
    var selectedPatient = GetSelectedPatient();
    if (selectedPatient != null)
    {
        // 네비게이션 요청
        EventBus?.GetEvent<NavigationRequestEvent>()
            .Publish(new NavigationRequestEventPayload
            {
                TargetScreenId = "EMR_PATIENT_DETAIL_001",
                Context = CreateContextWithPatient(selectedPatient),
                Source = ProgramID
            });
    }
}
```

### PatientDetailControl

#### 1. 다중 이벤트 구독
```csharp
private void SubscribeToEvents()
{
    // 환자 선택 이벤트
    EventBus?.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected);

    // 환자 업데이트 이벤트
    EventBus?.GetEvent<PatientUpdatedEvent>()
        .Subscribe(OnPatientUpdated);

    // 컨텍스트 변경 이벤트
    EventBus?.GetEvent<WorkContextChangedEvent>()
        .Subscribe(OnWorkContextChanged);
}
```

#### 2. 이벤트 로그 표시
```csharp
private void AddEventLog(string message)
{
    var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
    var logMessage = $"[{timestamp}] {message}\r\n";

    memoEventLog.Text += logMessage;
    memoEventLog.SelectionStart = memoEventLog.Text.Length;
    memoEventLog.ScrollToCaret();
}
```

#### 3. 자동 갱신
```csharp
private void OnPatientSelected(object payload)
{
    if (payload is not PatientSelectedEventPayload evt)
        return;

    AddEventLog($"?? PatientSelectedEvent received from '{evt.Source}'");
    AddEventLog($"   Patient: {evt.Patient.PatientName}");

    // UI 자동 갱신
    DisplayPatientInfo(evt.Patient);
    UpdateStatus($"환자 선택됨: {evt.Patient.PatientName}", Color.Blue);
}
```

## 이벤트 타입

### PatientSelectedEvent
```csharp
public class PatientSelectedEventPayload
{
    public PatientInfoDto Patient { get; set; }  // 선택된 환자
    public string Source { get; set; }            // 이벤트 발행자 ID
}
```

**용도**: 환자가 선택되었을 때 다른 모듈에 알림

### PatientUpdatedEvent
```csharp
public class PatientUpdatedEventPayload
{
    public PatientInfoDto Patient { get; set; }   // 업데이트된 환자
    public string Source { get; set; }
    public string UpdatedBy { get; set; }         // 업데이트 수행자
}
```

**용도**: 환자 정보가 변경되었을 때 다른 모듈에 알림

### WorkContextChangedEvent
```csharp
public class WorkContextChangedEventPayload
{
    public WorkContext OldContext { get; set; }   // 이전 컨텍스트
    public WorkContext NewContext { get; set; }   // 새 컨텍스트
    public string Source { get; set; }
    public string ChangedProperty { get; set; }   // 변경된 속성
}
```

**용도**: 전체 작업 컨텍스트가 변경되었을 때

### NavigationRequestEvent
```csharp
public class NavigationRequestEventPayload
{
    public string TargetScreenId { get; set; }    // 열 화면 ID
    public WorkContext Context { get; set; }      // 전달할 컨텍스트
    public string Source { get; set; }
}
```

**용도**: 다른 화면을 열도록 요청

## 모범 사례

### 1. 항상 Source 확인
```csharp
// ? Good - 자신이 발행한 이벤트는 무시
if (evt.Source == ProgramID)
    return;

// ? Bad - 무한 루프 위험
// Source 확인 없이 모든 이벤트 처리
```

### 2. Null 체크
```csharp
// ? Good
if (payload is not PatientSelectedEventPayload evt)
    return;

if (evt.Patient == null)
    return;

// ? Bad
var evt = (PatientSelectedEventPayload)payload; // NullReferenceException 위험
```

### 3. UI 스레드 확인
```csharp
// ? Good - 크로스 스레드 안전
if (InvokeRequired)
{
    Invoke(new Action(() => DisplayPatientInfo(patient)));
}
else
{
    DisplayPatientInfo(patient);
}
```

### 4. 로깅 활용
```csharp
// ? Good - 디버깅 용이
AddEventLog($"?? Event received from {evt.Source}");
LogInfo($"Patient selected: {patient.PatientName}");
LogAudit(AuditAction.Read, "Patient", patient.PatientId);
```

## 트러블슈팅

### 1. 이벤트가 수신되지 않음

**원인**: EventBus가 설정되지 않음

**해결**:
```csharp
protected override void OnScreenActivated()
{
    base.OnScreenActivated();

    // EventBus null 체크
    if (EventBus == null)
    {
        LogWarning("EventBus is not set!");
        return;
    }

    // 이벤트 구독
    EventBus.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected);
}
```

### 2. 이벤트가 중복으로 처리됨

**원인**: 자신이 발행한 이벤트도 수신

**해결**:
```csharp
private void OnPatientSelected(object payload)
{
    if (payload is not PatientSelectedEventPayload evt)
        return;

    // 자신이 발행한 이벤트는 무시
    if (evt.Source == ProgramID)
        return;

    // 처리
}
```

### 3. UI가 업데이트되지 않음

**원인**: 크로스 스레드 접근

**해결**:
```csharp
private void OnPatientSelected(object payload)
{
    // ... 이벤트 처리 ...

    // UI 스레드에서 실행
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateUI(patient)));
    }
    else
    {
        UpdateUI(patient);
    }
}
```

## 확장 가능성

### 다른 이벤트 추가

```csharp
// 1. 이벤트 정의 (StandardEvents.cs)
public class VitalSignUpdatedEvent : PubSubEvent { }

public class VitalSignUpdatedEventPayload
{
    public VitalSignDto VitalSign { get; set; }
    public PatientInfoDto Patient { get; set; }
    public string Source { get; set; }
}

// 2. 발행
EventBus?.GetEvent<VitalSignUpdatedEvent>()
    .Publish(new VitalSignUpdatedEventPayload
    {
        VitalSign = vitalSign,
        Patient = patient,
        Source = ProgramID
    });

// 3. 구독
EventBus?.GetEvent<VitalSignUpdatedEvent>()
    .Subscribe(OnVitalSignUpdated);
```

### 다른 모듈에서 사용

```csharp
// 어떤 모듈에서든 동일한 패턴 사용
public class MyCustomControl : BaseWorkControl
{
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();

        // 환자 선택 이벤트 구독
        EventBus?.GetEvent<PatientSelectedEvent>()
            .Subscribe(OnPatientSelected);
    }

    private void OnPatientSelected(object payload)
    {
        if (payload is not PatientSelectedEventPayload evt)
            return;

        if (evt.Source == ProgramID)
            return;

        // 나만의 처리 로직
        ProcessPatientSelection(evt.Patient);
    }
}
```

## 성능 최적화

### 1. 이벤트 필터링
```csharp
// 관련 없는 이벤트는 빠르게 무시
private void OnPatientSelected(object payload)
{
    if (payload is not PatientSelectedEventPayload evt)
        return; // 빠른 리턴

    if (evt.Source == ProgramID)
        return; // 빠른 리턴

    // 현재 필요한 경우만 처리
    if (!IsNeedToProcess(evt))
        return;

    // 실제 처리
    Process(evt);
}
```

### 2. 대량 데이터는 ID만 전달
```csharp
// ? Bad - 큰 데이터 전달
public class PatientDetailDataPayload
{
    public List<ExamResultDto> AllExamResults { get; set; } // 대량 데이터
    public List<VitalSignDto> AllVitalSigns { get; set; }
}

// ? Good - ID만 전달하고 필요시 조회
public class PatientDataChangedPayload
{
    public string PatientId { get; set; }
    public string ChangedType { get; set; } // "ExamResult", "VitalSign" 등
}
```

## 라이선스

? 2024 nU3 Framework
