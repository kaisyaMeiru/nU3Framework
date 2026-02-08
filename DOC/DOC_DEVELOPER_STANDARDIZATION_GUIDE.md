# nU3.Framework 개발자 표준화 가이드
**전자정부 프레임워크 기반 개발 생산성 향상 전략**

> 작성일: 2026-02-07
> 작성자: Architecture Team
> 버전: 1.0
> 참조: 전자정부 표준 프레임워크 (eGovFrame)

---

## 📋 목차

1. [개요 및 목표](#1-개요-및-목표)
2. [아키텍처 표준화](#2-아키텍처-표준화)
3. [UI 컴포넌트 표준화](#3-ui-컴포넌트-표준화)
4. [DTO 표준화 및 관리](#4-dto-표준화-및-관리)
5. [서버 통신 표준화](#5-서버-통신-표준화)
6. [개발 템플릿 및 도구](#6-개발-템플릿-및-도구)
7. [개발 가이드 및 표준](#7-개발-가이드-및-표준)

---

# 1. 개요 및 목표

## 1.1 문제 정의

### 현재 문제점
1. **DTO 정의의 중복**: 개발자마다 업무별로 DTO 정의 → 유지보수 어려움
2. **UI 컴포넌트 불일치**: 개발자마다 그리드/폼 스타일 다름 → 사용자 경험 저하
3. **서버 통신 비표준화**: HTTP 호출 방식 상이 → 통합 테스트 어려움
4. **트랜잭션 처리 부재**: 클라이언트에서 개별 API 호출 → 데이터 무결성 위험

### 목표
- **개발 생산성 50% 향상**: 표준화된 컴포넌트와 템플릿 사용
- **코드 품질 향상**: 일관된 아키텍처와 코딩 표준
- **유지보수성 강화**: 중복 최소화, 표준화된 인터페이스
- **데이터 무결성 보장**: 서버 사이드 트랜잭션 처리

## 1.2 전자정부 프레임워크 참조

### eGovFrame 특징
```
┌─────────────────────────────────────────────────────────────┐
│  전자정부 표준 프레임워크 (eGovFrame) 구조                    │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────────────────────────────────────────────┐    │
│  │  Presentation Layer (Web)                          │    │
│  │  - JSP/HTML 템플릿                                 │    │
│  │  - UI 컴포넌트 라이브러리                           │    │
│  └──────────────────────────────────────────────────────┘    │
│                           ↓                                    │
│  ┌──────────────────────────────────────────────────────┐    │
│  │  Business Layer (Service)                          │    │
│  │  - 비즈니스 로직                                     │    │
│  │  - 트랜잭션 관리 (@Transactional)                  │    │
│  └──────────────────────────────────────────────────────┘    │
│                           ↓                                    │
│  ┌──────────────────────────────────────────────────────┐    │
│  │  Persistence Layer (DAO)                           │    │
│  │  - 데이터 엑세스                                     │    │
│  │  - iBatis/MyBatis 매핑                            │    │
│  └──────────────────────────────────────────────────────┘    │
│                                                              │
│  [공통 컴포넌트]                                              │
│  - 요청/응답 DTO (BaseDto)                                  │
│  - 페이징/검색 (PageableRequest, PageableResponse)          │
│  - 결과/코드 (ResultDto, CodeDto)                           │
│  - 에러 처리 (Exception Handler)                           │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### nU3.Framework 적용 전략
| eGovFrame | nU3.Framework | 설명 |
|-----------|---------------|------|
| Web Layer | WinForms + DevExpress | WinForms UI 기반 |
| Service Layer | Service Agent Layer | HTTP 기반 서비스 통신 |
| DAO Layer | HTTP DB Access Client | REST API 기반 데이터 엑세스 |
| BaseDto | BaseRequestDto / BaseResponseDto | 공통 요청/응답 DTO |
| @Transactional | Transaction Context | 서버 사이드 트랜잭션 |

---

# 2. 아키텍처 표준화

## 2.1 3계층 아키텍처 (3-Tier Architecture)

### 전체 구조

```
┌──────────────────────────────────────────────────────────────────────────┐
│  Client Side (nU3.Client)                                             │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Presentation Layer (UI)                                     │    │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │    │
│  │  │ SearchForm  │  │ GridControl│  │ EditForm    │              │    │
│  │  │ (검색)      │  │ (목록)      │  │ (상세/수정)  │              │    │
│  │  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘              │    │
│  │         │                 │                 │                     │    │
│  └─────────┼─────────────────┼─────────────────┼─────────────────┘    │
│            ↓                 ↓                 ↓                         │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Business Layer (ViewModel/Presenter)                         │    │
│  │  - 데이터 바인딩                                               │    │
│  │  - UI 로직                                                    │    │
│  │  - 사용자 입력 검증                                             │    │
│  └────────────────────────────────────────────────────────────────┘    │
│            ↓                                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Service Agent Layer (통신)                                   │    │
│  │  - 서비스 인터페이스 정의                                      │    │
│  │  - HTTP 요청/응답                                             │    │
│  │  - DTO 변환 (DTO ↔ Entity)                                    │    │
│  │  - 에러 처리                                                   │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
                                 ↓
                    HTTPS / JSON / gRPC
                                 ↓
┌──────────────────────────────────────────────────────────────────────────┐
│  Server Side (nU3.Server)                                              │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  API Layer (Controller)                                       │    │
│  │  - 요청 수신                                                   │    │
│  │  - DTO 검증                                                   │    │
│  │  - 서비스 호출                                                 │    │
│  │  - 응답 변환 (Entity → DTO)                                   │    │
│  └────────────────────────────────────────────────────────────────┘    │
│            ↓                                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Service Layer (Business Logic)                              │    │
│  │  - 비즈니스 로직                                               │    │
│  │  - 트랜잭션 관리                                              │    │
│  │  - 도메인 로직                                                │    │
│  └────────────────────────────────────────────────────────────────┘    │
│            ↓                                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Repository Layer (Data Access)                              │    │
│  │  - Oracle/SQL Server 엑세스                                 │    │
│  │  - Entity Framework / Dapper                                  │    │
│  │  - 쿼리 최적화                                                │    │
│  └────────────────────────────────────────────────────────────────┘    │
│            ↓                                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Database (Oracle)                                             │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

## 2.2 Service Agent 패턴

### 개념
클라이언트에서 서버 통신 로직을 추상화한 패턴으로, 개발자가 HTTP 통신 코드를 직접 작성하지 않도록 함

### 인터페이스 정의
```csharp
// nU3.Core/Services/IPatientServiceAgent.cs
public interface IPatientServiceAgent
{
    Task<PagedResultDto<PatientListDto>> GetPatientsAsync(PatientSearchRequestDto request);
    Task<PatientDetailDto> GetPatientAsync(string patientId);
    Task<PatientDetailDto> CreatePatientAsync(CreatePatientRequestDto request);
    Task<PatientDetailDto> UpdatePatientAsync(UpdatePatientRequestDto request);
    Task<bool> DeletePatientAsync(string patientId);
}

// nU3.Connectivity/Services/PatientServiceAgent.cs
public class PatientServiceAgent : IPatientServiceAgent
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IAuthenticationService _authService;

    public PatientServiceAgent(
        HttpClient httpClient,
        IOptions<ServerConnectionConfig> config,
        IAuthenticationService authService)
    {
        _httpClient = httpClient;
        _baseUrl = config.Value.BaseUrl;
        _authService = authService;
    }

    public async Task<PagedResultDto<PatientListDto>> GetPatientsAsync(PatientSearchRequestDto request)
    {
        // 1. 토큰 주입
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _authService.GetAccessToken());

        // 2. HTTP 요청
        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}/api/patients/search",
            request);

        // 3. 에러 처리
        response.EnsureSuccessStatusCode();

        // 4. 응답 변환
        return await response.Content.ReadFromJsonAsync<PagedResultDto<PatientListDto>>();
    }
}
```

### DI 등록
```csharp
// nU3.Shell/Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // HTTP Client 등록
    services.AddHttpClient<IPatientServiceAgent, PatientServiceAgent>();

    // Service Agent 등록
    services.AddScoped<IPatientServiceAgent, PatientServiceAgent>();
}
```

### ViewModel에서 사용
```csharp
// nU3.Modules.EMR.CL/PatientListViewModel.cs
public class PatientListViewModel
{
    private readonly IPatientServiceAgent _serviceAgent;

    public PatientListViewModel(IPatientServiceAgent serviceAgent)
    {
        _serviceAgent = serviceAgent;
    }

    public async Task LoadDataAsync(PatientSearchRequestDto request)
    {
        var result = await _serviceAgent.GetPatientsAsync(request);

        // 데이터 바인딩
        Patients = new BindingList<PatientListDto>(result.Items);
        TotalCount = result.TotalCount;
    }
}
```

## 2.3 트랜잭션 처리

### 서버 사이드 트랜잭션

#### Service Layer에서 트랜잭션 관리
```csharp
// nU3.Server/Services/PatientService.cs
public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPatientRepository _patientRepository;
    private readonly IVisitRepository _visitRepository;

    [Transaction(TransactionScopeOption.Required)] // 트랜잭션 속성
    public async Task<PatientDetailDto> CreatePatientWithVisitAsync(
        CreatePatientRequestDto patientRequest,
        CreateVisitRequestDto visitRequest)
    {
        // 트랜잭션 시작
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. 환자 생성
            var patient = new Patient
            {
                PatientId = GeneratePatientId(),
                PatientName = patientRequest.PatientName,
                BirthDate = patientRequest.BirthDate,
                // ...
            };

            await _patientRepository.AddAsync(patient);

            // 2. 방문 등록 (환자 ID 참조)
            var visit = new Visit
            {
                PatientId = patient.PatientId,
                VisitDate = visitRequest.VisitDate,
                DepartmentCode = visitRequest.DepartmentCode,
                // ...
            };

            await _visitRepository.AddAsync(visit);

            // 3. 커밋
            await _unitOfWork.CommitAsync(transaction);

            // 4. DTO 반환
            return MapToDetailDto(patient);
        }
        catch (Exception ex)
        {
            // 롤백
            await _unitOfWork.RollbackAsync(transaction);
            throw;
        }
    }
}
```

### 클라이언트 사이드 트랜잭션 요청

#### 복합 작업 DTO
```csharp
// nU3.Models/DTOs/CreatePatientWithVisitRequestDto.cs
[TransactionalRequest] // 트랜잭션 요청 마커
public class CreatePatientWithVisitRequestDto : BaseRequestDto
{
    [Required]
    public CreatePatientRequestDto Patient { get; set; }

    [Required]
    public CreateVisitRequestDto Visit { get; set; }
}

// 사용 예
var request = new CreatePatientWithVisitRequestDto
{
    Patient = new CreatePatientRequestDto { /* ... */ },
    Visit = new CreateVisitRequestDto { /* ... */ }
};

var result = await _serviceAgent.CreatePatientWithVisitAsync(request);
// 서버에서 트랜잭션으로 처리됨
```

---

# 3. UI 컴포넌트 표준화

## 3.1 DevExpress 래핑 컨트롤

### 이미 구현된 래핑 컨트롤
```
nU3.Core.UI.Controls/
├── BaseControl/
│   ├── NuBaseControl.cs              (DevExpress XtraUserControl 래핑)
│   ├── NuBaseForm.cs                (DevExpress XtraForm 래핑)
│   └── NuBaseEditForm.cs            (편집 폼 기본 클래스)
├── Data/
│   ├── NuGridControl.cs             (DevExpress GridControl 래핑)
│   ├── NuTreeListControl.cs         (DevExpress TreeList 래핑)
│   ├── NuLookUpEdit.cs              (DevExpress LookUpEdit 래핑)
│   ├── NuCheckedComboBoxEdit.cs     (DevExpress CheckedComboBoxEdit 래핑)
│   └── NuSearchControl.cs           (공통 검색 컨트롤)
├── Input/
│   ├── NuTextEdit.cs                (DevExpress TextEdit 래핑)
│   ├── NuDateEdit.cs                (DevExpress DateEdit 래핑)
│   ├── NuCalcEdit.cs                (DevExpress CalcEdit 래핑)
│   ├── NuSpinEdit.cs               (DevExpress SpinEdit 래핑)
│   └── NuMemoEdit.cs               (DevExpress MemoEdit 래핑)
├── Buttons/
│   ├── NuButton.cs                 (DevExpress SimpleButton 래핑)
│   ├── NuSearchButton.cs           (검색 버튼)
│   ├── NuSaveButton.cs             (저장 버튼)
│   ├── NuDeleteButton.cs           (삭제 버튼)
│   └── NuCancelButton.cs           (취소 버튼)
└── Dialogs/
    ├── NuXtraMessageBox.cs         (메시지 박스 래핑)
    └── NuXtraOpenFileDialog.cs    (파일 열기 대화상자 래핑)
```

### 래핑 컨트롤 예시

#### NuGridControl (표준화된 그리드)
```csharp
// nU3.Core.UI.Controls/Data/NuGridControl.cs
public class NuGridControl : GridControl
{
    public NuGridControl()
    {
        // 기본 스타일 설정
        this.LookAndFeel.SkinName = "Office 2019 White";
        this.OptionsView.ShowGroupPanel = false;
        this.OptionsView.ShowIndicator = true;
        this.OptionsSelection.MultiSelect = false;
        this.OptionsBehavior.Editable = false;
        this.UseEmbeddedNavigator = false;

        // 번역 기능 (예정)
        // this.SetMultilingualSupport();
    }

    // 엑셀 내보내기 기능
    public void ExportToExcel(string fileName)
    {
        var options = new XlsxExportOptionsEx
        {
            ExportType = ExportType.DataAware,
            SheetName = "Data"
        };

        this.ExportToXlsx(fileName, options);
    }

    // 레이아웃 저장/복원
    public void SaveLayout(string filePath)
    {
        this.MainView.SaveLayoutToXml(filePath);
    }

    public void RestoreLayout(string filePath)
    {
        this.MainView.RestoreLayoutFromXml(filePath);
    }
}
```

#### NuSearchControl (공통 검색 컨트롤)
```csharp
// nU3.Core.UI.Controls/Data/NuSearchControl.cs
public partial class NuSearchControl : NuBaseControl
{
    public NuSearchControl()
    {
        InitializeComponent();

        // 기본 검색 조건 초기화
        InitializeSearchFields();
    }

    public event EventHandler<SearchEventArgs> Search;

    protected virtual void OnSearch(SearchEventArgs e)
    {
        Search?.Invoke(this, e);
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
        // 검색 조건 수집
        var searchCriteria = CollectSearchCriteria();

        // 이벤트 발생
        OnSearch(new SearchEventArgs(searchCriteria));
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        // 검색 조건 초기화
        ResetSearchFields();
    }

    // 하위 클래스에서 오버라이드
    protected virtual void InitializeSearchFields() { }
    protected virtual SearchCriteriaDto CollectSearchCriteria() { return new SearchCriteriaDto(); }
    protected virtual void ResetSearchFields() { }
}
```

### 사용 예시
```csharp
// nU3.Modules.EMR.CL/PatientListControl.cs
public partial class PatientListControl : NuBaseControl
{
    private readonly PatientListViewModel _viewModel;

    public PatientListControl(PatientListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        // 그리드 초기화
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // 컬럼 바인딩
        grdPatient.DataSource = _viewModel.Patients;

        // 컬럼 설정
        var view = grdPatient.MainView as GridView;
        view.Columns["PatientId"].Caption = "환자ID";
        view.Columns["PatientName"].Caption = "환자명";
        view.Columns["BirthDate"].Caption = "생년월일";
        view.Columns["Gender"].Caption = "성별";

        // 컬럼 너비 자동 조정
        view.BestFitColumns();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        var request = new PatientSearchRequestDto
        {
            PatientName = txtPatientName.Text,
            BirthDate = dteBirthDate.DateTime,
            PageNumber = 1,
            PageSize = 50
        };

        await _viewModel.LoadDataAsync(request);
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
        grdPatient.ExportToExcel("PatientList.xlsx");
    }
}
```

## 3.2 공통 컴포넌트 라이브러리

### 목록
| 컴포넌트 | 설명 | 사용 예시 |
|---------|------|----------|
| **NuSearchControl** | 공통 검색 컨트롤 | 모든 목록 화면 |
| **NuGridControl** | 표준화된 그리드 | 데이터 목록 표시 |
| **NuEditControl** | 공통 편집 폼 | 상세/수정 화면 |
| **NuTreeListControl** | 트리 리스트 | 계층형 데이터 |
| **NuLookUpEdit** | 룩업 에디트 (콤보박스) | 코드 선택 |
| **NuSearchLookup** | 검색 룩업 팝업 | 대용량 데이터 선택 |
| **NuDateRangeEdit** | 날짜 범위 에디트 | 기간 검색 |
| **NuMultiSelect** | 다중 선택 컨트롤 | 여러 항목 선택 |

---

# 4. DTO 표준화 및 관리

## 4.1 공통 베이스 DTO

### BaseRequestDto
```csharp
// nU3.Models/DTOs/Base/BaseRequestDto.cs
public abstract class BaseRequestDto
{
    /// <summary>
    /// 요청자 사용자 ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 요청자 부서 코드
    /// </summary>
    public string DeptCode { get; set; }

    /// <summary>
    /// 요청 ID (추적용)
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// 요청 일시
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 요청자 IP 주소
    /// </summary>
    public string ClientIp { get; set; }
}
```

### BaseResponseDto
```csharp
// nU3.Models/DTOs/Base/BaseResponseDto.cs
public abstract class BaseResponseDto
{
    /// <summary>
    /// 성공 여부
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 응답 코드
    /// </summary>
    public string ResponseCode { get; set; }

    /// <summary>
    /// 응답 메시지
    /// </summary>
    public string ResponseMessage { get; set; }

    /// <summary>
    /// 에러 정보 (실패 시)
    /// </summary>
    public ErrorInfoDto Error { get; set; }

    /// <summary>
    /// 응답 일시
    /// </summary>
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
}

// 에러 정보 DTO
public class ErrorInfoDto
{
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
    public Dictionary<string, string> Details { get; set; }
}
```

## 4.2 페이징 DTO

### PagedRequestDto
```csharp
// nU3.Models/DTOs/Common/PagedRequestDto.cs
public abstract class PagedRequestDto : BaseRequestDto
{
    /// <summary>
    /// 페이지 번호 (1부터 시작)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "페이지 번호는 1 이상이어야 합니다.")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 페이지 크기
    /// </summary>
    [Range(1, 100, ErrorMessage = "페이지 크기는 1~100 사이여야 합니다.")]
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// 정렬 컬럼
    /// </summary>
    public string SortColumn { get; set; }

    /// <summary>
    /// 정렬 방향 (ASC/DESC)
    /// </summary>
    public string SortDirection { get; set; } = "ASC";
}
```

### PagedResultDto
```csharp
// nU3.Models/DTOs/Common/PagedResultDto.cs
public class PagedResultDto<T> : BaseResponseDto
{
    /// <summary>
    /// 데이터 목록
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// 전체 개수
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 전체 페이지 수
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 현재 페이지 번호
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 페이지 크기
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 이전 페이지 여부
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 다음 페이지 여부
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}
```

## 4.3 검색 DTO

### 검색 조건 DTO
```csharp
// nU3.Models/DTOs/Common/SearchCriteriaDto.cs
public class SearchCriteriaDto
{
    /// <summary>
    /// 검색 키워드
    /// </summary>
    public string Keyword { get; set; }

    /// <summary>
    /// 검색 시작일
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 검색 종료일
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 추가 검색 조건
    /// </summary>
    public Dictionary<string, object> AdditionalConditions { get; set; }
        = new Dictionary<string, object>();
}
```

### 환자 검색 DTO 예시
```csharp
// nU3.Models/DTOs/Patient/PatientSearchRequestDto.cs
public class PatientSearchRequestDto : PagedRequestDto
{
    /// <summary>
    /// 환자명
    /// </summary>
    [StringLength(50)]
    public string PatientName { get; set; }

    /// <summary>
    /// 환자 ID
    /// </summary>
    [StringLength(20)]
    public string PatientId { get; set; }

    /// <summary>
    /// 생년월일
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 성별
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// 혈액형
    /// </summary>
    public BloodType? BloodType { get; set; }
}
```

## 4.4 CRUD DTO 템플릿

### CreateRequestDto
```csharp
// nU3.Models/DTOs/Common/CreateRequestDto.cs
public abstract class CreateRequestDto : BaseRequestDto
{
    /// <summary>
    /// 생성자 사용자 ID (자동 설정)
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// 생성 일시 (자동 설정)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### UpdateRequestDto
```csharp
// nU3.Models/DTOs/Common/UpdateRequestDto.cs
public abstract class UpdateRequestDto : BaseRequestDto
{
    /// <summary>
    /// 엔티티 ID
    /// </summary>
    [Required]
    public string Id { get; set; }

    /// <summary>
    /// 수정자 사용자 ID (자동 설정)
    /// </summary>
    public string UpdatedBy { get; set; }

    /// <summary>
    /// 수정 일시 (자동 설정)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 버전 (낙관적 동시성 제어)
    /// </summary>
    public int Version { get; set; }
}
```

### DeleteRequestDto
```csharp
// nU3.Models/DTOs/Common/DeleteRequestDto.cs
public class DeleteRequestDto : BaseRequestDto
{
    /// <summary>
    /// 삭제할 엔티티 ID 목록
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<string> Ids { get; set; } = new List<string>();

    /// <summary>
    /// 삭제 사유
    /// </summary>
    [StringLength(500)]
    public string Reason { get; set; }
}
```

## 4.5 DTO 관리 전략

### DTO 계층 구조
```
nU3.Models/DTOs/
├── Base/                          # 공통 베이스 DTO
│   ├── BaseRequestDto.cs
│   ├── BaseResponseDto.cs
│   └── ErrorInfoDto.cs
├── Common/                        # 공통 DTO
│   ├── PagedRequestDto.cs
│   ├── PagedResultDto.cs
│   ├── SearchCriteriaDto.cs
│   ├── CreateRequestDto.cs
│   ├── UpdateRequestDto.cs
│   └── DeleteRequestDto.cs
├── Patient/                        # 환자 관련 DTO
│   ├── PatientListDto.cs
│   ├── PatientDetailDto.cs
│   ├── PatientSearchRequestDto.cs
│   ├── CreatePatientRequestDto.cs
│   ├── UpdatePatientRequestDto.cs
│   └── DeletePatientRequestDto.cs
├── Visit/                          # 방문 관련 DTO
│   ├── VisitListDto.cs
│   ├── VisitDetailDto.cs
│   └── ...
└── ...                             # 기타 도메인 DTO
```

### DTO 명명 규칙
| DTO 타입 | 명명 규칙 | 예시 |
|---------|-----------|------|
| **목록 DTO** | `[Entity]ListDto` | `PatientListDto` |
| **상세 DTO** | `[Entity]DetailDto` | `PatientDetailDto` |
| **검색 요청** | `[Entity]SearchRequestDto` | `PatientSearchRequestDto` |
| **생성 요청** | `Create[Entity]RequestDto` | `CreatePatientRequestDto` |
| **수정 요청** | `Update[Entity]RequestDto` | `UpdatePatientRequestDto` |
| **삭제 요청** | `Delete[Entity]RequestDto` | `DeletePatientRequestDto` |
| **결과 DTO** | `[Action]ResultDto` | `SavePatientResultDto` |

---

# 5. 서버 통신 표준화

## 5.1 RESTful API 표준

### API URL 표준
| 작업 | HTTP Method | URL | 설명 |
|------|-------------|-----|------|
| **목록 조회** | GET | `/api/{resource}` | 전체 목록 조회 |
| **검색** | POST | `/api/{resource}/search` | 검색 조건 포함 |
| **상세 조회** | GET | `/api/{resource}/{id}` | 단일 항목 조회 |
| **생성** | POST | `/api/{resource}` | 신규 생성 |
| **수정** | PUT | `/api/{resource}/{id}` | 전체 수정 |
| **부분 수정** | PATCH | `/api/{resource}/{id}` | 부분 수정 |
| **삭제** | DELETE | `/api/{resource}/{id}` | 단일 삭제 |
| **대량 삭제** | POST | `/api/{resource}/delete` | 여러 항목 삭제 |

### 예시
```http
# 환자 검색 (POST - 복잡한 검색 조건)
POST /api/patients/search
Content-Type: application/json
Authorization: Bearer {token}

{
  "patientName": "홍길동",
  "birthDate": "1980-01-01",
  "gender": 1,
  "pageNumber": 1,
  "pageSize": 50
}

# 환자 상세 조회 (GET)
GET /api/patients/P001
Authorization: Bearer {token}

# 환자 생성 (POST)
POST /api/patients
Content-Type: application/json
Authorization: Bearer {token}

{
  "patientName": "홍길동",
  "birthDate": "1980-01-01",
  "gender": 1,
  "bloodType": 1
}

# 환자 수정 (PUT)
PUT /api/patients/P001
Content-Type: application/json
Authorization: Bearer {token}

{
  "id": "P001",
  "patientName": "홍길순",
  "version": 1
}

# 환자 삭제 (DELETE)
DELETE /api/patients/P001
Authorization: Bearer {token}
```

## 5.2 CRUD 템플릿

### CRUD Service Agent
```csharp
// nU3.Core/Services/Base/BaseCrudServiceAgent.cs
public abstract class BaseCrudServiceAgent<TListDto, TDetailDto, TSearchRequest, TCreateRequest, TUpdateRequest>
    where TListDto : class
    where TDetailDto : class
    where TSearchRequest : PagedRequestDto
    where TCreateRequest : CreateRequestDto
    where TUpdateRequest : UpdateRequestDto
{
    private readonly HttpClient _httpClient;
    private readonly string _resourceUrl;

    protected BaseCrudServiceAgent(HttpClient httpClient, string resourceUrl)
    {
        _httpClient = httpClient;
        _resourceUrl = resourceUrl;
    }

    public virtual async Task<PagedResultDto<TListDto>> SearchAsync(TSearchRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_resourceUrl}/search", request);
        return await response.Content.ReadFromJsonAsync<PagedResultDto<TListDto>>();
    }

    public virtual async Task<TDetailDto> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"{_resourceUrl}/{id}");
        return await response.Content.ReadFromJsonAsync<TDetailDto>();
    }

    public virtual async Task<TDetailDto> CreateAsync(TCreateRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_resourceUrl, request);
        return await response.Content.ReadFromJsonAsync<TDetailDto>();
    }

    public virtual async Task<TDetailDto> UpdateAsync(TUpdateRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_resourceUrl}/{request.Id}", request);
        return await response.Content.ReadFromJsonAsync<TDetailDto>();
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"{_resourceUrl}/{id}");
        return response.IsSuccessStatusCode;
    }

    public virtual async Task<bool> DeleteManyAsync(DeleteRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_resourceUrl}/delete", request);
        return response.IsSuccessStatusCode;
    }
}
```

### 구현 예시
```csharp
// nU3.Connectivity/Services/PatientServiceAgent.cs
public class PatientServiceAgent : BaseCrudServiceAgent<
    PatientListDto,
    PatientDetailDto,
    PatientSearchRequestDto,
    CreatePatientRequestDto,
    UpdatePatientRequestDto>
{
    public PatientServiceAgent(HttpClient httpClient, IOptions<ServerConnectionConfig> config)
        : base(httpClient, $"{config.Value.BaseUrl}/api/patients")
    {
    }

    // 추가 비즈니스 로직이 필요한 경우 오버라이드
    public async Task<PatientDetailDto> CreateWithVisitAsync(
        CreatePatientRequestDto patientRequest,
        CreateVisitRequestDto visitRequest)
    {
        var request = new CreatePatientWithVisitRequestDto
        {
            Patient = patientRequest,
            Visit = visitRequest
        };

        var response = await _httpClient.PostAsJsonAsync($"{_resourceUrl}/with-visit", request);
        return await response.Content.ReadFromJsonAsync<PatientDetailDto>();
    }
}
```

## 5.3 에러 처리 표준

### 에러 코드 정의
```csharp
// nU3.Core/Common/ErrorCode.cs
public static class ErrorCode
{
    // 성공
    public const string SUCCESS = "0000";
    public const string CREATED = "0001";
    public const string UPDATED = "0002";
    public const string DELETED = "0003";

