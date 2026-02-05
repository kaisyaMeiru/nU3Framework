  # 환자 목록 화면 기본 사용 가이드 - 자동 문서

  ## 개요

  환자 목록 화면을 구현하려면 기본 컨트롤과 EventBus를 사용하여 통신합니다.

  ## 주요 컨트롤

  ### 1. PatientListControl (환자 목록)
  - **ProgramID**: `EMR_PATIENT_LIST_001`
  - **역할**: 이벤트 발행자 (Publisher)
  - **기능**:
    - 환자 목록 표시 (DevExpress GridControl)
    - 환자 선택 시 이벤트 발행
    - 검색 및 정렬 기능
    - 더블클릭으로 상세 화면 이동

  ### 2. PatientDetailControl (환자 상세)
  - **ProgramID**: `EMR_PATIENT_DETAIL_001`
  - **역할**: 이벤트 구독자 (Subscriber)
  - **기능**:
    - 환자 기본 정보 표시
    - 진료 처리 화면 표시
    - 이벤트 로그 표시
    - 다른 화면으로 환자 전달 가능

  ### 3. SampleWorkControl (시스템 시범 화면)
  - **ProgramID**: `EMR_SAMPLE_001`
  - **역할**: 이벤트 발행자 + 테스트
  - **기능**:
    - 환자 목록 이벤트 발행
    - 이벤트 로그 표시
    - 테스트 이벤트 버튼

  ## 이벤트 흐름

  ```
  환자 목록 화면 ────┬── (이벤트 발행) ───┬── EventAggregator (EventBus) ───┬───┐
  │                   │                  │                        │        │
  │                   │                  │                        │        │
  │                   │                  │                        │        │
  │                   │                  │                        │        │
  └─ PatientListControl (환자 목록) ──┘
       (이벤트 발행자)         │
                             │
                             │  이동 요청
                             │
                     ───┬─────┘
                     │
                     │
                     │
                     │
  └────────────────────────────────────────────────────────────────────┐
  │           EventAggregator                      │
  │           (EventBus)                           │
  └───────────────────────────────────────────────────────────────────┘
                     │            │
                     │            │
                     │            │
                     │            │
  ┌───────────────────┴───┬───────────────────┬──────────────────┐
  │ PatientDetail  │  │  SampleWork  │  │
  │ Control          │  │  Control          │
  │ (발행자)       │  │  (발행자)       │
  └───────────────────┴───┴───────────────────┴──────────────────┘
  ```

  ## 메뉴 설정

  ### 1. 기본 메뉴

  Deployer에서 다음과 같이 메뉴를 설정합니다:

  ```
  메뉴: EMR > 환자 목록
  ProgramID: EMR_PATIENT_LIST_001

  메뉴: EMR > 환자 상세
  ProgramID: EMR_PATIENT_DETAIL_001

  메뉴: EMR > 시범 화면
  ProgramID: EMR_SAMPLE_001
  ```

  ### 2. 테스트 처리

  #### 테스트 처리 1: 환자 목록에서 상세 화면으로 이동

  1. **환자 목록**에서 선택
  2. **환자 상세**로 전환
  3. **환자 목록**을 누른 환자 탭으로 전환
  4. 선택된 환자 컨트롤 전달

  ```
  [환자 목록]
      └─ 환자 컨트롤 (예: 김철수)
          │
  [EventBus]
      └─ PatientSelectedEvent
          │
  [환자 상세]
      └─ OnPatientSelected() 호출
          └─ 선택된 환자 컨트롤 표시
  ```

  #### 테스트 처리 2: MainShell 테스트 환자 전환

  1. **환자 상세** 메뉴 선택
  2. **메뉴** > **환자 목록 테스트** 클릭
  3. 메뉴에서 테스트 환자 전환 가능

  #### 테스트 처리 3: 환자 전환 전체 테스트

  1. **환자 목록** 화면 실행
  2. **환자 상세** 화면 실행
  3. **시범 화면** 실행
  4. **환자 목록**에서 환자 컨트롤 선택
  5. 메뉴에서 환자 전환 가능

  ```
  [환자 목록]으로 이벤트 발행
      │
  [EventBus] 로딩
      │
      └─ 메뉴에서 [환자 상세]로 전환
      └─ 메뉴에서 [시범 화면]으로 전환
      └─ 메뉴에서 [다른 화면]으로 전환
  ```

  ## 사용 예시

  ### 기본 구현 (PatientListControl)

  ```csharp
  // 환자 목록 컨트롤
  private void GridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
  {
      var selectedPatient = GetSelectedPatient();
      if (selectedPatient != null)
      {
          // 1. 화면 컨텍스트를 새로 설정
          var newContext = Context.Clone();
          newContext.CurrentPatient = selectedPatient;
          UpdateContext(newContext);

          // 2. 다른 화면으로 이벤트 발행
          EventBus?.GetEvent<PatientSelectedEvent>()
              .Publish(new PatientSelectedEventPayload
              {
                  Patient = selectedPatient,
                  Source = ProgramID // "EMR_PATIENT_LIST_001"
              });
      }
  }
  ```

  ### 기본 구현 (PatientDetailControl)

  ```csharp
  // 화면 활성화 시 이벤트 구독
  protected override void OnScreenActivated()
  {
      base.OnScreenActivated();

      EventBus?.GetEvent<PatientSelectedEvent>()
          .Subscribe(OnPatientSelected);
  }

  // 이벤트 처리
  private void OnPatientSelected(object payload)
  {
      if (payload is not PatientSelectedEventPayload evt)
          return;

      // 자신의 화면에서 발행된 이벤트는 무시
      if (evt.Source == ProgramID)
          return;

      // 환자 상세 표시
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

  #### 2. 환자 전환 발행
  ```csharp
  private void PublishPatientSelected(PatientInfoDto patient)
  {
      EventBus?.GetEvent<PatientSelectedEvent>()
          .Publish(new PatientSelectedEventPayload
          {
              Patient = patient,
              Source = ProgramID
          });

      LogInfo($"환자 PatientSelectedEvent 발행: {patient.PatientName}");
  }
  ```

  #### 3. 더블클릭으로 화면 이동
  ```csharp
  private void GridView_DoubleClick(object sender, EventArgs e)
  {
      var selectedPatient = GetSelectedPatient();
      if (selectedPatient != null)
      {
          // 바로가기 요청
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

  #### 1. 이벤트 구독
  ```csharp
  private void SubscribeToEvents()
  {
      // 환자 목록 이벤트
      EventBus?.GetEvent<PatientSelectedEvent>()
          .Subscribe(OnPatientSelected);

      // 환자 상세 목록 이벤트
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

  #### 3. 처리
  ```csharp
  private void OnPatientSelected(object payload)
  {
      if (payload is not PatientSelectedEventPayload evt)
          return;

      AddEventLog($"이벤트 발행한 '{evt.Source}'에서 PatientSelectedEvent 수신");
      AddEventLog($"   환자: {evt.Patient.PatientName}");

      // UI 처리
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

  **용도**: 사용자가 선택되면 다른 화면으로 알림

  ### PatientUpdatedEvent
  ```csharp
  public class PatientUpdatedEventPayload
  {
      public PatientInfoDto Patient { get; set; }   // 업데이트된 환자
      public string Source { get; set; }
      public string UpdatedBy { get; set; }         // 업데이트 발행자
  }
  ```

  **용도**: 환자 상세에서 업데이트되면 다른 화면으로 알림

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
      public string TargetScreenId { get; set; }    // 대상 화면 ID
      public WorkContext Context { get; set; }      // 화면 컨텍스트
      public string Source { get; set; }
  }
  ```

  **용도**: 다른 화면으로 이동 요청

  ## 팁

  ### 체크 Source
  ```csharp
  // Good - 자신의 화면에서 발행된 이벤트는 무시
  if (evt.Source == ProgramID)
      return;

  // Bad - Source 확인 안 하면 이벤트 처리
  ```

  ### Null 체크
  ```csharp
  // Good
  if (payload is not PatientSelectedEventPayload evt)
      return;

  if (evt.Patient == null)
      return;

  // Bad
  var evt = (PatientSelectedEventPayload)payload; // NullReferenceException 발생
  ```

  ### UI 스레드 체크
  ```csharp
  // Good - UI 스레드에서 처리
  if (InvokeRequired)
  {
      Invoke(new Action(() => DisplayPatientInfo(patient)));
  }
  else
  {
      DisplayPatientInfo(patient);
  }
  ```

  ### 로그 활용
  ```csharp
  // Good - 적절한 로그
  AddEventLog($"Event received from {evt.Source}");
  LogInfo($"Patient selected: {patient.PatientName}");
  LogAudit(AuditAction.Read, "Patient", patient.PatientId);
  ```

  ## 실수 방지

  ### 1. EventBus null 체크

  **문제**: EventBus가 null인 경우

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

  ### 2. 이벤트 중복 처리

  **문제**: 자신의 화면에서 발행된 이벤트가 또다시 처리

  **해결**:
  ```csharp
  private void OnPatientSelected(object payload)
  {
      if (payload is not PatientSelectedEventPayload evt)
          return;

      // 자신의 화면에서 발행된 이벤트는 무시
      if (evt.Source == ProgramID)
          return;

      // 처리
  }
  ```

  ### 3. UI 스레드에서 처리

  **문제**: UI 스레드에서 처리하지 않음

  **해결**:
  ```csharp
  private void OnPatientSelected(object payload)
  {
      // ... 이벤트 처리 ...

      // UI 스레드에서 처리
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

  ## 추가 이벤트 예시

  ### VitalSign 업데이트 이벤트 추가

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

  ### 다른 화면으로 전환 가능

  ```csharp
  // 어떤 화면에서든지 실행 가능한 컨트롤
  public class MyCustomControl : BaseWorkControl
  {
      protected override void OnScreenActivated()
      {
          base.OnScreenActivated();

          // 환자 목록 이벤트 구독
          EventBus?.GetEvent<PatientSelectedEvent>()
              .Subscribe(OnPatientSelected);
      }

      private void OnPatientSelected(object payload)
      {
          if (payload is not PatientSelectedEventPayload evt)
              return;

          if (evt.Source == ProgramID)
              return;

          // 필요한 처리
          ProcessPatientSelection(evt.Patient);
      }
  }
  ```

  ## 업데이트 팁

  ### 1. 이벤트 데이터
  ```csharp
  // 데이터베이스 조회 없이 이벤트 데이터로 처리
  private void OnPatientSelected(object payload)
  {
      if (payload is not PatientSelectedEventPayload evt)
          return; // 형식 확인

      if (evt.Source == ProgramID)
          return; // 소스 확인

      // 필요한 정보로 처리
      if (!IsNeedToProcess(evt))
          return;

      // 처리
      Process(evt);
  }
  ```

  ### 2. 정보의 크기와 ID 확인
  ```csharp
  // Bad - 큰 데이터를 전달
  public class PatientDetailDataPayload
  {
      public List<ExamResultDto> AllExamResults { get; set; } // 모든 검사 결과
      public List<VitalSignDto> AllVitalSigns { get; set; }
  }

  // Good - ID로 조회 가능하게
  public class PatientDataChangedPayload
  {
      public string PatientId { get; set; }
      public string ChangedType { get; set; } // "ExamResult", "VitalSign" 등
  }
  ```

  ## 라이선스 정보

  (c) 2024 nU3 Framework
