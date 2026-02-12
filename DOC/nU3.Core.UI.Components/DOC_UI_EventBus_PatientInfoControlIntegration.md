# EventBus 통합 문서

## 개요

`PatientInfoControl`과 `PatientListControl` 간의 EventBus 통합을 위한 문서입니다. 두 컨트롤은 EventBus를 통해 환자 선택 이벤트를 상호 통신합니다.

## 구조 변경 사항

### 1. PatientInfoControl 이관 (nU3.Core.UI.Components)

**이전 위치:**
- `SRC/Modules/OCS/IN/nU3.Modules.OCS.IN.MainEntry/Controls/PatientInfoControl.cs`
- `SRC/Modules/OCS/IN/nU3.Modules.OCS.IN.MainEntry/Controls/PatientInfoControl.Designer.cs`
- `SRC/Modules/OCS/IN/nU3.Modules.OCS.IN.MainEntry/Controls/PatientInfoControl.resx`

**새 위치:**
- `SRC/nU3.Core.UI.Components/Controls/PatientInfoControl.cs`
- `SRC/nU3.Core.UI.Components/Controls/PatientInfoControl.Designer.cs`
- `SRC/nU3.Core.UI.Components/Controls/PatientInfoControl.resx`

**주요 변경 사항:**
- 네임스페이스: `nU3.Core.UI.Components.Controls`로 변경
- Base 클래스: `BaseWorkControl` → `BaseWorkComponent`으로 변경
- EventBus 지원: `BaseWorkComponent`에서 제공하는 EventBus, EventBusUse, EventSource 프로퍼티 활용
- 컨트롤: nU3 컨트롤(`nU3GroupControl`, `nU3GridControl`, `nU3PanelControl`, `nU3Button` 등) 사용

### 2. EventBus 통합

#### 이벤트 타입
- `PatientSelectedEvent`: EventBus를 통해 환자 선택 이벤트 전파
- `PatientContext`: 환자 컨텍스트 (PatientId, PatientName, VisitNo)

#### 데이터 흐름

```
PatientListControl (이벤트 발행)
    ↓
    PublishPatientSelectedEvent(PatientInfoDto)
    ↓
    EventBus.GetEvent<PatientSelectedEvent>()
        .Publish(new PatientSelectedEventPayload { Patient, Source })
    ↓
PatientInfoControl (이벤트 구독)
    ↓
    EventBus.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected, ThreadOption.UIThread, true)
    ↓
    OnPatientSelected(PatientContext context)
    ↓
    GetPatientInfoByPatientId(patientId) → PatientInfoDto
    ↓
    SetPatientInfo(patientInfo) → UI 표시
```

## 구현 상세

### PatientListControl (이벤트 발행)

**파일:** `SRC/nU3.Core.UI.Components/Controls/PatientListControl.cs`

```csharp
// EventBus 전파 (명시적인 메서드 사용)
public void PublishPatientSelectedEvent(PatientInfoDto patient)
{
    if (patient == null) return;

    // EventBus를 통해 다른 모듈에 이벤트 발행
    EventBus?.GetEvent<PatientSelectedEvent>()
        .Publish(new PatientSelectedEventPayload
        {
            Patient = patient,
            Source = this.Name
        });

    LogInfo($"PatientSelectedEvent 발행: {patient.PatientName} ({patient.PatientId})");

    // 인스턴스 이벤트 발생
    var eventArgs = new PatientSelectedEventArgs
    {
        Patient = patient,
        Source = EventSource
    };

    PatientSelected?.Invoke(this, eventArgs);
}
```

**사용 예시:**
```csharp
private void gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
{
    _selectedPatient = gridView.GetRow(gridView.FocusedRowHandle) as PatientInfoDto;

    if (_selectedPatient != null)
    {
        // EventBus 전파
        PublishPatientSelectedEvent(_selectedPatient);

        // 인스턴스 이벤트 발생
        PatientSelected?.Invoke(this, new PatientSelectedEventArgs
        {
            Patient = _selectedPatient,
            Source = EventSource
        });
    }
}
```

### PatientInfoControl (이벤트 구독)

**파일:** `SRC/nU3.Core.UI.Components/Controls/PatientInfoControl.cs`