    // 클라이언트 에러 (4xx)
    public const string BAD_REQUEST = "4000";
    public const string UNAUTHORIZED = "4001";
    public const string FORBIDDEN = "4003";
    public const string NOT_FOUND = "4004";
    public const string VALIDATION_ERROR = "4005";
    public const string DUPLICATE_KEY = "4006";
    public const string CONFLICT = "4009";

    // 서버 에러 (5xx)
    public const string INTERNAL_SERVER_ERROR = "5000";
    public const string DATABASE_ERROR = "5001";
    public const string NETWORK_ERROR = "5002";
    public const string EXTERNAL_SERVICE_ERROR = "5003";

    // 비즈니스 에러
    public const string PATIENT_NOT_FOUND = "6001";
    public const string VISIT_ALREADY_EXISTS = "6002";
    public const string DRUG_INTERACTION = "6003";
    public const string ALLERGY_ALERT = "6004";
}
```

### 에러 핸들러
```csharp
// nU3.Connectivity/Handlers/ApiResponseHandler.cs
public class ApiResponseHandler
{
    public static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<T>(content);
        }
        else
        {
            var errorResponse = JsonSerializer.DeserializeObject<BaseResponseDto>(content);

            throw new ApiException
            {
                ErrorCode = errorResponse.ResponseCode,
                ErrorMessage = errorResponse.ResponseMessage,
                ErrorDetails = errorResponse.Error
            };
        }
    }
}

