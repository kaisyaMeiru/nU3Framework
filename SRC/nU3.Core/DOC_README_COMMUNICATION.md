# nU3 Framework - 모듈 간 통신 가이드

## 개요

nU3 Framework는 두 가지 통신 방식을 지원합니다:
1. **강결합 통신**: WorkContext를 통한 직접 전달 (오픈 시)
2. **약결합 통신**: EventAggregator를 통한 이벤트 기반 통신 (런타임)

## 이벤트 시스템 아키텍처

```
┌─────────────────────────────────────────┐
│      IEventAggregator (중재자)          │
│                                         │
│  GetEvent<TEvent>() where TEvent :      │
│      PubSubEvent, new()                 │
└───────────┬─────────────────────────────┘
            │
            ↓
┌───────────────────────────────────────────┐
│      PubSubEvent (이벤트 기반 클래스)      │
│                                           │
│  Subscribe(Action<object> action)        │
│  Publish(object payload)                 │
└───────────────────────────────────────────┘
            │
            ↓
┌───────────────────────────────────────────┐
│     구체적인 이벤트 클래스들                │
│  - PatientSelectedEvent                  │
│  - ExamSelectedEvent                     │
│  - WorkContextChangedEvent               │
│  - NavigationRequestEvent등              │
└───────────────────────────────────────────┘
```

## 통신 패턴

```
┌──────────────────────────────────────────────────────────┐
│                    통신 패턴                              │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  1. 강결합 (모듈 오픈 시)                                 │
│     MainShell ──InitializeContext──→ Module             │
│                                                          │
│  2. 약결합 (런타임)                                      │
│     MainShell ←──EventAggregator──→ Module              │
│     Module A  ←──EventAggregator──→ Module B            │
│                                                          │
└──────────────────────────────────────────────────────────┘

### 1. MainShell → Module (강결합)

모듈 오픈 시 직접 컨텍스트를 전달합니다.

```csharp
// MainShellForm.cs
private Control CreateProgramContent(Type type)
{
    var content = (Control)Activator.CreateInstance(type);
    
    // 1. 모듈 오픈 시 (강결합)
    if (content is BaseWorkControl workControl)
    {
        workControl.EventBus = _eventAggregator; // EventBus 설정
        
        // 초기 컨텍스트 전달 (강결합)
        var context = CreateWorkContext();
        workControl.InitializeContext(context);
    }
    
    return content;
}
```

### 2. MainShell ↔ Module (약결합)

런타임 중 EventAggregator를 통한 양방향 통신입니다.

#### MainShell에서 이벤트 발행

```csharp
// MainShellForm.cs
public class MainShellForm : BaseWorkForm
{
    private readonly IEventAggregator _eventAggregator;
    
    // 환자 선택 시
    public void SelectPatient(PatientInfoDto patient)
    {
        if (patient == null)
            return;

        try
        {
            // 이벤트 발행
            _eventAggregator?.GetEvent<PatientSelectedEvent>()
                .Publish(new PatientSelectedEventPayload 
                { 
                    Patient = patient, 
                    Source = "MainShell" 
                });

            // 컨텍스트 생성 및 브로드캐스트
            var context = new WorkContext();
            context.CurrentPatient = patient;
            
            BroadcastContextChange(context);

            LogManager.Info($"Patient selected: {patient.PatientName}", "Shell");
        }
        catch (Exception ex)
        {
            LogManager.Error($"Error selecting patient: {ex.Message}", "Shell", ex);
        }
    }
    