```csharp
// 컨트롤 로드 시 EventBus 이벤트 구독 시작
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);
    SubscribeToPatientSelectedEvent();
}

// PatientSelectedEvent 구독
private void SubscribeToPatientSelectedEvent()
{
    // 디자인 모드에서는 구독 안 함
    if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        return;

    // EventBus가 할당되지 않았으면 부모에서 상속받기
    if (EventBus == null)
        AssignEventBusFromParent();

    if (EventBus == null)
    {
        LogWarning("EventBus가 할당되지 않아 이벤트 구독을 시작할 수 없습니다.");
        return;
    }

    // PatientSelectedEvent 구독
    EventBus.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected, ThreadOption.UIThread, true);

    LogInfo("PatientSelectedEvent 구독 시작");
}

// 환자 선택 이벤트 처리
private void OnPatientSelected(PatientContext context)
{
    if (context == null) return;

    LogInfo($"환자 선택 이벤트 수신: {context.PatientName} ({context.PatientId})");

    // 이전 환자 ID와 비교하여 중복 이벤트 방지
    if (_currentPatientId == context.PatientId)
    {
        LogInfo($"같은 환자 ID로 중복 이벤트 무시: {context.PatientId}");
        return;
    }

    _currentPatientId = context.PatientId;

    // 데모 데이터 조회 (실제로는 DB에서 조회)
    var patientInfo = GetPatientInfoByPatientId(context.PatientId);

    if (patientInfo != null)
    {
        // 환자 정보 표시
        SetPatientInfo(patientInfo);
    }
    else
    {
        // 환자 정보가 없으면 초기화
        ClearPatientInfo();
        LogWarning($"환자 정보를 찾을 수 없음: {context.PatientId}");
    }
}
```

## 사용 방법

### 1. 컨트롤을 Form에 배치

```csharp
// PatientListControl (환자 목록)
var patientListControl = new PatientListControl
{
    EventBusUse = true,
    EventSource = "OCS.MainEntry"
};

// PatientInfoControl (환자 정보)
var patientInfoControl = new PatientInfoControl
{
    EventBusUse = true,
    EventSource = "OCS.MainEntry"
};

// Form에 배치
this.Controls.Add(patientListControl);
this.Controls.Add(patientInfoControl);
```

### 2. 이벤트 처리 (옵션)

```csharp
// PatientListControl에서 인스턴스 이벤트 수신
patientListControl.PatientSelected += (sender, e) =>
{
    Console.WriteLine($"환자 선택됨: {e.Patient.PatientName}");
};

// PatientInfoControl에서 환자 정보 직접 로드
patientInfoControl.LoadPatientInfo("P001234");
```

### 3. EventBus 전파 설정

**기본값:**
- `EventBusUse = false`: EventBus 전파 비활성화 (기본값)

**활성화:**
```csharp
patientListControl.EventBusUse = true;  // PatientListControl에서 이벤트 발행
patientInfoControl.EventBusUse = true;   // PatientInfoControl에서 이벤트 구독
```

## 주의 사항

### 1. EventBus 할당

`BaseWorkComponent`는 부모 컨트롤에서 EventBus를 자동으로 상속받습니다:
```csharp
protected void AssignEventBusFromParent()
{
    var current = this.Parent;

    while (current != null)
    {
        if (current is BaseWorkControl baseWorkControl)
        {
            this.EventBus = baseWorkControl.EventBus;
            break;
        }

        current = current.Parent;
    }
}
```

### 2. 중복 이벤트 방지

`PatientInfoControl`은 환자 ID를 추적하여 중복 이벤트를 방지합니다:
```csharp
if (_currentPatientId == context.PatientId)
{
    return;  // 같은 환자 ID로 중복 이벤트 무시
}
```

### 3. 로깅

모든 이벤트 동작에 대한 로깅이 수행됩니다:
```csharp
LogInfo($"환자 선택 이벤트 수신: {context.PatientName} ({context.PatientId})");
LogInfo($"PatientSelectedEvent 구독 시작");
```

## 테스트 방법

1. **디자인 타임 테스트:**
   - Visual Studio에서 Form에 두 컨트롤을 배치
   - `EventBusUse = true` 설정
   - PatientListControl에서 환자 선택 시 PatientInfoControl이 자동으로 업데이트되는지 확인

2. **런타임 테스트:**
   - 애플리케이션 실행
   - PatientListControl에서 환자 클릭
   - PatientInfoControl의 환자 정보가 업데이트되는지 확인

3. **로그 확인:**
   - 로그 파일에서 `PatientSelectedEvent` 관련 로그 확인
   - 구독/발행 로그 확인

## 향후 개선 사항

1. **데이터 소스 개선:**
   - 데모 데이터 → 실제 Repository 패턴을 통한 DB 조회

2. **이벤트 처리 개선:**
   - 비동기 처리 추가
   - 에러 처리 강화

3. **테스트 추가:**
   - 단위 테스트 작성
   - 통합 테스트 작성

4. **문서화:**
   - 코드 예시 추가
   - 비디오 튜토리얼 추가