// 사용 예
try
{
    var result = await _serviceAgent.GetPatientAsync(patientId);
}
catch (ApiException ex)
{
    NuXtraMessageBox.ShowError(ex.ErrorMessage);
}
```

---

# 6. 개발 템플릿 및 도구

## 6.1 Visual Studio 템플릿

### Item Templates

#### 화면 템플릿 (List 화면)
```xml
<!-- PatientListControl.zip (Visual Studio Item Template) -->
<VSTemplate Version="3.0.0" xmlns="http://schemas.microsoft.com/developer/vstemplate/2005" Type="Item">
  <TemplateData>
    <DefaultName>PatientListControl.cs</DefaultName>
    <Name>Patient List Control</Name>
    <Description>환자 목록 화면 템플릿</Description>
    <ProjectType>CSharp</ProjectType>
    <SortOrder>1000</SortOrder>
    <Icon>__PreviewImage.ico</Icon>
  </TemplateData>
  <TemplateContent>
    <ProjectItem SubType="UserControl" TargetFileName="$fileinputname$.cs" ReplaceParameters="true">
      PatientListControl.cs
    </ProjectItem>
    <ProjectItem SubType="Designer" TargetFileName="$fileinputname$.Designer.cs" ReplaceParameters="true">
      PatientListControl.Designer.cs
    </ProjectItem>
    <ProjectItem TargetFileName="$fileinputname$.resx" ReplaceParameters="true">
      PatientListControl.resx
    </ProjectItem>
  </TemplateContent>
  <Parameters>
    <Parameter Name="$namespace$" Type="Text" Default="nU3.Modules.EMR.CL" />
    <Parameter Name="$entityname$" Type="Text" Default="Patient" />
  </Parameters>