    // 전체 컨텍스트 변경 브로드캐스트
    public void BroadcastContextChange(WorkContext newContext)
    {
        try
        {
            // 이벤트 발행
            _eventAggregator?.GetEvent<WorkContextChangedEvent>()
                .Publish(new WorkContextChangedEventPayload 
                { 
                    NewContext = newContext, 
                    Source = "MainShell" 
                });
            
            // 2. 런타임 브로드캐스트 (약결합)
            // 모든 열린 탭에 직접 업데이트
            foreach (XtraTabPage page in xtraTabControlMain.TabPages)
            {
                if (page.Controls.Count > 0 && page.Controls[0] is IWorkContextProvider contextProvider)
                {
                    contextProvider.UpdateContext(newContext);
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.Error($"Error broadcasting context: {ex.Message}", "Shell", ex);
        }
    }
}
```

#### Module에서 이벤트 구독

```csharp
// PatientDetailControl.cs
public class PatientDetailControl : BaseWorkControl
{
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        
        // 환자 선택 이벤트 구독
        EventBus?.GetEvent<PatientSelectedEvent>()
            .Subscribe(OnPatientSelected);
            
        // 컨텍스트 변경 이벤트 구독
        EventBus?.GetEvent<WorkContextChangedEvent>()
            .Subscribe(OnContextChanged);
    }
    
    private void OnPatientSelected(object payload)
    {
        if (payload is not PatientSelectedEventPayload evt)
            return;

        // 다른 모듈에서 발행한 경우만 처리
        if (evt.Source == ScreenId)
            return;

        // 환자 정보 업데이트
        var newContext = Context.Clone();
        newContext.CurrentPatient = evt.Patient;
        UpdateContext(newContext);
        
        LoadPatientData(evt.Patient.PatientId);
        
        LogInfo($"Received patient selection: {evt.Patient.PatientName}");
    }
    
    private void OnContextChanged(object payload)
    {
        if (payload is not WorkContextChangedEventPayload evt)
            return;

        // 환자가 변경된 경우
        if (evt.OldContext?.CurrentPatient?.PatientId != evt.NewContext?.CurrentPatient?.PatientId)
        {
            RefreshData();
        }
    }
}
```

### 3. Module ↔ Module (약결합)

모듈 간 직접 통신은 EventAggregator를 통해서만 가능합니다.

#### 발행자 모듈

```csharp
// PatientListControl.cs
public class PatientListControl : BaseWorkControl
{
    private void GridView_RowClick(object sender, RowClickEventArgs e)
    {
        var selectedPatient = GetSelectedPatient();
        
        // 컨텍스트 업데이트
        var newContext = Context.Clone();
        newContext.CurrentPatient = selectedPatient;
        UpdateContext(newContext);
        
        // 다른 모듈에 알림
        EventBus?.GetEvent<PatientSelectedEvent>()
            .Publish(new PatientSelectedEventPayload 
            { 
                Patient = selectedPatient, 
                Source = ScreenId 
            });
        
        LogAudit(AuditAction.Read, "Patient", selectedPatient.PatientId);
    }
    
    private void BtnSave_Click(object sender, EventArgs e)
    {
        var patient = SavePatient();
        
        // 저장 후 다른 모듈에 변경 알림
        EventBus?.GetEvent<PatientUpdatedEvent>()
            .Publish(new PatientUpdatedEventPayload 
            { 
                Patient = patient, 
                Source = ScreenId,
                UpdatedBy = Context.CurrentUser?.UserId 
            });
    }
}
```

#### 구독자 모듈 (자동 갱신)

```csharp
// PatientChartControl.cs
public class PatientChartControl : BaseWorkControl
{
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        
        // 환자 업데이트 이벤트 구독
        EventBus?.GetEvent<PatientUpdatedEvent>()
            .Subscribe(OnPatientUpdated);
            
        // 검사 결과 업데이트 이벤트 구독
        EventBus?.GetEvent<ExamResultUpdatedEvent>()
            .Subscribe(OnExamResultUpdated);
    }
    
    private void OnPatientUpdated(object payload)
    {
        if (payload is not PatientUpdatedEventPayload evt)
            return;

        // 현재 표시 중인 환자가 업데이트되었는지 확인
        if (Context.CurrentPatient?.PatientId == evt.Patient.PatientId)
        {
            // 자동 갱신
            RefreshPatientData();
            
            MessageBox.Show("환자 정보가 업데이트되었습니다.", "알림");
        }
    }
    
    private void OnExamResultUpdated(object payload)
    {
        if (payload is not ExamResultUpdatedEventPayload evt)
            return;

        // 현재 환자의 검사 결과인지 확인
        if (IsCurrentPatientExam(evt.Result))
        {
            RefreshChart();
            ShowNotification($"새 검사 결과: {evt.Result.ExamName}");
        }
    }
}
```

## 실전 시나리오

### 시나리오 1: 환자 등록 → 상세 조회

```csharp
// Step 1: 등록 화면에서 환자 저장
public class PatientRegistrationControl : BaseWorkControl
{
    private void BtnSave_Click(object sender, EventArgs e)
    {
        var patient = SaveNewPatient();
        
        // 등록 완료 이벤트 발행
        EventBus?.GetEvent<PatientSelectedEvent>()
            .Publish(new PatientSelectedEventPayload 
            { 
                Patient = patient, 
                Source = ScreenId 
            });
        
        // 상세 화면 열기 요청
        var context = Context.Clone();
        context.CurrentPatient = patient;
        context.SetParameter("Mode", "View");
        
        EventBus?.GetEvent<NavigationRequestEvent>()
            .Publish(new NavigationRequestEventPayload 
            { 
                TargetScreenId = "PATIENT_DETAIL_001", 
                Context = context, 
                Source = ScreenId 
            });
    }
}

