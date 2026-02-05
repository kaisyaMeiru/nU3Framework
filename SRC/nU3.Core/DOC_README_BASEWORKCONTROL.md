  # BaseWorkControl - 기본 작업 컨트롤 사용 가이드

  ## 개요

  BaseWorkControl은 모든 작업 화면 컨트롤의 기본이 되며, 표준 기능과 리소스 관리를 제공합니다.

  ## 주요 기능

  ### 1. 작업 컨텍스트 (WorkContext)

  화면 간 공유되는 컨텍스트

  #### 컨텍스트에 포함된 정보
  - **CurrentPatient**: 현재 선택된 환자 정보
  - **CurrentExam**: 현재 선택된 검사 정보
  - **CurrentAppointment**: 현재 선택된 예약 정보
  - **CurrentUser**: 현재 로그인한 사용자 정보
  - **Permissions**: 각 버튼/기능의 권한 정보
  - **Parameters**: 화면으로 전달되는 매개변수
  - **AdditionalData**: 추가 정보를 저장하는 공간

  ### 2. 권한 제어 (ModulePermissions)

  각 버튼/기능별 권한

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

  모든 화면 컨트롤에 대한 리소스 관리

  #### 리소스 관리 메서드
  - **RegisterDisposable()**: Disposable 리소스 자동 관리
  - **ReleaseResources()**: 화면 종료 시 리소스 해제 (BeforeClose 시 호출)
  - **OnReleaseResources()**: ReleaseResources 호출 후 수행하는 로직 (사용자 정의)
  - **CancellationToken**: 비동기 작업 취소 토큰

  ### 4. 라이프사이클 메서드

  화면 활성화/비활성화 제어

  #### 라이프사이클 메서드
  - `OnScreenActivated()`: 화면 활성화 시
  - `OnScreenDeactivated()`: 화면 비활성화 시
  - `OnBeforeClose()`: 닫기 전

  ## 사용 예시

  ### 1. 기본 사용

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

      // 환자 정보 확인
      if (newContext.CurrentPatient != null)
      {
          var patientId = newContext.CurrentPatient.PatientId;
          var patientName = newContext.CurrentPatient.PatientName;
          LoadPatientData(patientId);
      }

      // 사용자 정보 확인
      if (newContext.CurrentUser != null)
      {
          var userId = newContext.CurrentUser.UserId;
          var userName = newContext.CurrentUser.UserName;
      }

      // 매개변수 확인
      var mode = newContext.GetParameter<string>("Mode", "View");
      var recordId = newContext.GetParameter<int>("RecordId", 0);
  }

  protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
  {
      base.OnContextChanged(oldContext, newContext);

      // 컨텍스트 변경 시 UI 갱신
      UpdateUI();
  }
  ```

  ### 3. 권한 확인

  ```csharp
  private void BtnSave_Click(object sender, EventArgs e)
  {
      // 수정 권한 확인
      if (!CanUpdate)
      {
          MessageBox.Show("수정 권한이 없습니다.");
          return;
      }

      SaveData();
  }

  private void InitializeUI()
  {
      // 각 버튼의 권한에 따라 활성화
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
          // 타이머 등록 (자동 관리)
          _refreshTimer = new Timer();
          _refreshTimer.Interval = 5000;
          _refreshTimer.Tick += RefreshTimer_Tick;
          RegisterDisposable(_refreshTimer); // 자동 리소스 관리

          // 데이터베이스 연결 (테이블 등록 필요)
          _connection = new SqlConnection(connectionString);
          RegisterDisposable(_connection); // 자동 리소스 관리
      }

      protected override void OnReleaseResources()
      {
          base.OnReleaseResources();

          // 취소 토큰 정리
          _searchCancellation?.Cancel();
          _searchCancellation?.Dispose();
          _searchCancellation = null;

          // 타이머 중지
          _refreshTimer?.Stop();

          // 이벤트 핸들러 정리
          if (_refreshTimer != null)
              _refreshTimer.Tick -= RefreshTimer_Tick;

          LogInfo("Resources released");
      }
  }
  ```

  ### 5. 비동기 작업의 취소 지원

  ```csharp
  private async void BtnSearch_Click(object sender, EventArgs e)
  {
      try
      {
          // CancellationToken 전달 (자동 리소스 관리)
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
      // 작업 취소 지원 비동기 작업
      var results = await _patientService.SearchAsync(keyword, cancellationToken);
      return results;
  }
  ```

  ### 6. 라이프사이클 메서드

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
              "저장하지 않은 데이터가 있습니다. 저장하시겠습니까?",
              "확인",
              MessageBoxButtons.YesNo);

          return result == DialogResult.Yes;
      }

      return true;
  }
  ```

  ## 다양한 인터페이스 사용

  MainShellForm에서 다양한 인터페이스를 사용합니다:

  ```csharp
  // 컨텍스트 초기화
  if (content is IWorkContextProvider contextProvider)
  {
      var context = CreateWorkContext();
      contextProvider.InitializeContext(context);
  }

  // 라이프사이클 메서드
  if (control is ILifecycleAware lifecycleAware)
  {
      lifecycleAware.OnActivated();
  }

  // 리소스 관리
  if (control is IResourceManager resourceManager)
  {
      resourceManager.ReleaseResources();
  }
  ```

  ## 표준 사용

  ### 1. 권한 확인 사용

  ```csharp
  private void PerformSensitiveOperation()
  {
      if (!CanUpdate)
      {
          MessageBox.Show("수정 권한이 없습니다.");
          LogAudit(AuditAction.Update, "Entity", "123", "Permission denied");
          return;
      }

      DoUpdate();
      LogAudit(AuditAction.Update, "Entity", "123", "Updated successfully");
  }
  ```

  ### 2. 리소스 안전 사용

  ```csharp
  public MyWorkControl()
  {
      // IDisposable 리소스 등록
      _timer = new Timer();
      RegisterDisposable(_timer);

      _connection = new SqlConnection();
      RegisterDisposable(_connection);

      _client = new HttpClient();
      RegisterDisposable(_client);
  }
  ```

  ### 3. 비동기 작업의 취소 지원 사용

  ```csharp
  private async Task LoadDataAsync()
  {
      try
      {
          // CancellationToken 전달
          var data = await _service.GetDataAsync(CancellationToken);
          DisplayData(data);
      }
      catch (OperationCanceledException)
      {
          // 작업 취소
          LogInfo("Operation cancelled");
      }
  }
  ```

  ### 4. 로그 사용

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

  ## 팁

  1. **컨텍스트 안전 처리**: Clone()을 사용하여 컨트롤 간 전달 안전
  2. **권한 확인 시작**: 모든 중요한 작업 전 권한 확인 필수
  3. **리소스 관리**: RegisterDisposable() 또는 OnReleaseResources() 사용
  4. **비동기 작업**: CancellationToken 필수 전달
  5. **로그**: 중요한 작업 후 필수 로그 기록

  ## 라이선스 정보

  (c) 2024 nU3 Framework