</VSTemplate>
```

### Project Templates

#### 모듈 프로젝트 템플릿
```bash
# nU3.Module.Template.nupkg

# 템플릿 구조
nU3.Module.Template/
├── nU3.Module.Template.csproj
├── Controllers/
│   └── $moduleid$Controller.cs
├── Views/
│   ├── $entity$ListView.cs
│   └── $entity$EditView.cs
├── ViewModels/
│   ├── $entity$ListViewModel.cs
│   └── $entity$EditViewModel.cs
└── DTOs/
    └── $entity$Dtos.cs
```

## 6.2 코드 생성기 (Scaffold)

### DTO 생성기
```csharp
// nU3.Tools.DtoGenerator/DtoGenerator.cs
public class DtoGenerator
{
    public void GenerateDtos(string entityName, List<PropertyDefinition> properties)
    {
        // 1. List DTO 생성
        var listDto = GenerateListDto(entityName, properties);
        File.WriteAllText($"{entityName}ListDto.cs", listDto);

        // 2. Detail DTO 생성
        var detailDto = GenerateDetailDto(entityName, properties);
        File.WriteAllText($"{entityName}DetailDto.cs", detailDto);

        // 3. Search Request DTO 생성
        var searchRequestDto = GenerateSearchRequestDto(entityName, properties);
        File.WriteAllText($"{entityName}SearchRequestDto.cs", searchRequestDto);

        // 4. Create Request DTO 생성
        var createRequestDto = GenerateCreateRequestDto(entityName, properties);
        File.WriteAllText($"Create{entityName}RequestDto.cs", createRequestDto);

        // 5. Update Request DTO 생성
        var updateRequestDto = GenerateUpdateRequestDto(entityName, properties);
        File.WriteAllText($"Update{entityName}RequestDto.cs", updateRequestDto);
    }
}