// Step 2: MainShell에서 네비게이션 처리
public class MainShellForm : BaseWorkForm
{
    private void SubscribeToEvents()
    {
        _eventAggregator?.GetEvent<NavigationRequestEvent>()
            .Subscribe(OnNavigationRequest);
    }
    
    private void OnNavigationRequest(object payload)
    {
        if (payload is not NavigationRequestEventPayload evt)
            return;

        try
        {
            // 프로그램 열기
            OpenProgram(evt.TargetScreenId);
            
            // 컨텍스트 전달
            if (evt.Context != null)
            {
                var tabPage = FindTabByProgId(evt.TargetScreenId);
                if (tabPage?.Controls[0] is IWorkContextProvider contextProvider)
                {
                    contextProvider.UpdateContext(evt.Context);
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.Error($"Navigation failed: {ex.Message}", "Shell", ex);
        }
    }
}
```

### 시나리오 2: 검사 결과 입력 → 차트 갱신

```csharp
// Step 1: 검사 결과 입력 화면
public class ExamResultEntryControl : BaseWorkControl
{
    private void BtnSave_Click(object sender, EventArgs e)
    {
        var result = SaveExamResult();
        
        // 결과 업데이트 이벤트 발행
        EventBus?.GetEvent<ExamResultUpdatedEvent>()
            .Publish(new ExamResultUpdatedEventPayload 
            { 
                Result = result, 
                Source = ScreenId,
                UpdatedBy = Context.CurrentUser?.UserId 
            });
        
        LogAudit(AuditAction.Create, "ExamResult", result.ResultId);
    }
}

// Step 2: 차트 화면에서 자동 갱신
public class PatientChartControl : BaseWorkControl
{
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        EventBus?.GetEvent<ExamResultUpdatedEvent>()
            .Subscribe(OnExamResultUpdated);
    }
    
    private void OnExamResultUpdated(object payload)
    {
        if (payload is not ExamResultUpdatedEventPayload evt)
            return;

        // 현재 환자의 검사 결과인지 확인
        if (IsCurrentPatientExam(evt.Result))
        {
            RefreshChart();
            ShowNotification($"새로운 검사 결과: {evt.Result.ExamName}");
        }
    }
}
```

## 이벤트 정의 가이드

### 새로운 이벤트 만들기

```csharp
// nU3.Core/Events/StandardEvents.cs

/// <summary>
/// 바이탈 사인 업데이트 이벤트
/// </summary>
public class VitalSignUpdatedEvent : PubSubEvent { }

public class VitalSignUpdatedEventPayload
{
    public VitalSignDto VitalSign { get; set; }
    public PatientInfoDto Patient { get; set; }
    public string Source { get; set; }
    public string UpdatedBy { get; set; }
}
```

### 사용 예시

```csharp
// 발행
EventBus?.GetEvent<VitalSignUpdatedEvent>()
    .Publish(new VitalSignUpdatedEventPayload 
    { 
        VitalSign = vitalSign, 
        Patient = patient, 
        Source = ScreenId,
        UpdatedBy = Context.CurrentUser?.UserId 
    });

// 구독
EventBus?.GetEvent<VitalSignUpdatedEvent>()
    .Subscribe(OnVitalSignUpdated);

// 핸들러
private void OnVitalSignUpdated(object payload)
{
    if (payload is not VitalSignUpdatedEventPayload evt)
        return;

    if (evt.Patient.PatientId == Context.CurrentPatient?.PatientId)
    {
        RefreshVitalSignChart();
    }
}
```

## 모범 사례

### 1. 항상 Source 명시

```csharp
// ✅ Good
EventBus?.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload 
    { 
        Patient = patient, 
        Source = ScreenId // 소스 명시
    });
