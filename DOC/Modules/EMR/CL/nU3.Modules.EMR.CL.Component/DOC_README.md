# nU3.Modules.EMR.Clinic - 외래진료 모듈

## 개요

외래진료 모듈은 **MVVM 패턴(EventBus)**과 **이벤트 핸들러 패턴**을 하이브리드로 사용하여 구현된 참고 구현체입니다.

- **MVVM 패턴**: 모듈 간 통신 (EventBus 기반)
- **이벤트 핸들러 패턴**: UI Components 간 통신

## 프로젝트 구조

```
nU3.Modules.EMR.Clinic/
├── nU3.Modules.EMR.Clinic.csproj
├── ClinicMainControl.cs           # 메인 컨트롤 (EventBus + UI Components)
├── ClinicPatientControl.cs         # 환자 선택 컨트롤 (PatientSelectorControl)
├── ClinicVisitControl.cs           # 진료 기록 컨트롤 (DateRangeControl, ChecklistControl)
├── ClinicStatsControl.cs           # 통계 컨트롤
└── README.md                        # 이 파일
```

## 패턴 비교 분석

### 1. MVVM 패턴 (EventBus) - 모듈 간 통신

**사용 위치:** `ClinicMainControl.cs`

```csharp
// 이벤트 구독 (MVVM 패턴)
public override void InitializeContext(WorkContext context)
{
    base.InitializeContext(context);

    if (EventBus == null)
        return;

    // 모듈 간 통신을 위한 EventBus 구독
    EventBus.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelectedFromModule);

    EventBus.GetEvent<VisitInfoUpdatedEvent>()
        .Subscribe(OnVisitInfoUpdated);
}

// 이벤트 발행 (MVVM 패턴)
private void PublishPatientSelected(PatientInfoDto patient)
{
    EventBus?.GetEvent<PatientSelectedEvent>()
        .Publish(new PatientSelectedEventPayload
        {
            Patient = patient,
            Source = ProgramID
        });
}

// 이벤트 처리기 (MVVM 패턴)
private void OnPatientSelectedFromModule(object payload)
{
    if (payload is not PatientSelectedEventPayload evt)
        return;

    // 자기 자신이 발행한 이벤트는 무시
    if (evt.Source == ProgramID)
        return;

    // 다른 모듈에서 온 이벤트 처리
    UpdatePatientUI(evt.Patient);
}
```

**특징:**
- 느슨한 결합 (Loose Coupling)
- Publisher/Subscriber 패턴
- 모듈 간 확장성이 좋음
- Source ID로 순환 참조 방지

**장점:**
- ✅ 모듈 간 독립적 개발 가능
- ✅ 새로운 구독자 추가가 쉬움
- ✅ 테스트 용이성이 높음
- ✅ 확장성이 좋음

**단점:**
- ❌ EventBus 개념 이해 필요
- ❌ 디버깅이 어려울 수 있음
- ❌ 오버헤드 발생 가능

---

### 2. 이벤트 핸들러 패턴 - UI Components 통신

**사용 위치:** `ClinicPatientControl.cs`, `ClinicVisitControl.cs`

```csharp
// ClinicPatientControl.cs - PatientSelectorControl 사용
public partial class ClinicPatientControl : XtraUserControl
{
    private PatientSelectorControl? _patientSelector;

    public event EventHandler<PatientSelectedEventArgs>? PatientSelected;

    private void InitializeControls()
    {
        _patientSelector = new PatientSelectorControl
        {
            AutoSearch = true,
            SearchLimit = 100
        };

        // UI Component 이벤트 구독 (이벤트 핸들러 패턴)
        _patientSelector.PatientSelected += OnPatientSelected;
    }

    // UI Component 이벤트 처리
    private void OnPatientSelected(object? sender, PatientSelectedEventArgs e)
    {
        _currentPatient = e.Patient;

        // 상위 컨트롤로 이벤트 전달 (이벤트 핸들러 패턴)
        PatientSelected?.Invoke(this, new PatientSelectedEventArgs
        {
            Patient = _currentPatient,
            Source = "ClinicPatientControl"
        });
    }
}
```

**특징:**
- 강한 결합 (Tight Coupling) - 직접 참조
- 단순하고 직관적
- UI Component 수준에서 사용

**장점:**
- ✅ 간단하고 직관적
- ✅ 학습 곡선이 낮음
- ✅ DevExpress DataSource 활용 용이
- ✅ 빠른 개발 가능

**단점:**
- ❌ 상위 컨트롤에 강하게 의존
- ❌ 재사용성이 제한적
- ❌ 테스트가 어려움

---

## 하이브리드 접근 방식 (이 프로젝트)

```
┌─────────────────────────────────────────────────────────┐
│              ClinicMainControl (EventBus)                │
│  ┌──────────────────────────────────────────────────┐  │
│  │  MVVM 패턴: 모듈 간 통신                          │  │
│  │   - EventBus.GetEvent<PatientSelectedEvent>()    │  │
│  │   - Publish/Publish()                             │  │
│  │   - 다른 모듈과 느슨하게 결합                    │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                         ↓ 사용
┌─────────────────────────────────────────────────────────┐
│            UI Components (이벤트 핸들러)                   │
│  ┌──────────────────────────────────────────────────┐  │
│  │  ClinicPatientControl                             │  │
│  │    - PatientSelectorControl 사용                 │  │
│  │    - 이벤트 핸들러로 상위와 통신                  │  │
│  │  ClinicVisitControl                               │  │
│  │    - DateRangeControl 사용                        │  │
│  │    - ChecklistControl 사용                        │  │
│  │    - 이벤트 핸들러로 상위와 통신                  │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### ClinicMainControl - 하이브리드 패턴 구현

```csharp
public partial class ClinicMainControl : BaseWorkControl
{
    private ClinicPatientControl? _patientControl;
    private ClinicVisitControl? _visitControl;