// 사용 예
var generator = new DtoGenerator();
generator.GenerateDtos("Patient", new List<PropertyDefinition>
{
    new PropertyDefinition("PatientId", typeof(string), true),
    new PropertyDefinition("PatientName", typeof(string), false),
    new PropertyDefinition("BirthDate", typeof(DateTime), false),
    new PropertyDefinition("Gender", typeof(int), false)
});
```

### Service Agent 생성기
```csharp
// nU3.Tools.ServiceAgentGenerator/ServiceAgentGenerator.cs
public class ServiceAgentGenerator
{
    public void GenerateServiceAgent(string entityName)
    {
        var template = @"
using nU3.Core.Services;
using nU3.Models.DTOs.@entity@;

namespace nU3.Connectivity.Services
{
    public class @entity@ServiceAgent : BaseCrudServiceAgent<
        @entity@ListDto,
        @entity@DetailDto,
        @entity@SearchRequestDto,
        Create@entity@RequestDto,
        Update@entity@RequestDto>
    {
        public @entity@ServiceAgent(
            HttpClient httpClient,
            IOptions<ServerConnectionConfig> config)
            : base(httpClient, $""{config.Value.BaseUrl}/api/@resource@"")
        {
        }
    }
}";

        var code = template
            .Replace("@entity@", entityName)
            .Replace("@resource@", entityName.ToLower());

        File.WriteAllText($"{entityName}ServiceAgent.cs", code);
    }
}
```

## 6.3 데이터베이스 툴

### 스키마 스캔 및 DTO 생성
```sql
-- Oracle DB 스크립트