```

### 2. 자기 자신이 발행한 이벤트는 무시

```csharp
// ✅ Good
private void OnPatientSelected(object payload)
{
    if (payload is not PatientSelectedEventPayload evt)
        return;

    // 자신이 발행한 이벤트는 무시
    if (evt.Source == ScreenId)
        return;

    // 처리
    UpdatePatientInfo(evt.Patient);
}
```

### 3. Null 체크 철저히

```csharp
// ✅ Good
private void OnPatientUpdated(object payload)
{
    if (payload is not PatientUpdatedEventPayload evt)
        return; // Payload가 null이거나 타입이 다르면 무시

    if (evt.Patient == null)
        return; // Patient가 null이면 무시

    // 처리
    RefreshData();
}
```

### 4. 조건부 처리

```csharp
// ✅ Good - 현재 화면과 관련된 이벤트만 처리
private void OnExamResultUpdated(object payload)
{
    if (payload is not ExamResultUpdatedEventPayload evt)
        return;

    // 현재 환자의 검사가 아니면 무시
    if (evt.Result.PatientId != Context.CurrentPatient?.PatientId)
        return;

    RefreshData();
}
```

### 5. WeakReference 사용 (자동 구독 해제)

```csharp
// PubSubEvent는 내부적으로 WeakReference 사용
// 구독자가 GC되면 자동으로 구독 해제됨

// 명시적 구독 해제는 선택사항
protected override void OnScreenDeactivated()
{
    base.OnScreenDeactivated();
    
    // 필요하다면 명시적으로 구독 해제 가능
    // (하지만 WeakReference 덕분에 필수는 아님)
}
```

## 통신 흐름도

```
┌─────────────┐
│ MainShell   │
│  (강결합)   │
└──────┬──────┘
       │ InitializeContext()
       ↓
┌─────────────┐      Publish      ┌─────────────┐
│  Module A   │ ────────────────→ │  EventBus   │
│             │                    │  (약결합)   │
└─────────────┘                    └──────┬──────┘
                                          │ Subscribe
                                          ↓
                                   ┌─────────────┐
                                   │  Module B   │
                                   │             │
                                   └─────────────┘
```

## 성능 고려사항

1. **이벤트 빈도**: 너무 자주 발생하는 이벤트는 성능 저하 유발
2. **WeakReference**: PubSubEvent는 WeakReference 사용으로 메모리 누수 방지
3. **페이로드 크기**: 큰 데이터는 ID만 전달하고 필요시 조회
4. **선택적 처리**: 관련 없는 이벤트는 빠르게 무시

## 디버깅 팁

### 이벤트 로깅 추가

```csharp
// 이벤트 발행 시
EventBus?.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload 
    { 
        Patient = patient, 
        Source = ScreenId 
    });

LogInfo($"Event published: PatientSelected from {ScreenId}");

// 이벤트 수신 시
private void OnPatientSelected(object payload)
{
    LogInfo($"Event received: PatientSelected in {ScreenId}");
    
    // 처리...
}
```


## 이벤트 흐름도

```
[환자 목록] 
    ↓ 환자 클릭
    ↓ EventBus.Publish(PatientSelectedEvent)
    ↓
[EventAggregator]
    ↓ Subscribe
    ├─→ [환자 상세] (자동 갱신)
    ├─→ [차트 뷰어] (자동 갱신)
    ├─→ [검사 목록] (자동 필터링)
    └─→ [예약 목록] (자동 필터링)
    
[검사 결과 입력]
    ↓ 결과 저장
    ↓ EventBus.Publish(ExamResultUpdatedEvent)
    ↓
[EventAggregator]
    ↓ Subscribe
    ├─→ [환자 차트] (차트 자동 갱신)
    ├─→ [검사 이력] (목록 자동 갱신)
    └─→ [통계 대시보드] (통계 자동 갱신)
```

## 시나리오 추가

```
[환자 등록]
    ↓ 등록 완료
    ↓ EventBus.Publish(NavigationRequestEvent)
    ↓
[MainShell]
    ↓ OnNavigationRequest
    ├─→ OpenProgram("PATIENT_DETAIL_001")
    └─→ UpdateContext (환자 정보 전달)
        ↓
    [환자 상세] (새로 열리고 환자 정보 표시)




    ## 라이선스

© 2026 nU3 Framework