    private void InitializeControls()
    {
        // 1. UI Components 이벤트 구독 (이벤트 핸들러 패턴)
        _patientControl.PatientSelected += OnPatientSelectedFromControl;
        _visitControl.VisitRecorded += OnVisitRecordedFromControl;
    }

    // 이벤트 핸들러: UI Component에서 온 이벤트
    private void OnPatientSelectedFromControl(object? sender, PatientSelectedEventArgs e)
    {
        UpdatePatientUI(e.Patient);

        // 2. EventBus로 브로드캐스트 (MVVM 패턴)
        PublishPatientSelected(e.Patient);

        UpdateContext(newContext);
    }

    // MVVM: 다른 모듈에서 온 이벤트
    private void OnPatientSelectedFromModule(object payload)
    {
        if (payload is not PatientSelectedEventPayload evt)
            return;

        if (evt.Source == ProgramID)  // 자기 이벤트 무시
            return;

        UpdatePatientUI(evt.Patient);
    }
}
```

## 각 패턴 사용 가이드라인

| 상황 | 사용할 패턴 | 이유 |
|------|-----------|------|
| **모듈 간 통신** | MVVM (EventBus) | 느슨한 결합, 확장성 |
| **UI Component ↔ 상위 컨트롤** | 이벤트 핸들러 | 간단함, 직접 참조 필요 |
| **단순 컨트롤 내부** | 이벤트 핸들러 | 학습 곡선 낮음 |
| **복잡한 비즈니스 로직** | MVVM (EventBus) | 테스트 용이성 |
| **DevExpress DataSource 사용** | 이벤트 핸들러 | 데이터 바인딩 쉬움 |

## 실제 실행 예시

### 1. 사용자가 환자를 선택

```
사용자 → PatientSelectorControl (UI Component)
  → OnPatientSelected 이벤트 발생 (이벤트 핸들러)
    → ClinicPatientControl에서 처리
      → ClinicMainControl로 이벤트 전달 (이벤트 핸들러)
        → EventBus로 브로드캐스트 (MVVM)
          → 다른 모듈(PatientDetail, Worklist 등)에 수신
```

### 2. 다른 모듈에서 환자 선택 이벤트 수신

```
다른 모듈 → EventBus.Publish(PatientSelectedEvent)
  → ClinicMainControl에서 수신 (MVVM)
    → UI 업데이트 (ClinicPatientControl, ClinicVisitControl)
```

## 빌드 및 실행

```bash
# 프로젝트 빌드
dotnet build SRC\Modules/EMR/Clinic/nU3.Modules.EMR.Clinic/nU3.Modules.EMR.Clinic.csproj

# 전체 솔루션 빌드
dotnet build nU3.Framework.sln
```

## 참고 자료

### 비교 분석

| 특징 | MVVM (EventBus) | 이벤트 핸들러 |
|------|------------------|---------------|
| **통신 범위** | 모듈 간 / 애플리케이션 전체 | 컨트롤 내부 / 부모-자식 |
| **결합도** | 느슨함 (Loose Coupling) | 강함 (Tight Coupling) |
| **데이터 바인딩** | Context 직접 업데이트 | DevExpress DataSource |
| **테스트 용이성** | 좋음 (ViewModel 분리 가능) | 제한적 (UI에 강하게 의존) |
| **학습 곡선** | 높음 (EventBus 이해 필요) | 낮음 (기존 WinForms 방식) |
| **적용 범위** | 복잡한 모듈 간 통신 | 단일 컨트롤 |

### 코드 패턴 비교

**MVVM 패턴:**
```csharp
// EventBus 기반 모듈 간 통신
EventBus.GetEvent<PatientSelectedEvent>()
    .Subscribe(OnPatientSelected);

EventBus.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload { ... });

// Source ID로 순환 참조 방지
if (evt.Source == ProgramID)
    return;
```

**이벤트 핸들러 패턴:**
```csharp
// UI Component 이벤트 구독
_patientSelector.PatientSelected += OnPatientSelected;

// 이벤트 발행
PatientSelected?.Invoke(this, new PatientSelectedEventArgs { ... });
```

## 결론

이 프로젝트는 **MVVM 패턴**과 **이벤트 핸들러 패턴**을 상황에 따라 적절히 사용하는 하이브리드 접근 방식을 보여줍니다:

- **모듈 간 통신**: MVVM (EventBus) - 느슨한 결합, 확장성
- **UI Components**: 이벤트 핸들러 - 간단함, 직접 참조

각 패턴의 장점을 활용하여, 복잡한 시스템에서도 유지보수 가능한 코드를 작성할 수 있습니다.

---

**작성일:** 2026-02-03  
**버전:** 1.0