-- 환자 테이블 (T_PATIENT)
CREATE TABLE T_PATIENT (
    PATIENT_ID      VARCHAR2(20) PRIMARY KEY,
    PATIENT_NAME   VARCHAR2(100) NOT NULL,
    BIRTH_DATE     DATE,
    GENDER         NUMBER(1),
    BLOOD_TYPE     NUMBER(1),
    CREATED_BY     VARCHAR2(20),
    CREATED_AT     DATE DEFAULT SYSDATE,
    UPDATED_BY     VARCHAR2(20),
    UPDATED_AT     DATE,
    VERSION        NUMBER(10) DEFAULT 0
);

-- 방문 테이블 (T_VISIT)
CREATE TABLE T_VISIT (
    VISIT_ID       VARCHAR2(20) PRIMARY KEY,
    PATIENT_ID     VARCHAR2(20) NOT NULL,
    VISIT_DATE     DATE NOT NULL,
    DEPT_CODE      VARCHAR2(10),
    DOCTOR_ID      VARCHAR2(20),
    STATUS         VARCHAR2(10),
    CREATED_BY     VARCHAR2(20),
    CREATED_AT     DATE DEFAULT SYSDATE,
    CONSTRAINT FK_VISIT_PATIENT FOREIGN KEY (PATIENT_ID)
        REFERENCES T_PATIENT(PATIENT_ID)
);
```

### DTO 자동 생성 도구
```bash
# nU3.Tools.DtoGenerator/Program.cs
# 명령행 도구

dotnet nU3.Tools.DtoGenerator.dll \
  --connection-string "Data Source=ORACLE;User Id=USER;Password=PASS" \
  --table-name "T_PATIENT" \
  --namespace "nU3.Models.DTOs.Patient" \
  --output-dir "./nU3.Models/DTOs/Patient"

# 자동 생성되는 파일
# - PatientListDto.cs
# - PatientDetailDto.cs
# - PatientSearchRequestDto.cs
# - CreatePatientRequestDto.cs
# - UpdatePatientRequestDto.cs
```

---

# 7. 개발 가이드 및 표준

## 7.1 개발 표준 가이드

### 코드 스타일
```csharp
// 1. 네임스페이스 순서
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using nU3.Core;
using nU3.Core.Services;
using nU3.Models.DTOs.Patient;

// 2. 클래스 순서
public class PatientListControl : NuBaseControl
{
    // 1. 필드 (Private)
    private readonly PatientListViewModel _viewModel;
    private IDisposable _subscription;

    // 2. 속성 (Public)
    public string ScreenId => "PATIENT_LIST_001";

    // 3. 생성자
    public PatientListControl(PatientListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }

    // 4. 이벤트 핸들러
    private void OnLoad(object sender, EventArgs e)
    {
        // 초기화 로직
    }

    // 5. 공용 메서드 (Public)
    public async Task RefreshAsync()
    {
        await LoadDataAsync();
    }

    // 6. 비공용 메서드 (Private)
    private async Task LoadDataAsync()
    {
        // 데이터 로드 로직
    }

    // 7. 리소스 정리
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscription?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

### 네이밍 규칙
| 요소 | 규칙 | 예시 |
|------|------|------|
| **클래스** | PascalCase | `PatientListControl` |
| **인터페이스** | I-prefixed | `IPatientServiceAgent` |
| **DTO** | **Dto** suffix | `PatientListDto` |
| **ViewModel** | **ViewModel** suffix | `PatientListViewModel` |
| **메서드** | PascalCase, 동사로 시작 | `LoadDataAsync()`, `GetPatientAsync()` |
| **필드** | _camelCase | `_viewModel`, `_serviceAgent` |
| **속성** | PascalCase | `PatientName`, `TotalCount` |
| **이벤트** | **Event** suffix | `PatientSelectedEvent` |
| **이벤트 핸들러** | On[EventName] | `OnPatientSelected()` |

## 7.2 화면 개발 절차

### 1단계: DTO 정의 (서버/클라이언트 공유)
```csharp
// nU3.Models/DTOs/Patient/PatientListDto.cs
public class PatientListDto
{
    public string PatientId { get; set; }
    public string PatientName { get; set; }
    public DateTime BirthDate { get; set; }
    public int Gender { get; set; }
    public string GenderName => Gender == 1 ? "남성" : "여성";
    public int BloodType { get; set; }
}
```

### 2단계: Service Agent 생성
```csharp
// nU3.Connectivity/Services/PatientServiceAgent.cs
public class PatientServiceAgent : BaseCrudServiceAgent<
    PatientListDto,
    PatientDetailDto,
    PatientSearchRequestDto,
    CreatePatientRequestDto,
    UpdatePatientRequestDto>
{
    public PatientServiceAgent(HttpClient httpClient, IOptions<ServerConnectionConfig> config)
        : base(httpClient, $"{config.Value.BaseUrl}/api/patients")
    {
    }
}
```

### 3단계: ViewModel 생성
```csharp
// nU3.Modules.EMR.CL/ViewModels/PatientListViewModel.cs
public class PatientListViewModel
{
    private readonly IPatientServiceAgent _serviceAgent;

    public BindingList<PatientListDto> Patients { get; set; }
    public int TotalCount { get; private set; }

    public PatientListViewModel(IPatientServiceAgent serviceAgent)
    {
        _serviceAgent = serviceAgent;
        Patients = new BindingList<PatientListDto>();
    }

    public async Task LoadDataAsync(PatientSearchRequestDto request)
    {
        var result = await _serviceAgent.SearchAsync(request);

        Patients.Clear();
        foreach (var patient in result.Items)
        {
            Patients.Add(patient);
        }

        TotalCount = result.TotalCount;
    }
}
```

### 4단계: 화면 (Control) 생성
```csharp
// nU3.Modules.EMR.CL/Controls/PatientListControl.cs
[nU3ProgramInfo(typeof(PatientListControl), "환자목록", "MOD_EMR_CL", "CHILD")]
public partial class PatientListControl : NuBaseControl
{
    private readonly PatientListViewModel _viewModel;

    public PatientListControl(PatientListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        InitializeGrid();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        var request = new PatientSearchRequestDto
        {
            PatientName = txtPatientName.Text,
            BirthDate = dteBirthDate.DateTime,
            PageNumber = 1,
            PageSize = 50
        };

        await _viewModel.LoadDataAsync(request);
    }
}
```

### 5단계: DI 등록
```csharp
// nU3.Shell/Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Service Agent 등록
    services.AddHttpClient<IPatientServiceAgent, PatientServiceAgent>();

    // ViewModel 등록
    services.AddTransient<PatientListViewModel>();
}
```

### 6단계: 메뉴 등록 (Deployer 도구 사용)
```
1. Deployer 도구 실행
2. 모듈 업로드 (nU3.Modules.EMR.CL.dll)
3. 메뉴 편집기에서 메뉴 구성
   - 메뉴 ID: MENU_PATIENT_LIST
   - 메뉴 명: 환자목록
   - 프로그램 ID: PATIENT_LIST_001
   - 정렬 순서: 1
4. 저장
```

## 7.3 트랜잭션 처리 가이드

### 클라이언트 사이드: 복합 작업 요청
```csharp
// 복합 작업 요청 DTO
public class CreateOrderWithPrescriptionRequestDto : BaseRequestDto
{
    [Required]
    public CreateOrderRequestDto Order { get; set; }

    [Required]
    public List<CreatePrescriptionRequestDto> Prescriptions { get; set; }
}

// ViewModel에서 사용
public async Task CreateOrderWithPrescriptionAsync(
    CreateOrderRequestDto order,
    List<CreatePrescriptionRequestDto> prescriptions)
{
    var request = new CreateOrderWithPrescriptionRequestDto
    {
        Order = order,
        Prescriptions = prescriptions
    };

    var result = await _serviceAgent.CreateOrderWithPrescriptionAsync(request);
}
```

### 서버 사이드: 트랜잭션 처리
```csharp
[Transaction(TransactionScopeOption.Required)]
public async Task<OrderDetailDto> CreateOrderWithPrescriptionAsync(
    CreateOrderWithPrescriptionRequestDto request)
{
    using var transaction = await _unitOfWork.BeginTransactionAsync();

    try
    {
        // 1. 오더 생성
        var order = MapToEntity(request.Order);
        await _orderRepository.AddAsync(order);

        // 2. 처방전 생성 (오더 ID 참조)
        foreach (var prescriptionRequest in request.Prescriptions)
        {
            var prescription = MapToEntity(prescriptionRequest);
            prescription.OrderId = order.OrderId;
            await _prescriptionRepository.AddAsync(prescription);
        }

        // 3. 약물 상호작용 검사
        var interactions = await _cdsService.CheckDrugInteractionsAsync(
            request.Prescriptions.Select(p => p.DrugCode).ToList());

        if (interactions.Any(i => i.Severity == InteractionSeverity.Critical))
        {
            await _unitOfWork.RollbackAsync(transaction);
            throw new DrugInteractionException(interactions);
        }

        // 4. 커밋
        await _unitOfWork.CommitAsync(transaction);

        return MapToDetailDto(order);
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollbackAsync(transaction);
        throw;
    }
}
```

---

# 8. 결론 및 제언

## 8.1 핵심 전략 요약

### 1. **표준화된 아키텍처**
- 3계층 아키텍처 (Presentation → Service Agent → Server)
- Service Agent 패턴으로 통신 로직 추상화
- CRUD 템플릿으로 반복 코드 최소화

### 2. **공통 DTO 관리**
- BaseRequestDto / BaseResponseDto 기반 표준화
- PagedRequestDto / PagedResultDto로 페이징 처리
- Create/Update/DeleteRequestDto 템플릿

### 3. **UI 컴포넌트 표준화**
- DevExpress 래핑 컨트롤 (NuGridControl, NuSearchControl 등)
- 공통 검색/편집 컴포넌트
- 엑셀 내보내기, 레이아웃 저장 등 공통 기능

### 4. **서버 사이드 트랜잭션**
- 복합 작업 요청 DTO
- Service Layer에서 [Transaction] 속성 사용
- Unit of Work 패턴으로 트랜잭션 관리

### 5. **개발 도구 자동화**
- Visual Studio 템플릿 (Item/Project Templates)
- DTO 생성기 (스키마 스캔 → DTO 자동 생성)
- Service Agent 생성기

## 8.2 개발 생산성 향상 기대효과

| 항목 | 개선 전 | 개선 후 | 향상률 |
|------|---------|---------|--------|
| **새 화면 개발 시간** | 3일 | 0.5일 | 83% ↓ |
| **DTO 정의 시간** | 2시간 | 10분 (자동 생성) | 92% ↓ |
| **Service Agent 개발** | 1시간 | 0분 (템플릿) | 100% ↓ |
| **코드 품질** | 중복 많음 | 표준화됨 | ↑ |
| **유지보수** | 어려움 | 쉬움 | ↑ |

## 8.3 우선 구현 순서

### 단계 1: 기반 구축 (2주)
- [ ] BaseRequestDto / BaseResponseDto 구현
- [ ] PagedRequestDto / PagedResultDto 구현
- [ ] BaseCrudServiceAgent 구현
- [ ] NuGridControl, NuSearchControl 기본 구현

### 단계 2: 개발 도구 (2주)
- [ ] Visual Studio 템플릿 작성
- [ ] DTO 생성기 개발
- [ ] Service Agent 생성기 개발
- [ ] 스키마 스캔 도구 개발

### 단계 3: 표준화 확장 (4주)
- [ ] 추가 UI 컴포넌트 개발
- [ ] 에러 처리 표준화
- [ ] 개발 가이드 작성
- [ ] 샘플 프로젝트 제공

### 단계 4: 서버 트랜잭션 (3주)
- [ ] [Transaction] 속성 구현
- [ ] Unit of Work 패턴 구현
- [ ] 복합 작업 요청 DTO 구현
- [ ] 테스트 코드 작성

---

**문서 버전**: 1.0
**최종 수정일**: 2026-02-07
**작성자**: Architecture Team

---

## 📚 참고 자료

### 전자정부 프레임워크
- [eGovFrame 공식 사이트](http://www.egovframe.go.kr/)
- [eGovFrame 개발 가이드](http://www.egovframe.go.kr/wiki/doku.php)

### 아키텍처 패턴
- [Microsoft Architecture Guide](https://docs.microsoft.com/en-us/azure/architecture/)
- [DDD Patterns](https://martinfowler.com/tags/domain%20driven%20design.html)

### DTO 표준
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Repository Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/repository/)
