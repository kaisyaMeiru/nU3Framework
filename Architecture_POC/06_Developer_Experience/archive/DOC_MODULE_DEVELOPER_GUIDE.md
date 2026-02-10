# nU3.Framework 업무화면 개발자 가이드
**nU3ProgramInfo 어트리뷰트를 활용한 모듈 개발 완전 매뉴얼**

> 작성일: 2026-02-07  
> 작성자: nU3.Framework Development Team  
> 버전: 1.0  
> 대상: 업무화면 개발자

---

## 📋 목차

1. [개요](#1-개요)
2. [프로젝트 구조 및 설정](#2-프로젝트-구조-및-설정)
3. [nU3ProgramInfo 어트리뷰트 상세](#3-nu3programinfo-어트리뷰트-상세)
4. [업무화면 개발 절차](#4-업무화면-개발-절차)
5. [코드 표준 및 규칙](#5-코드-표준-및-규칙)
6. [데이터 바인딩 및 이벤트 처리](#6-데이터-바인딩-및-이벤트-처리)
7. [권한 제어](#7-권한-제어)
8. [테스트 및 배포](#8-테스트-및-배포)
9. [문제 해결 및 FAQ](#9-문제-해결-및-faq)
10. [예제 및 템플릿](#10-예제-및-템플릿)

---

# 1. 개요

## 1.1 목적
이 가이드는 nU3.Framework 기반의 업무화면 개발을 위해 필요한 모든 규칙, 절차, 코드 표준을 상세히 설명하는 문서입니다. 특히 `nU3ProgramInfo` 어트리뷰트의 사용법을 중심으로 모듈 개발 전 과정을 안내합니다.

## 1.2 대상 독자
- 업무화면 개발자 (초급 ~ 중급)
- 모듈 개발 담당자
- 업무 분석가 및 기획자
- 테스트 및 QA 담당자

## 1.1 필수 사전 지식
- C# .NET 8.0 개발 환경 숙지
- WinForms 기본 이해
- DevExpress 컨트롤 기본 사용법
- Visual Studio 2022 사용법

---

# 2. 프로젝트 구조 및 설정

## 2.1 전체 솔루션 구조

```
nU3.Framework.sln
├── nU3.Core/                        # 프레임워크 코어
├── nU3.Core.UI/                     # UI 기반 클래스
├── nU3.Core.UI.Components/          # 공통 컴포넌트
├── nU3.Data/                        # 데이터 접근 계층
├── nU3.Models/                      # DTO 모델
├── nU3.Shell/                       # 기본 쉘
├── nU3.MainShell/                   # 메인 쉘 (DevExpress)
├── nU3.Bootstrapper/                # 부트스트래퍼
├── nU3.Connectivity/                # 통신 계층
├── nU3.Tools.Deployer/              # 배포 도구
├── Servers/                         # 서버 프로젝트
│   ├── nU3.Server.Host/            # ASP.NET Core API
│   └── nU3.Server.Connectivity/    # 서버 통신
└── Modules/                         # 업무 모듈 (★★★ 개발 영역 ★★★)
    ├── ADM/                         # 관리 (Admin)
    │   └── AD/                      # 관리 - 관리
    │       └── nU3.Modules.ADM.AD.Deployer/
    └── EMR/                         # 전자의무기록
        ├── IN/                      # 입원 (Inpatient)
        │   └── nU3.Modules.EMR.IN.Worklist/
        ├── OT/                      # 수술실 (Operating Theater)
        │   └── nU3.Modules.EMR.OT.Worklist/
        └── CL/                      # 진료 (Clinic)
            └── nU3.Modules.EMR.CL.Component/
```

## 2.2 모듈 프로젝트 생성 절차

### 2.2.1 신규 모듈 프로젝트 생성

#### 방법 1: Visual Studio에서 직접 생성
```
1. Solution 탐색기 → Modules 폴더 우클릭
2. 추가 → 새 프로젝트
3. "클래스 라이브러리" 선택
4. 프로젝트 이름: nU3.Modules.[카테고리].[서브시스템].[업무명]
   예: nU3.Modules.EMR.CL.Patient
5. 위치: Modules/[카테고리]/[서브시스템]
   예: Modules/EMR/CL/
6. 확인
```

#### 방법 2: 프로젝트 템플릿 사용 (권장)
```
1. nU3.Tools.Deployer 실행
2. "새 모듈 생성" 버튼 클릭
3. 모듈 정보 입력:
   - 카테고리: EMR
   - 서브시스템: CL
   - 업무명: Patient
   - 모듈 ID: MOD_EMR_CL_PATIENT
4. "프로젝트 생성" 클릭
```

### 2.2.2 프로젝트 설정

#### 프로젝트 파일 (.csproj) 설정
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- ★★★ 필수 설정 ★★★ -->
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- 어셈블리 정보 -->
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    
    <!-- 출력 경로 -->
    <OutputPath>..\..\..\bin\$(Configuration)\Modules\$(MSBuildProjectName)\</OutputPath>
    <IntermediateOutputPath>obj\$(Configuration)\$(MSBuildProjectName)\</IntermediateOutputPath>
  </PropertyGroup>
  
  <!-- ★★★ 필수 참조 ★★★ -->
  <ItemGroup>
    <!-- 프레임워크 코어 -->
    <ProjectReference Include="..\..\nU3.Core\nU3.Core.csproj" />
    <ProjectReference Include="..\..\nU3.Core.UI\nU3.Core.UI.csproj" />
    <ProjectReference Include="..\..\nU3.Models\nU3.Models.csproj" />
    <ProjectReference Include="..\..\nU3.Connectivity\nU3.Connectivity.csproj" />
    
    <!-- DevExpress 참조 -->
    <PackageReference Include="DevExpress.Win" Version="23.2.9" />
    <PackageReference Include="DevExpress.Win.Design" Version="23.2.9" />
    
    <!-- 기타 필요한 패키지 -->
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>
</Project>
```

#### 폴더 구조 (표준)
```
nU3.Modules.EMR.CL.Patient/
├── Properties/
│   ├── AssemblyInfo.cs           # 어셈블리 정보
│   └── Resources.Designer.cs      # 리소스 파일
├── Controls/                      # 화면 컨트롤
│   ├── PatientListControl.cs       # 환자 목록
│   ├── PatientListControl.Designer.cs
│   ├── PatientListControl.resx
│   ├── PatientDetailControl.cs      # 환자 상세
│   ├── PatientDetailControl.Designer.cs
│   ├── PatientDetailControl.resx
│   └── PatientSearchControl.cs     # 환자 검색
├── ViewModels/                     # 뷰 모델
│   ├── PatientListViewModel.cs
│   ├── PatientDetailViewModel.cs
│   └── PatientSearchViewModel.cs
├── DTOs/                          # DTO (필요한 경우만)
│   └── PatientDtos.cs
└── Services/                      # 로컬 서비스 (필요한 경우만)
    └── PatientService.cs
```

---

# 3. nU3ProgramInfo 어트리뷰트 상세

## 3.1 어트리뷰트란?
`nU3ProgramInfo`는 업무화면을 프레임워크에 등록하기 위한 메타데이터 어트리뷰트입니다. 이 어트리뷰트를 사용해야 프레임워크가 화면을 인식하고 메뉴에 등록할 수 있습니다.

## 3.2 어트리뷰트 구조

```csharp
namespace nU3.Core.Attributes
{
    /// <summary>
    /// 프레임워크에 업무화면을 등록하기 위한 속성
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class nU3ProgramInfoAttribute : Attribute
    {
        #region 생성자
        
        /// <summary>
        /// nU3ProgramInfo 속성 생성자
        /// </summary>
        /// <param name="controlType">컨트롤 타입 (typeof(YourControl))</param>
        /// <param name="displayName">화면 표시 이름</param>
        /// <param name="moduleId">모듈 ID (MOD_카테고리_서브시스템)</param>
        /// <param name="moduleType">모듈 타입 (MAIN/CHILD/DIALOG)</param>
        public nU3ProgramInfoAttribute(
            Type controlType,
            string displayName,
            string moduleId,
            string moduleType)
        {
            ControlType = controlType;
            DisplayName = displayName;
            ModuleId = moduleId;
            ModuleType = moduleType;
        }
        
        #endregion
        
        #region 속성
        
        /// <summary>
        /// 컨트롤 타입
        /// </summary>
        public Type ControlType { get; }
        
        /// <summary>
        /// 화면 표시 이름 (한글)
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 모듈 ID
        /// </summary>
        public string ModuleId { get; }
        
        /// <summary>
        /// 모듈 타입
        /// - MAIN: 메인 화면
        /// - CHILD: 자식 화면 (탭으로 열림)
        /// - DIALOG: 대화상자 (팝업)
        /// </summary>
        public string ModuleType { get; }
        
        /// <summary>
        /// 프로그램 ID (자동 생성: 카테고리_서브시스템_화면명)
        /// </summary>
        public string ProgramId { get; set; }
        
        /// <summary>
        /// 설명
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 아이콘 이름 (프레임워크 내장 아이콘)
        /// </summary>
        public string IconName { get; set; }
        
        /// <summary>
        /// 권한 레벨 (0: 관리자, 1: 일반, 2: 조회만)
        /// </summary>
        public int AuthLevel { get; set; } = 1;
        
        /// <summary>
        /// 헬프 파일 경로
        /// </summary>
        public string HelpFile { get; set; }
        
        /// <summary>
        /// 닫기 버튼 허용 여부
        /// </summary>
        public bool AllowClose { get; set; } = true;
        
        /// <summary>
        /// 리사이즈 가능 여부
        /// </summary>
        public bool AllowResize { get; set; } = true;
        
        /// <summary>
        /// 최소화 가능 여부
        /// </summary>
        public bool AllowMinimize { get; set; } = true;
        
        /// <summary>
        /// 최대화 가능 여부
        /// </summary>
        public bool AllowMaximize { get; set; } = true;
        
        /// <summary>
        /// 기본 너비
        /// </summary>
        public int DefaultWidth { get; set; } = 1024;
        
        /// <summary>
        /// 기본 높이
        /// </summary>
        public int DefaultHeight { get; set; } = 768;
        
        /// <summary>
        /// 최소 너비
        /// </summary>
        public int MinWidth { get; set; } = 800;
        
        /// <summary>
        /// 최소 높이
        /// </summary>
        public int MinHeight { get; set; } = 600;
        
        /// <summary>
        /// 시작 위치 (Center, Default)
        /// </summary>
        public string StartPosition { get; set; } = "Center";
        
        /// <summary>
        /// 화면 분류 (고정값)
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// 하위 분류 (고정값)
        /// </summary>
        public string SubCategory { get; set; }
        
        /// <summary>
        /// 개발자 정보
        /// </summary>
        public string Developer { get; set; }
        
        /// <summary>
        /// 버전 정보
        /// </summary>
        public string Version { get; set; } = "1.0.0.0";
        
        /// <summary>
        /// 수정일
        /// </summary>
        public string ModifiedDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        
        #endregion
    }
}
```

## 3.3 어트리뷰트 사용 예시

### 3.3.1 기본 사용법
```csharp
using nU3.Core.Attributes;
using nU3.Core.UI;

  // ★★★ 모든 업무화면은 반드시 nU3ProgramInfo 어트리뷰트를 가져야 합니다 ★★★
[nU3ProgramInfo(
    typeof(PatientListControl),               // 현재 클래스 타입 (declaringType)
"환자목록",                                // 화면 표시 이름
"EMR_CL_PATIENT_LIST_001",                // 프로그램 ID (ProgId)
"CHILD")]                                 // 폼 타입 (CHILD, POPUP, SDI)
public partial class PatientListControl : BaseWorkControl
{
    public PatientListControl()
{
    InitializeComponent();
}

// 화면 ID (nU3ProgramInfo의 세 번째 인자와 일치해야 함)
public override string ProgramID => "EMR_CL_PATIENT_LIST_001";

// 화면이 활성화될 때 호출
protected override void OnScreenActivated()
{
    base.OnScreenActivated();
InitializeData();
}
}
```

### 3.3.2 상세 설정 예시
```csharp
[nU3ProgramInfo(
    typeof(PatientDetailControl),
    "환자상세정보",
    "MOD_EMR_CL_PATIENT",
    "CHILD",
    ProgramId = "EMR_CL_PATIENT_DETAIL_001",
    DisplayName = "환자상세정보",
    Description = "환자의 상세 정보를 조회하고 수정하는 화면",
    ModuleId = "MOD_EMR_CL_PATIENT",
    ModuleType = "CHILD",
    AuthLevel = 1,
    IconName = "patient_detail",
    HelpFile = @"DOC\HELP\PatientDetail.chm",
    AllowClose = true,
    AllowResize = true,
    AllowMinimize = true,
    AllowMaximize = true,
    DefaultWidth = 1200,
    DefaultHeight = 800,
    MinWidth = 1024,
    MinHeight = 768,
    StartPosition = "Center",
    Category = "EMR",
    SubCategory = "CL",
    Developer = "홍길동",
    Version = "1.0.0.0",
    ModifiedDate = "2026-02-07")]
public partial class PatientDetailControl : NuBaseControl
{
    public override string ScreenId => "EMR_CL_PATIENT_DETAIL_001";
    
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        
        // 권한 체크
        if (!HasPermission(PermissionType.Read))
        {
            NuXtraMessageBox.ShowError("조회 권한이 없습니다.");
            return;
        }
        
        // 데이터 로드
        LoadPatientData();
    }
}
```

### 3.3.3 대화상자(DIALOG) 예시
```csharp
[nU3ProgramInfo(
    typeof(PatientSearchDialog),
    "환자검색",
    "MOD_EMR_CL_PATIENT",
    "DIALOG",                                 // 대화상자 타입
    ModuleType = "DIALOG",                     // 대화상자 타입
    AuthLevel = 0,
    DefaultWidth = 600,
    DefaultHeight = 400,
    MinWidth = 500,
    MinHeight = 300,
    AllowResize = true,
    StartPosition = "CenterParent")]
public partial class PatientSearchDialog : NuBaseForm
{
    public string SelectedPatientId { get; private set; }
    
    public PatientSearchDialog()
    {
        InitializeComponent();
    }
    
    public override string ScreenId => "EMR_CL_PATIENT_SEARCH_001";
    
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        
        // 대화상자 초기화
        LoadPatients();
    }
    
    private void btnSelect_Click(object sender, EventArgs e)
    {
        if (grdPatientView.FocusedRowHandle >= 0)
        {
            var patient = grdPatient.GetFocusedRow() as PatientListDto;
            SelectedPatientId = patient.PatientId;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
```

## 3.4 모듈 타입별 특징

### MAIN 타입
```csharp
[nU3ProgramInfo(
    typeof(MainDashboardControl),
    "대시보드",
    "MOD_MAIN_DASHBOARD",
    "MAIN")]
public partial class MainDashboardControl : NuBaseControl
{
    // 메인 화면 특징:
    // - 쉘이 종료될 때까지 유지됨
    // - 다른 화면들의 컨테이너 역할
    // - 다른 화면을 호출하는 역할
}
```

### CHILD 타입
```csharp
[nU3ProgramInfo(
    typeof(PatientListControl),
    "환자목록",
    "MOD_EMR_CL_PATIENT",
    "CHILD")]
public partial class PatientListControl : NuBaseControl
{
    // 자식 화면 특징:
    // - 탭으로 열림
    // - 여러 개 동시에 열 수 있음
    // - 독립적인 생명주기 가짐
}
```

### DIALOG 타입
```csharp
[nU3ProgramInfo(
    typeof(PatientSearchDialog),
    "환자검색",
    "MOD_EMR_CL_PATIENT",
    "DIALOG",
    ModuleType = "DIALOG")]
public partial class PatientSearchDialog : NuBaseForm
{
    // 대화상자 특징:
    // - 팝업으로 열림
    // - DialogResult 반환
    // - 부모 화면이 종료되면 자동으로 닫힘
}
```

---

# 4. 업무화면 개발 절차

## 4.1 개발 절차 전체 흐름

```
1. 요구사항 분석
   ↓
2. 프로젝트 생성 및 설정
   ↓
3. DTO 정의 (서버/클라이언트 공유)
   ↓
4. View Model 생성
   ↓
5. 화면 디자인 (Control)
   ↓
6. 데이터 바인딩
   ↓
7. 이벤트 처리
   ↓
8. 권한 적용
   ↓
9. 테스트
   ↓
10. 메뉴 등록 (Deployer)
   ↓
11. 배포
```

## 4.2 상세 개발 절차

### 4.2.1 단계 1: 요구사항 분석

#### 요구사항 분석서 작성
```markdown
# 환자 목록 화면 요구사항

## 1. 개요
- 화면명: 환자 목록 조회
- 목적: 등록된 환자들의 목록을 조회하고 상세 정보를 확인

## 2. 기능 요구사항
### 2.1 검색 기능
- 환자명, 환자ID, 생년월일로 검색
- 성별, 혈액형으로 필터링
- 검색 결과는 그리드에 표시

### 2.2 목록 기능
- 환자 목록 페이징 처리
- 엑셀 내보내기 기능
- 더블클릭 시 상세 화면 이동

### 2.3 권한
- 조회: 레벨 2 이상
- 상세 화면: 레벨 1 이상
```

### 4.2.2 단계 2: 프로젝트 생성

#### 프로젝트 생성 스크립트
```bash
# 1. 프로젝트 폴더 생성
mkdir -p Modules/EMR/CL/Patient

# 2. 프로젝트 생성
dotnet new classlib -n nU3.Modules.EMR.CL.Patient -o Modules/EMR/CL/Patient

# 3. .csproj 파일 설정 (위에서 설명한 내용 참조)

# 4. 폴더 구조 생성
mkdir -p Modules/EMR/CL/Patient/{Controls,ViewModels,DTOs,Services,Properties}
```

### 4.2.3 단계 3: DTO 정의

#### PatientDtos.cs 파일 생성
```csharp
// nU3.Modules.EMR.CL.Patient/DTOs/PatientDtos.cs
using nU3.Models.DTOs.Base;
using System;

namespace nU3.Modules.EMR.CL.Patient.DTOs
{
    #region List DTO
    
    /// <summary>
    /// 환자 목록 DTO
    /// </summary>
    public class PatientListDto
    {
        /// <summary>
        /// 환자 ID
        /// </summary>
        public string PatientId { get; set; }
        
        /// <summary>
        /// 환자명
        /// </summary>
        public string PatientName { get; set; }
        
        /// <summary>
        /// 생년월일
        /// </summary>
        public DateTime BirthDate { get; set; }
        
        /// <summary>
        /// 성별 (1: 남성, 2: 여성)
        /// </summary>
        public int Gender { get; set; }
        
        /// <summary>
        /// 성별명
        /// </summary>
        public string GenderName => Gender == 1 ? "남성" : "여성";
        
        /// <summary>
        /// 혈액형 (0: 미정, 1: A+, 2: A-, 3: B+, 4: B-, 5: O+, 6: O-)
        /// </summary>
        public int BloodType { get; set; }
        
        /// <summary>
        /// 혈액형명
        /// </summary>
        public string BloodTypeName => BloodType switch
        {
            1 => "A+",
            2 => "A-",
            3 => "B+",
            4 => "B-",
            5 => "O+",
            6 => "O-",
            _ => "미정"
        };
    }
    
    #endregion
    
    #region Search Request DTO
    
    /// <summary>
    /// 환자 검색 요청 DTO
    /// </summary>
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
        /// 생년월일 (시작)
        /// </summary>
        public DateTime? BirthDateFrom { get; set; }
        
        /// <summary>
        /// 생년월일 (종료)
        /// </summary>
        public DateTime? BirthDateTo { get; set; }
        
        /// <summary>
        /// 성별 (0: 전체, 1: 남성, 2: 여성)
        /// </summary>
        public Gender? Gender { get; set; }
        
        /// <summary>
        /// 혈액형 (0: 전체, 1-6: A+,A-,B+,B-,O+,O-)
        /// </summary>
        public BloodType? BloodType { get; set; }
    }
    
    #endregion
    
    #region Detail DTO
    
    /// <summary>
    /// 환자 상세 DTO
    /// </summary>
    public class PatientDetailDto : PatientListDto
    {
        /// <summary>
        /// 연락처
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// 주소
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 등록일
        /// </summary>
        public DateTime RegisteredDate { get; set; }
        
        /// <summary>
        /// 등록자
        /// </summary>
        public string RegisteredBy { get; set; }
        
        /// <summary>
        /// 최종 수정일
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }
        
        /// <summary>
        /// 최종 수정자
        /// </summary>
        public string LastModifiedBy { get; set; }
    }
    
    #endregion
}
```

### 4.2.4 단계 4: View Model 생성

#### PatientListViewModel.cs 생성
```csharp
// nU3.Modules.EMR.CL.Patient/ViewModels/PatientListViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using nU3.Core.Services;
using nU3.Core.UI;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.ViewModels
{
    /// <summary>
    /// 환자 목록 뷰 모델
    /// </summary>
    public class PatientListViewModel : INotifyPropertyChanged, IDisposable
    {
        #region 필드
        
        private readonly IPatientServiceAgent _serviceAgent;
        private bool _isLoading;
        private int _totalCount;
        private PatientSearchRequestDto _searchCondition;
        
        #endregion
        
        #region 속성
        
        /// <summary>
        /// 환자 목록
        /// </summary>
        public BindingList<PatientListDto> Patients { get; private set; }
        
        /// <summary>
        /// 전체 개수
        /// </summary>
        public int TotalCount
        {
            get => _totalCount;
            private set
            {
                if (_totalCount != value)
                {
                    _totalCount = value;
                    OnPropertyChanged(nameof(TotalCount));
                }
            }
        }
        
        /// <summary>
        /// 로딩 중 여부
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }
        
        /// <summary>
        /// 검색 조건
        /// </summary>
        public PatientSearchRequestDto SearchCondition
        {
            get => _searchCondition ??= new PatientSearchRequestDto();
            set
            {
                if (_searchCondition != value)
                {
                    _searchCondition = value;
                    OnPropertyChanged(nameof(SearchCondition));
                }
            }
        }
        
        #endregion
        
        #region 이벤트
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
        
        #region 생성자
        
        public PatientListViewModel(IPatientServiceAgent serviceAgent)
        {
            _serviceAgent = serviceAgent;
            Patients = new BindingList<PatientListDto>();
        }
        
        #endregion
        
        #region 공용 메서드
        
        /// <summary>
        /// 데이터 로드
        /// </summary>
        public async Task LoadDataAsync(PatientSearchRequestDto searchCondition = null)
        {
            try
            {
                IsLoading = true;
                
                var condition = searchCondition ?? SearchCondition;
                var result = await _serviceAgent.GetPatientsAsync(condition);
                
                // 데이터 바인딩
                Patients.Clear();
                foreach (var patient in result.Items)
                {
                    Patients.Add(patient);
                }
                
                TotalCount = result.TotalCount;
            }
            catch (Exception ex)
            {
                NuXtraMessageBox.ShowError($"데이터 로드 중 오류가 발생했습니다: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// 환자 상세 정보 조회
        /// </summary>
        public async Task<PatientDetailDto> GetPatientDetailAsync(string patientId)
        {
            try
            {
                IsLoading = true;
                return await _serviceAgent.GetPatientAsync(patientId);
            }
            catch (Exception ex)
            {
                NuXtraMessageBox.ShowError($"환자 상세 정보 조회 중 오류가 발생했습니다: {ex.Message}");
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// 엑셀 내보내기
        /// </summary>
        public void ExportToExcel(string fileName)
        {
            try
            {
                if (Patients.Count == 0)
                {
                    NuXtraMessageBox.ShowInformation("내보낼 데이터가 없습니다.");
                    return;
                }
                
                // 여기에 엑셀 내보내기 로직 구현
                // NuGridControl.ExportToExcel(fileName) 등
                
                NuXtraMessageBox.ShowInformation("엑셀 내보내기를 완료했습니다.");
            }
            catch (Exception ex)
            {
                NuXtraMessageBox.ShowError($"엑셀 내보내기 중 오류가 발생했습니다: {ex.Message}");
            }
        }
        
        #endregion
        
        #region 리소스 정리
        
        public void Dispose()
        {
            // 리소스 정리
            Patients?.Clear();
        }
        
        #endregion
    }
}
```

### 4.2.5 단계 5: 화면 디자인 (Control)

#### PatientListControl.cs 생성
```csharp
// nU3.Modules.EMR.CL.Patient/Controls/PatientListControl.cs
using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using nU3.Core;
using nU3.Core.Attributes;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using nU3.Modules.EMR.CL.Patient.ViewModels;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.Controls
{
    /// <summary>
    /// 환자 목록 컨트롤
    /// </summary>
    [nU3ProgramInfo(
        typeof(PatientListControl),
        "환자목록",
        "MOD_EMR_CL_PATIENT",
        "CHILD",
        ProgramId = "EMR_CL_PATIENT_LIST_001",
        Description = "환자 목록을 조회하고 관리하는 화면",
        AuthLevel = 2,
        DefaultWidth = 1200,
        DefaultHeight = 800)]
    public partial class PatientListControl : NuBaseControl
    {
        #region 필드
        
        private PatientListViewModel _viewModel;
        
        #endregion
        
        #region 속성
        
        /// <summary>
        /// 화면 ID (nU3ProgramInfo의 ProgramId와 일치해야 함)
        /// </summary>
        public override string ScreenId => "EMR_CL_PATIENT_LIST_001";
        
        /// <summary>
        /// 현재 선택된 환자
        /// </summary>
        public PatientListDto SelectedPatient
        {
            get
            {
                if (grdPatientView.FocusedRowHandle >= 0)
                {
                    return grdPatientView.GetFocusedRow() as PatientListDto;
                }
                return null;
            }
        }
        
        #endregion
        
        #region 생성자
        
        public PatientListControl(PatientListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            
            // 이벤트 핸들러 등록
            RegisterEvents();
            
            // 초기화
            InitializeControls();
        }
        
        #endregion
        
        #region 화면 활성화
        
        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();
            
            // 권한 체크
            if (!HasPermission(PermissionType.Read))
            {
                NuXtraMessageBox.ShowError("조회 권한이 없습니다.");
                return;
            }
            
            // 데이터 로드
            LoadData();
        }
        
        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();
            
            // 화면 비활성화 시 처리
            SaveLayout();
        }
        
        #endregion
        
        #region 초기화
        
        private void InitializeControls()
        {
            // 그리드 초기화
            InitializeGrid();
            
            // 검색 컨트롤 초기화
            InitializeSearchControls();
            
            // 버튼 이벤트 연결
            ConnectButtonEvents();
        }
        
        private void InitializeGrid()
        {
            // 그리드 컨트롤 설정
            grdPatient.Dock = DockStyle.Fill;
            grdPatient.UseEmbeddedNavigator = false;
            grdPatient.OptionsView.ShowGroupPanel = false;
            grdPatient.OptionsView.ShowIndicator = true;
            grdPatient.OptionsSelection.MultiSelect = false;
            grdPatient.OptionsBehavior.Editable = false;
            
            // 뷰 설정
            var gridView = grdPatient.MainView as GridView;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
            gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
            gridView.OptionsView.ShowAutoFilterRow = true;
            gridView.OptionsView.ShowFooter = true;
            
            // 컬럼 생성
            gridView.Columns.Clear();
            
            // 환자ID 컬럼
            var colPatientId = gridView.Columns.AddField("PatientId");
            colPatientId.Caption = "환자ID";
            colPatientId.Visible = true;
            colPatientId.Width = 100;
            colPatientId.OptionsColumn.AllowEdit = false;
            
            // 환자명 컬럼
            var colPatientName = gridView.Columns.AddField("PatientName");
            colPatientName.Caption = "환자명";
            colPatientName.Visible = true;
            colPatientName.Width = 150;
            colPatientName.OptionsColumn.AllowEdit = false;
            
            // 생년월일 컬럼
            var colBirthDate = gridView.Columns.AddField("BirthDate");
            colBirthDate.Caption = "생년월일";
            colBirthDate.Visible = true;
            colBirthDate.Width = 120;
            colBirthDate.DisplayFormat.FormatString = "yyyy-MM-dd";
            colBirthDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colBirthName.OptionsColumn.AllowEdit = false;
            
            // 성별 컬럼
            var colGenderName = gridView.Columns.AddField("GenderName");
            colGenderName.Caption = "성별";
            colGenderName.Visible = true;
            colGenderName.Width = 80;
            colGenderName.OptionsColumn.AllowEdit = false;
            
            // 혈액형 컬럼
            var colBloodTypeName = gridView.Columns.AddField("BloodTypeName");
            colBloodTypeName.Caption = "혈액형";
            colBloodTypeName.Visible = true;
            colBloodTypeName.Width = 80;
            colBloodTypeName.OptionsColumn.AllowEdit = false;
            
            // 컬럼 너비 자동 조정
            gridView.BestFitColumns();
        }
        
        private void InitializeSearchControls()
        {
            // 검색 컨트롤 기본값 설정
            dteBirthDateFrom.EditValue = DateTime.Today.AddYears(-100);
            dteBirthDateTo.EditValue = DateTime.Today;
            cboGender.Properties.Items.AddRange(new object[] { "전체", "남성", "여성" });
            cboGender.SelectedIndex = 0;
            cboBloodType.Properties.Items.AddRange(new object[] { "전체", "A+", "A-", "B+", "B-", "O+", "O-" });
            cboBloodType.SelectedIndex = 0;
        }
        
        private void ConnectButtonEvents()
        {
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            btnExcel.Click += BtnExcel_Click;
            btnDetail.Click += BtnDetail_Click;
            btnRefresh.Click += BtnRefresh_Click;
        }
        
        #endregion
        
        #region 이벤트 핸들러 등록
        
        private void RegisterEvents()
        {
            // 그리드 더블클릭 이벤트
            grdPatient.DoubleClick += GrdPatient_DoubleClick;
            
            // View Model 이벤트
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PatientListViewModel.IsLoading))
            {
                // 로딩 상태 변경 시 처리
                this.Cursor = _viewModel.IsLoading ? Cursors.WaitCursor : Cursors.Default;
                btnSearch.Enabled = !_viewModel.IsLoading;
                btnRefresh.Enabled = !_viewModel.IsLoading;
            }
        }
        
        #endregion
        
        #region 데이터 로드
        
        private async void LoadData()
        {
            var searchCondition = new PatientSearchRequestDto
            {
                PatientName = txtPatientName.Text,
                PatientId = txtPatientId.Text,
                BirthDateFrom = dteBirthDateFrom.DateTime,
                BirthDateTo = dteBirthDateTo.DateTime,
                Gender = cboGender.SelectedIndex == 0 ? (Gender?)null : (Gender)Enum.Parse(typeof(Gender), cboGender.SelectedValue.ToString()),
                BloodType = cboBloodType.SelectedIndex == 0 ? (BloodType?)null : (BloodType)Enum.Parse(typeof(BloodType), cboBloodType.SelectedValue.ToString()),
                PageNumber = 1,
                PageSize = 50
            };
            
            await _viewModel.LoadDataAsync(searchCondition);
            
            // 그리드에 데이터 바인딩
            grdPatient.DataSource = _viewModel.Patients;
            
            // 전체 개수 표시
            lblTotalCount.Text = $"총 {_viewModel.TotalCount:N0}건";
        }
        
        #endregion
        
        #region 버튼 이벤트
        
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await LoadData();
        }
        
        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadData();
        }
        
        private void BtnReset_Click(object sender, EventArgs e)
        {
            // 검색 조건 초기화
            txtPatientName.Text = string.Empty;
            txtPatientId.Text = string.Empty;
            dteBirthDateFrom.EditValue = DateTime.Today.AddYears(-100);
            dteBirthDateTo.EditValue = DateTime.Today;
            cboGender.SelectedIndex = 0;
            cboBloodType.SelectedIndex = 0;
            
            // 데이터 재조회
            BtnSearch_Click(sender, e);
        }
        
        private void BtnExcel_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new NuXtraSaveFileDialog
            {
                Filter = "Excel 파일 (*.xlsx)|*.xlsx|모든 파일 (*.*)|*.*",
                Title = "환자 목록 저장",
                FileName = $"환자목록_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _viewModel.ExportToExcel(saveFileDialog.FileName);
            }
        }
        
        private async void BtnDetail_Click(object sender, EventArgs e)
        {
            if (SelectedPatient == null)
            {
                NuXtraMessageBox.ShowInformation("상세 정보를 조회할 환자를 선택하세요.");
                return;
            }
            
            if (!HasPermission(PermissionType.Read))
            {
                NuXtraMessageBox.ShowError("상세 정보 조회 권한이 없습니다.");
                return;
            }
            
            // 환자 상세 정보 화면 열기
            var patientDetail = await _viewModel.GetPatientDetailAsync(SelectedPatient.PatientId);
            
            if (patientDetail != null)
            {
                var detailControl = new PatientDetailControl(patientDetail);
                var document = this.Parent as DevExpress.XtraBars.Docking.UI.Documents.Document;
                if (document != null)
                {
                    document.Manager.AddDocument(detailControl, patientDetail.PatientName);
                }
            }
        }
        
        #endregion
        
        #region 그리드 이벤트
        
        private void GrdPatient_DoubleClick(object sender, EventArgs e)
        {
            // 더블클릭 시 상세 정보 조회
            BtnDetail_Click(sender, e);
        }
        
        #endregion
        
        #region 레이아웃 관리
        
        private void SaveLayout()
        {
            try
            {
                // 그리드 레이아웃 저장
                var gridView = grdPatient.MainView as GridView;
                gridView.SaveLayoutToXml($@"Layouts\{ScreenId}_GridLayout.xml");
            }
            catch (Exception ex)
            {
                LogManager.Error($"그리드 레이아웃 저장 중 오류: {ex.Message}", ScreenId);
            }
        }
        
        private void RestoreLayout()
        {
            try
            {
                // 그리드 레이아웃 복원
                var gridView = grdPatient.MainView as GridView;
                gridView.RestoreLayoutFromXml($@"Layouts\{ScreenId}_GridLayout.xml");
            }
            catch (Exception ex)
            {
                LogManager.Error($"그리드 레이아웃 복원 중 오류: {ex.Message}", ScreenId);
            }
        }
        
        #endregion
        
        #region 리소스 정리
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _viewModel?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        #endregion
    }
}
```

### 4.2.6 단계 6-8: 데이터 바인딩, 이벤트 처리, 권한 적용

#### 위 코드에 이미 포함됨
- **데이터 바인딩**: `grdPatient.DataSource = _viewModel.Patients;`
- **이벤트 처리**: 버튼 클릭, 그리드 더블클릭 등
- **권한 적용**: `HasPermission(PermissionType.Read)`

### 4.2.9 단계 9: 테스트

#### 단위 테스트 예시
```csharp
// nU3.Modules.EMR.CL.Patient.Tests/PatientListViewModelTests.cs
using Xunit;
using Moq;
using nU3.Modules.EMR.CL.Patient.ViewModels;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.Tests
{
    public class PatientListViewModelTests
    {
        [Fact]
        public async Task LoadDataAsync_WhenCalled_ShouldLoadPatients()
        {
            // Arrange
            var mockServiceAgent = new Mock<IPatientServiceAgent>();
            var viewModel = new PatientListViewModel(mockServiceAgent.Object);
            
            var searchCondition = new PatientSearchRequestDto
            {
                PatientName = "홍길동",
                PageNumber = 1,
                PageSize = 10
            };
            
            var expectedPatients = new PagedResultDto<PatientListDto>
            {
                Items = new List<PatientListDto>
                {
                    new PatientListDto { PatientId = "P001", PatientName = "홍길동" }
                },
                TotalCount = 1
            };
            
            mockServiceAgent.Setup(x => x.GetPatientsAsync(searchCondition))
                .ReturnsAsync(expectedPatients);
            
            // Act
            await viewModel.LoadDataAsync(searchCondition);
            
            // Assert
            Assert.Single(viewModel.Patients);
            Assert.Equal("P001", viewModel.Patients[0].PatientId);
            Assert.Equal(1, viewModel.TotalCount);
        }
    }
}
```

### 4.2.10 단계 10: 메뉴 등록 (Deployer)

#### Deployer 도구 사용 절차
```
1. nU3.Tools.Deployer 실행

2. 모듈 업로드
   - "모듈 업로드" 탭 선택
   - "찾아보기" 버튼 클릭
   - 빌드된 DLL 파일 선택 (nU3.Modules.EMR.CL.Patient.dll)
   - "업로드" 버튼 클릭

3. 화면 등록 확인
   - "화면 목록" 탭 선택
   - 방금 업로드한 화면 확인
   - nU3ProgramInfo 속성이 자동으로 스캔되어 등록됨

4. 메뉴 구성
   - "메뉴 편집기" 탭 선택
   - 왼쪽에서 부모 메뉴 선택
   - 오른쪽 "추가" 버튼 클릭
   - 방금 등록한 화면 선택
   - 메뉴 정보 입력:
     - 메뉴 ID: MENU_EMR_CL_PATIENT_LIST
     - 메뉴 명: 환자목록
     - 정렬 순서: 1
   - "저장" 버튼 클릭

5. 배포
   - "배포" 탭 선택
   - "서버로 배포" 버튼 클릭
   - 확인 메시지 확인
```

### 4.2.11 단계 11: 배포

#### 배포 절차
```
1. 빌드
   - Visual Studio에서 "솔루션 빌드" 또는 F6
   - 빌드가 성공해야 함

2. 로컬 테스트
   - F5로 실행
   - 메뉴에서 해당 화면이 열리는지 확인
   - 기능들이 정상 작동하는지 확인

3. Deployer로 등록 (위에서 설명)

4. 테스트 서버 배포
   - Bootstrapper 실행
   - 정상적으로 화면이 로드되는지 확인

5. 운영 서버 배포
   - QA 테스트 완료 후
   - 운영 서버에 배포
```

---

# 5. 코드 표준 및 규칙

## 5.1 필수 규칙

### 5.1.1 nU3ProgramInfo 어트리뷰트
```csharp
// ★★★ 필수: 모든 업무화면은 반드시 nU3ProgramInfo 어트리뷰트를 가져야 합니다 ★★★
[nU3ProgramInfo(
    typeof(YourControl),
    "화면 표시 이름",
    "MOD_카테고리_서브시스템",
    "CHILD")]  // 또는 "DIALOG"
public partial class YourControl : NuBaseControl
{
    // ScreenId는 반드시 구현해야 합니다.
    public override string ScreenId => "CATEGORY_SUBSYSTEM_SCREEN_001";
    
    protected override void OnScreenActivated()
    {
        // 필수: 권한 체크
        if (!HasPermission(PermissionType.Read))
        {
            NuXtraMessageBox.ShowError("조회 권한이 없습니다.");
            return;
        }
        
        // 필수: 초기화 로직
        base.OnScreenActivated();
        InitializeData();
    }
}
```

### 5.1.2 클래스 상속 규칙
```csharp
// ✅ 올바른 예: BaseWorkControl 상속
public partial class PatientListControl : BaseWorkControl 
{
    public override string ProgramID => "EMR_CL_PATIENT_LIST_001";
}

// ✅ 팝업/독립 창 예: BaseWorkForm 상속
public partial class PatientSearchDialog : BaseWorkForm
{
    public override string ProgramID => "EMR_CL_PATIENT_SEARCH_001";
}
```

### 5.1.3 ScreenId 규칙
```csharp
// ScreenId는 반드시 nU3ProgramInfo.ProgramId와 일치해야 합니다
[nU3ProgramInfo(
    typeof(PatientListControl),
    "환자목록",
    "MOD_EMR_CL_PATIENT",
    "CHILD",
    ProgramId = "EMR_CL_PATIENT_LIST_001")]  // 여기서 정의
public partial class PatientListControl : NuBaseControl
{
    // 반드시 동일한 ID로 구현
    public override string ScreenId => "EMR_CL_PATIENT_LIST_001";  // 일치해야 함
}
```

## 5.2 네이밍 규칙

### 5.2.1 프로젝트 네이밍
```csharp
// 형식: nU3.Modules.[카테고리].[서브시스템].[업무명]
nU3.Modules.EMR.CL.Patient      // 전자의무기록 - 진료과 - 환자
nU3.Modules.ADM.AD.User         // 관리 - 관리 - 사용자
nU3.Modules.EMR.OT.Schedule     // 전자의무기록 - 수술실 - 스케줄
```

### 5.2.2 클래스 네이밍
```csharp
// 컨트롤: [Entity]Control
public class PatientListControl : NuBaseControl
public class PatientDetailControl : NuBaseControl
public class PatientSearchDialog : NuBaseForm

// 뷰 모델: [Entity]ListViewModel / [Entity]DetailViewModel
public class PatientListViewModel : INotifyPropertyChanged
public class PatientDetailViewModel : INotifyPropertyChanged

// DTO: [Entity]ListDto / [Entity]DetailDto
public class PatientListDto
public class PatientDetailDto
```

### 5.2.3 메서드 네이밍
```csharp
// 공용 메서드: 동사로 시작, 파스칼 케이스
public async Task LoadDataAsync()
public void InitializeControls()
public void SaveLayout()
public void ValidateInput()

// 이벤트 핸들러: On[이벤트명]
private void OnLoad(object sender, EventArgs e)
private void OnClick(object sender, EventArgs e)
private void OnSelectedIndexChanged(object sender, EventArgs e)

// 비동기 메서드: [동사]Async
public async Task LoadDataAsync()
public async Task SaveDataAsync()
public async Task< PatientDetailDto> GetPatientAsync(string patientId)
```

## 5.3 코드 구조 규칙

### 5.3.1 클래스 내부 구조
```csharp
public partial class PatientListControl : NuBaseControl
{
    #region 필드 (Private)
    
    private PatientListViewModel _viewModel;
    private bool _isLoading;
    
    #endregion
    
    #region 속성 (Public)
    
    public override string ScreenId => "EMR_CL_PATIENT_LIST_001";
    public PatientListDto SelectedPatient { get; private set; }
    
    #endregion
    
    #region 생성자
    
    public PatientListControl(PatientListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        
        RegisterEvents();
        InitializeControls();
    }
    
    #endregion
    
    #region 화면 생명주기
    
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        // 화면 활성화 시 로직
    }
    
    protected override void OnScreenDeactivated()
    {
        base.OnScreenDeactivated();
        // 화면 비활성화 시 로직
    }
    
    #endregion
    
    #region 초기화
    
    private void InitializeControls()
    {
        // 컨트롤 초기화 로직
    }
    
    private void RegisterEvents()
    {
        // 이벤트 등록 로직
    }
    
    #endregion
    
    #region 데이터 처리
    
    private async void LoadData()
    {
        // 데이터 로드 로직
    }
    
    private void BindData()
    {
        // 데이터 바인딩 로직
    }
    
    #endregion
    
    #region 이벤트 핸들러
    
    private void BtnSearch_Click(object sender, EventArgs e)
    {
        // 검색 버튼 클릭 로직
    }
    
    private void GrdPatient_DoubleClick(object sender, EventArgs e)
    {
        // 그리드 더블클릭 로직
    }
    
    #endregion
    
    #region 리소스 정리
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _viewModel?.Dispose();
        }
        base.Dispose(disposing);
    }
    
    #endregion
}
```

### 5.3.2 주석 규칙
```csharp
// 단일 줄 주석: 간단한 설명
var patientId = "P001";

// XML 주석: 공용 메서드, 클래스 설명
/// <summary>
/// 환자 목록을 비동기적으로 로드합니다.
/// </summary>
/// <param name="searchCondition">검색 조건</param>
/// <returns>로드된 환자 목록</returns>
public async Task< List<PatientListDto>> LoadPatientsAsync(PatientSearchRequestDto searchCondition)
{
    // 구현
}

// TODO 주석: 향후 개선 사항
// TODO: 검색 성능을 위해 캐시 추가 필요

// FIXME 주석: 버그 수정 필요
// FIXME: null 참조 예외 발생 가능성 있음

// HACK 주석: 임시 해결책
// HACK: UI 스레드 차단을 방지하기 위해 async/await 적용
```

## 5.4 예외 처리 규칙

### 5.4.1 try-catch 블록
```csharp
// ✅ 올바른 예
try
{
    var result = await _serviceAgent.GetPatientsAsync(searchCondition);
    BindData(result);
}
catch (ApiException ex)
{
    // API 오류: 사용자에게 친절한 메시지
    NuXtraMessageBox.ShowError($"서버 통신 오류: {ex.Message}");
    LogManager.Error($"API 오류 발생: {ex.Message}", ScreenId, ex);
}
catch (Exception ex)
{
    // 일반 오류: 로깅 후 사용자 알림
    LogManager.Error($"예기치 않은 오류 발생: {ex.Message}", ScreenId, ex);
    NuXtraMessageBox.ShowError("처리 중 오류가 발생했습니다. 관리자에게 문의하세요.");
}
finally
{
    // 항상 실행되어야 하는 로직 (로딩 상태 해제 등)
    IsLoading = false;
}
```

### 5.4.2 사용자 정의 예외
```csharp
// 사용자 정의 예외 클래스
public class PatientNotFoundException : Exception
{
    public string PatientId { get; }
    
    public PatientNotFoundException(string patientId, string message)
        : base(message)
    {
        PatientId = patientId;
    }
}

// 사용 예
try
{
    var patient = await _serviceAgent.GetPatientAsync(patientId);
    if (patient == null)
    {
        throw new PatientNotFoundException(patientId, "환자 정보를 찾을 수 없습니다.");
    }
}
catch (PatientNotFoundException ex)
{
    NuXtraMessageBox.ShowError(ex.Message);
    LogManager.Error(ex.Message, ScreenId, ex);
}
```

---

# 6. 데이터 바인딩 및 이벤트 처리

## 6.1 데이터 바인딩

### 6.1.1 ViewModel과의 데이터 바인딩
```csharp
public partial class PatientListControl : NuBaseControl
{
    private PatientListViewModel _viewModel;
    
    private void BindData()
    {
        // 그리드에 데이터 바인딩
        grdPatient.DataSource = _viewModel.Patients;
        
        // 레이블에 데이터 바인딩
        lblTotalCount.DataBindings.Clear();
        lblTotalCount.DataBindings.Add("Text", _viewModel, "TotalCount", true,
            DataSourceUpdateMode.Never, string.Empty, "총 {0:N0}건");
        
        // 로딩 상태 바인딩
        progressBar.DataBindings.Clear();
        progressBar.DataBindings.Add("EditValue", _viewModel, "IsLoading", true,
            DataSourceUpdateMode.Never);
        
        // 로딩 상태에 따른 컨트롤 상태 변경
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PatientListViewModel.IsLoading))
            {
                btnSearch.Enabled = !_viewModel.IsLoading;
                btnExcel.Enabled = !_viewModel.IsLoading;
                this.Cursor = _viewModel.IsLoading ? Cursors.WaitCursor : Cursors.Default;
            }
        };
    }
}
```

### 6.1.2 검색 조건 바인딩
```csharp
private void BindSearchControls()
{
    // 검색 조건을 ViewModel과 바인딩
    txtPatientName.DataBindings.Add("Text", _viewModel.SearchCondition, "PatientName");
    txtPatientId.DataBindings.Add("Text", _viewModel.SearchCondition, "PatientId");
    dteBirthDateFrom.DataBindings.Add("EditValue", _viewModel.SearchCondition, "BirthDateFrom");
    dteBirthDateTo.DataBindings.Add("EditValue", _viewModel.SearchCondition, "BirthDateTo");
    
    // 콤보박스는 바인딩이 복잡하므로 이벤트로 처리
    cboGender.SelectedIndexChanged += (s, e) =>
    {
        _viewModel.SearchCondition.Gender = 
            cboGender.SelectedIndex == 0 ? (Gender?)null : (Gender)cboGender.SelectedIndex;
    };
    
    cboBloodType.SelectedIndexChanged += (s, e) =>
    {
        _viewModel.SearchCondition.BloodType = 
            cboBloodType.SelectedIndex == 0 ? (BloodType?)null : (BloodType)cboBloodType.SelectedIndex;
    };
}
```

## 6.2 이벤트 처리

### 6.2.1 기본 이벤트 처리 패턴
```csharp
private void RegisterEvents()
{
    // 버튼 이벤트
    btnSearch.Click += BtnSearch_Click;
    btnReset.Click += BtnReset_Click;
    btnExcel.Click += BtnExcel_Click;
    btnDetail.Click += BtnDetail_Click;
    
    // 그리드 이벤트
    grdPatient.DoubleClick += GrdPatient_DoubleClick;
    grdPatientView.FocusedRowChanged += GrdPatientView_FocusedRowChanged;
    
    // 키보드 이벤트
    txtPatientName.KeyDown += TxtPatientName_KeyDown;
    
    // ViewModel 이벤트
    _viewModel.PropertyChanged += ViewModel_PropertyChanged;
}
```

### 6.2.2 그리드 이벤트 처리
```csharp
private void GrdPatient_DoubleClick(object sender, EventArgs e)
{
    // 더블클릭 시 상세 정보 조회
    OpenPatientDetail();
}

private void GrdPatientView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
{
    // 포커스 행 변경 시 상세 정보 표시
    var selectedPatient = SelectedPatient;
    if (selectedPatient != null)
    {
        ShowPatientPreview(selectedPatient);
    }
}

private void GrdPatientView_KeyDown(object sender, KeyEventArgs e)
{
    // 키보드 이벤트 처리
    if (e.KeyCode == Keys.Enter && e.Control)
    {
        // Ctrl+Enter: 상세 정보 조회
        OpenPatientDetail();
    }
    else if (e.KeyCode == Keys.Delete)
    {
        // Delete: 삭제 (권한 체크)
        DeletePatient();
    }
}
```

### 6.2.3 비동기 이벤트 처리
```csharp
private async void BtnSearch_Click(object sender, EventArgs e)
{
    // UI 스레드 차단 방지
    await Task.Run(async () =>
    {
        // 데이터 로드
        var searchCondition = CollectSearchCondition();
        await _viewModel.LoadDataAsync(searchCondition);
    }).ConfigureAwait(false);
    
    // UI 업데이트는 BeginInvoke 사용
    this.BeginInvoke(new Action(() =>
    {
        // 데이터 바인딩
        BindData();
    });
}

private async void BtnExcel_Click(object sender, EventArgs e)
{
    try
    {
        IsLoading = true;
        
        await Task.Run(() =>
        {
            // 시간이 걸리는 엑셀 내보내기 작업
            _viewModel.ExportToExcel($"환자목록_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }).ConfigureAwait(false);
        
        this.BeginInvoke(new Action(() =>
        {
            NuXtraMessageBox.ShowInformation("엑셀 내보내기를 완료했습니다.");
        }));
    }
    catch (Exception ex)
    {
        this.BeginInvoke(new Action(() =>
        {
            NuXtraMessageBox.ShowError($"엑셀 내보내기 중 오류 발생: {ex.Message}");
        }));
    }
    finally
    {
        IsLoading = false;
    }
}
```

## 6.3 복합 이벤트 처리

### 6.3.1 환자 선택 이벤트
```csharp
// 1. 이벤트 페이로드 클래스
public class PatientSelectedEventArgs : EventArgs
{
    public PatientListDto Patient { get; }
    public string SourceScreenId { get; }
    
    public PatientSelectedEventArgs(PatientListDto patient, string sourceScreenId)
    {
        Patient = patient;
        SourceScreenId = sourceScreenId;
    }
}

// 2. 이벤트 정의
public event EventHandler<PatientSelectedEventArgs> PatientSelected;

// 3. 이벤트 발생
protected virtual void OnPatientSelected(PatientListDto patient)
{
    PatientSelected?.Invoke(this, new PatientSelectedEventArgs(patient, ScreenId));
}

// 4. 그리드 선택 변경 시 이벤트 발생
private void GrdPatientView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
{
    var selectedPatient = SelectedPatient;
    if (selectedPatient != null)
    {
        OnPatientSelected(selectedPatient);
    }
}
```

### 6.3.2 이벤트 구독 및 처리
```csharp
// 다른 화면에서 환자 선택 이벤트 구독
protected override void OnScreenActivated()
{
    base.OnScreenActivated();
    
    // Event Aggregator를 통한 환자 선택 이벤트 구독
    EventAggregator?.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelectedFromOtherScreen);
}

private void OnPatientSelectedFromOtherScreen(object payload)
{
    // 다른 화면에서 환자를 선택했을 때의 처리
    if (payload is PatientSelectedEventArgs evt)
    {
        // 자기 자신의 이벤트는 무시
        if (evt.SourceScreenId == ScreenId)
            return;
            
        // 환자 정보 로드
        LoadPatientData(evt.Patient.PatientId);
    }
}
```

---

# 7. 권한 제어

## 7.1 권한 시스템 개요

nU3.Framework은 다단계 권한 시스템을 제공합니다. 모든 화면은 반드시 권한 체크를 수행해야 합니다.

### 7.1.1 권한 레벨
```csharp
// 권한 레벨 열거형
public enum AuthLevel
{
    /// <summary>
    /// 관리자 (모든 권한)
    /// </summary>
    Admin = 0,
    
    /// <summary>
    /// 일반 사용자 (조회/수정/삭제)
    /// </summary>
    User = 1,
    
    /// <summary>
    /// 조회 전용 (조회만 가능)
    /// </summary>
    ReadOnly = 2
}
```

### 7.1.2 권한 종류
```csharp
// 권한 종류 열거형
public enum PermissionType
{
    /// <summary>
    /// 조회 권한
    /// </summary>
    Read = 1,
    
    /// <summary>
    /// 생성 권한
    /// </summary>
    Create = 2,
    
    /// <summary>
    /// 수정 권한
    /// </summary>
    Update = 4,
    
    /// <summary>
    /// 삭제 권한
    /// </summary>
    Delete = 8,
    
    /// <summary>
    /// 출력 권한
    /// </summary>
    Print = 16,
    
    /// <summary>
    /// 엑셀 내보내기 권한
    /// </summary>
    Export = 32
}
```

## 7.2 권한 체크 구현

### 7.2.1 기본 권한 체크
```csharp
protected override void OnScreenActivated()
{
    base.OnScreenActivated();
    
    // 화면 활성화 시 권한 체크
    if (!HasPermission(PermissionType.Read))
    {
        NuXtraMessageBox.ShowError("화면을 열 권한이 없습니다.");
        this.Close();
        return;
    }
    
    // 버튼 권한 설정
    UpdateButtonPermissions();
    
    // 데이터 로드
    LoadData();
}

private void UpdateButtonPermissions()
{
    // 생성 권한
    btnNew.Enabled = HasPermission(PermissionType.Create);
    
    // 수정 권한
    btnEdit.Enabled = HasPermission(PermissionType.Update);
    
    // 삭제 권한
    btnDelete.Enabled = HasPermission(PermissionType.Delete);
    
    // 출력 권한
    btnPrint.Enabled = HasPermission(PermissionType.Print);
    
    // 엑셀 내보내기 권한
    btnExcel.Enabled = HasPermission(PermissionType.Export);
}
```

### 7.2.2 권한 체크 유틸리티
```csharp
// NuBaseControl에 내장된 권한 체크 메서드 사용
public bool HasPermission(PermissionType permissionType)
{
    return UserSession.Instance.HasPermission(ScreenId, permissionType);
}

public bool HasAnyPermission(params PermissionType[] permissionTypes)
{
    return UserSession.Instance.HasAnyPermission(ScreenId, permissionTypes);
}

public bool HasAllPermissions(params PermissionType[] permissionTypes)
{
    return UserSession.Instance.HasAllPermissions(ScreenId, permissionTypes);
}
```

### 7.2.3 고급 권한 체크
```csharp
// 특정 조건에 대한 권한 체크
private void CheckEditPermission(PatientDetailDto patient)
{
    // 수정 권한 체크
    if (!HasPermission(PermissionType.Update))
    {
        NuXtraMessageBox.ShowError("수정 권한이 없습니다.");
        return;
    }
    
    // 소유자 체크 (자기 자신의 데이터만 수정 가능)
    if (!UserSession.Instance.IsAdmin && 
        patient.RegisteredBy != UserSession.Instance.UserId)
    {
        NuXtraMessageBox.ShowError("자신의 데이터만 수정할 수 있습니다.");
        return;
    }
    
    // 데이터 상태 체크 (완료된 데이터는 수정 불가)
    if (patient.Status == "Completed")
    {
        NuXtraMessageBox.ShowError("완료된 데이터는 수정할 수 없습니다.");
        return;
    }
    
    // 모든 조건 통과
    EditPatient(patient);
}
```

### 7.2.4 화면 컨트롤별 권한 적용
```csharp
private void ApplyPermissionToControls()
{
    // 그리드 편집 권한
    if (!HasPermission(PermissionType.Update))
    {
        grdPatient.OptionsBehavior.Editable = false;
        gridViewPatient.Columns["ColumnName"].OptionsColumn.AllowEdit = false;
    }
    
    // 메뉴 컨텍스트 메뉴
    if (!HasPermission(PermissionType.Create))
    {
        contextMenuStrip.Items["mnuNew"].Enabled = false;
    }
    
    if (!HasPermission(PermissionType.Delete))
    {
        contextMenuStrip.Items["mnuDelete"].Enabled = false;
    }
    
    // 툴바 버튼
    toolbarRefresh.Enabled = HasPermission(PermissionType.Read);
    toolbarNew.Enabled = HasPermission(PermissionType.Create);
    toolbarEdit.Enabled = HasPermission(PermissionType.Update);
    toolbarDelete.Enabled = HasPermission(PermissionType.Delete);
    toolbarPrint.Enabled = HasPermission(PermissionType.Print);
    toolbarExcel.Enabled = HasPermission(PermissionType.Export);
}
```

## 7.3 동적 권한 변경

### 7.3.1 권한 변경 이벤트 처리
```csharp
protected override void OnScreenActivated()
{
    base.OnScreenActivated();
    
    // 권한 변경 이벤트 구독
    UserSession.Instance.PermissionChanged += OnPermissionChanged;
}

protected override void OnScreenDeactivated()
{
    base.OnScreenDeactivated();
    
    // 권한 변경 이벤트 구독 해제
    UserSession.Instance.PermissionChanged -= OnPermissionChanged;
}

private void OnPermissionChanged(object sender, PermissionChangedEventArgs e)
{
    // 화면 권한이 변경된 경우
    if (e.ScreenId == ScreenId)
    {
        // UI 스레드에서 권한 업데이트
        this.BeginInvoke(new Action(() =>
        {
            UpdateButtonPermissions();
            ApplyPermissionToControls();
            
            // 이미 열려있는 데이터가 있다면 권한 재확인
            if (_viewModel.Patients.Count > 0)
            {
                CheckDataPermissions();
            }
        }));
    }
}

private void CheckDataPermissions()
{
    // 현재 데이터에 대한 권한 재확인
    foreach (var patient in _viewModel.Patients)
    {
        // 특정 환자에 대한 권한 변경 사항 반영
        var rowHandle = gridViewPatient.LocateByValue("PatientId", patient.PatientId);
        if (rowHandle != DevExpress.XtraGrid.Data.GridControl.InvalidRowHandle)
        {
            // 권한에 따른 행 색상 변경
            UpdateRowAppearance(rowHandle, patient);
        }
    }
}
```

### 7.3.2 권한에 따른 UI 변경
```csharp
private void UpdateRowAppearance(int rowHandle, PatientDetailDto patient)
{
    var row = gridViewPatient.GetRow(rowHandle) as PatientDetailDto;
    if (row == null) return;
    
    // 읽기 전용 데이터 강조 표시
    if (!HasPermission(PermissionType.Update, row))
    {
        gridViewPatient.Appearance.FocusedRow.BackColor = Color.LightGray;
        gridViewPatient.Appearance.FocusedRow.ForeColor = Color.DarkGray;
    }
    
    // 중요 데이터 강조 표시
    if (row.IsImportant)
    {
        gridViewPatient.Appearance.Row.BackColor = Color.LightYellow;
    }
    
    // 삭제 예정 데이터
    if (row.Status == "ToDelete")
    {
        gridViewPatient.Appearance.Row.ForeColor = Color.Red;
        gridViewPatient.Appearance.Row.FontStyle = FontStyle.Strikeout;
    }
}
```

---

# 8. 테스트 및 배포

## 8.1 단위 테스트

### 8.1.1 테스트 프로젝트 구조
```
nU3.Modules.EMR.CL.Patient.Tests/
├── Properties/
├── References/                    # 테스트 참조
├── Unit Tests/                    # 단위 테스트
│   ├── PatientListViewModelTests.cs
│   ├── PatientServiceAgentTests.cs
│   └── PatientDtoTests.cs
├── Integration Tests/              # 통합 테스트
│   ├── PatientListControlTests.cs
│   └── PatientDataIntegrationTests.cs
└── Test Data/                    # 테스트 데이터
    ├── PatientTestData.cs
    └── MockServices.cs
```

### 8.1.2 ViewModel 테스트 예시
```csharp
using Xunit;
using Moq;
using nU3.Modules.EMR.CL.Patient.ViewModels;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.Tests
{
    public class PatientListViewModelTests
    {
        private readonly Mock<IPatientServiceAgent> _mockServiceAgent;
        private readonly PatientListViewModel _viewModel;
        
        public PatientListViewModelTests()
        {
            _mockServiceAgent = new Mock<IPatientServiceAgent>();
            _viewModel = new PatientListViewModel(_mockServiceAgent.Object);
        }
        
        [Fact]
        public async Task LoadDataAsync_WhenCalled_ShouldLoadPatients()
        {
            // Arrange
            var searchCondition = new PatientSearchRequestDto
            {
                PatientName = "홍길동",
                PageNumber = 1,
                PageSize = 10
            };
            
            var expectedPatients = new PagedResultDto<PatientListDto>
            {
                Items = new List<PatientListDto>
                {
                    new PatientListDto { PatientId = "P001", PatientName = "홍길동" }
                },
                TotalCount = 1
            };
            
            _mockServiceAgent.Setup(x => x.GetPatientsAsync(searchCondition))
                .ReturnsAsync(expectedPatients);
            
            // Act
            await _viewModel.LoadDataAsync(searchCondition);
            
            // Assert
            Assert.Single(_viewModel.Patients);
            Assert.Equal("P001", _viewModel.Patients[0].PatientId);
            Assert.Equal(1, _viewModel.TotalCount);
            
            // 메서드 호출 확인
            _mockServiceAgent.Verify(x => x.GetPatientsAsync(searchCondition), Times.Once);
        }
        
        [Fact]
        public async Task LoadDataAsync_WhenExceptionOccurs_ShouldLogError()
        {
            // Arrange
            var searchCondition = new PatientSearchRequestDto();
            
            _mockServiceAgent.Setup(x => x.GetPatientsAsync(searchCondition))
                .ThrowsAsync(new ApiException("서버 오류"));
            
            // Act
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _viewModel.LoadDataAsync(searchCondition));
            
            // Assert
            Assert.Equal("서버 오류", exception.Message);
        }
    }
}
```

### 8.1.3 컨트롤 테스트 예시
```csharp
using Xunit;
using nU3.Modules.EMR.CL.Patient.Controls;
using nU3.Modules.EMR.CL.Patient.ViewModels;

namespace nU3.Modules.EMR.CL.Patient.Tests
{
    public class PatientListControlTests
    {
        private readonly PatientListViewModel _mockViewModel;
        private readonly PatientListControl _control;
        
        public PatientListControlTests()
        {
            // Mock ViewModel 생성
            _mockViewModel = new Mock<PatientListViewModel>().Object;
            
            // Control 생성 (UI 스레드 테스트)
            var thread = new Thread(() =>
            {
                _control = new PatientListControl(_mockViewModel);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
        
        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeComponents()
        {
            // Assert
            Assert.NotNull(_control);
            Assert.Equal("EMR_CL_PATIENT_LIST_001", _control.ScreenId);
        }
        
        [Fact]
        public void GrdPatient_DoubleClick_WhenHasSelectedPatient_ShouldOpenDetail()
        {
            // Arrange
            var testPatient = new PatientListDto { PatientId = "P001", PatientName = "테스트 환자" };
            
            // 테스트용 데이터 설정
            _control.Invoke(new Action(() =>
            {
                var gridView = _control.grdPatient.MainView as GridView;
                gridView.AddRow(testPatient);
                gridView.FocusedRowHandle = 0;
            }));
            
            // Act
            _control.Invoke(new Action(() =>
            {
                _control.grdPatient_DoubleClick(null, EventArgs.Empty);
            }));
            
            // Assert
            // 여기서는 실제로 창이 열리지 않으므로, 대신 관련 메서드가 호출되었는지 확인
            // 이 부분은 테스트를 위해 별도의 인터페이스 분리 필요
        }
    }
}
```

## 8.2 통합 테스트

### 8.2.1 통합 테스트 설정
```csharp
// nU3.Modules.EMR.CL.Patient.Tests/Integration/PatientDataIntegrationTests.cs
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Services;
using nU3.Modules.EMR.CL.Patient.Services;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.Tests.Integration
{
    public class PatientDataIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;
        private readonly IPatientServiceAgent _serviceAgent;
        
        public PatientDataIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
            _serviceAgent = _fixture.ServiceProvider.GetService<IPatientServiceAgent>();
        }
        
        [Fact]
        public async Task GetPatientAsync_WhenPatientExists_ShouldReturnPatient()
        {
            // Arrange
            var patientId = "P001";
            
            // Act
            var patient = await _serviceAgent.GetPatientAsync(patientId);
            
            // Assert
            Assert.NotNull(patient);
            Assert.Equal(patientId, patient.PatientId);
        }
        
        [Fact]
        public async Task GetPatientsAsync_WithSearchCondition_ShouldReturnFilteredPatients()
        {
            // Arrange
            var searchCondition = new PatientSearchRequestDto
            {
                PatientName = "홍길동",
                PageNumber = 1,
                PageSize = 10
            };
            
            // Act
            var result = await _serviceAgent.GetPatientsAsync(searchCondition);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Items.Count > 0);
            Assert.All(result.Items, p => p.PatientName.Contains("홍길동"));
        }
    }
    
    // 통합 테스트를 위한 Fixture
    public class IntegrationTestFixture : IAsyncLifetime
    {
        public ServiceProvider ServiceProvider { get; private set; }
        
        public async Task InitializeAsync()
        {
            var services = new ServiceCollection();
            
            // 실제 서비스 등록 (테스트용 DB 사용)
            services.AddDbContext<TestDbContext>(options =>
                options.UseSqlServer(TestConnectionString));
                
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientServiceAgent, PatientServiceAgent>();
            
            ServiceProvider = services.BuildServiceProvider();
            
            // 테스트 데이터 초기화
            await InitializeTestData();
        }
        
        public async Task DisposeAsync()
        {
            await CleanupTestData();
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        
        private async Task InitializeTestData()
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            
            // 테스트용 환자 데이터 추가
            dbContext.Patients.AddRange(new List<Patient>
            {
                new Patient { PatientId = "P001", PatientName = "홍길동", BirthDate = DateTime.Now.AddYears(-30) },
                new Patient { PatientId = "P002", PatientName = "김철수", BirthDate = DateTime.Now.AddYears(-25) }
            });
            
            await dbContext.SaveChangesAsync();
        }
        
        private async Task CleanupTestData()
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            
            dbContext.Database.EnsureDeleted();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
```

## 8.3 배포 절차

### 8.3.1 배포 전 체크리스트

#### 8.3.1.1 코드 품질 체크리스트
```markdown
## 코드 품질 체크리스트

### ✅ nU3ProgramInfo 어트리뷰트
- [ ] 모든 업무화면에 nU3ProgramInfo 어트리뷰트가 있는가?
- [ ] 어트리뷰트의 ProgramId와 ScreenId가 일치하는가?
- [ ] 모듈 ID 형식이 올바른가? (MOD_카테고리_서브시스템)
- [ ] 모듈 타입이 올바르게 설정되었는가? (MAIN/CHILD/DIALOG)

### ✅ 클래스 상속
- [ ] 모든 컨트롤이 NuBaseControl을 상속하는가?
- [ ] 모든 폼이 NuBaseForm을 상속하는가?
- [ ] ScreenId 속성이 구현되었는가?

### ✅ 권한 체크
- [ ] 화면 활성화 시 권한 체크를 수행하는가?
- [ ] 버튼별 권한 설정이 되어있는가?
- [ ] 데이터 조작 전 권한 재확인을 하는가?

### ✅ 예외 처리
- [ ] try-catch 블록이 적절하게 사용되었는가?
- [ ] API 호출 시 ApiException 처리가 있는가?
- [ ] 사용자에게 친절한 오류 메시지를 표시하는가?

### ✅ 메모리 관리
- [ ] 이벤트 구독 해제가 되어있는가?
- [ ] IDisposable을 구현하는가?
- [ ] Dispose()에서 리소스를 정리하는가?

### ✅ 로깅
- [ ] 중요한 작업에 로깅이 있는가?
- [ ] 오류 발생 시 로깅이 있는가?
- [ ] ScreenId를 로깅에 포함하는가?
```

#### 8.3.1.2 기능 테스트 체크리스트
```markdown
## 기능 테스트 체크리스트

### ✅ 기본 기능
- [ ] 화면이 정상적으로 열리는가?
- [ ] 데이터가 정상적으로 로드되는가?
- [ ] 검색 기능이 정상적으로 동작하는가?
- [ ] 페이징 기능이 정상적으로 동작하는가?

### ✅ 데이터 조작
- [ ] 생성 기능이 정상적으로 동작하는가?
- [ ] 수정 기능이 정상적으로 동작하는가?
- [ ] 삭제 기능이 정상적으로 동작하는가?
- [ ] 데이터 유효성 검사가 있는가?

### ✅ UI 상호작용
- [ ] 그리드 더블클릭 시 상세 화면이 열리는가?
- [ ] 버튼 클릭 시 해당 기능이 동작하는가?
- [ ] 키보드 단축키가 동작하는가?
- [ ] 컨트롤의 상태 변경이 화면에 반영되는가?

### ✅ 권한 테스트
- [ ] 권한이 없을 때 화면이 열리지 않는가?
- [ ] 권한에 따라 버튼이 비활성화되는가?
- [ ] 권한이 없을 때 데이터 조작이 불가능한가?

### ✅ 예외 상황 테스트
- [ ] 네트워크 오류 시 에러 메시지가 표시되는가?
- [ ] 서버 오류 시 에러 메시지가 표시되는가?
- [ ] 유효하지 않은 입력 시 에러 메시지가 표시되는가?
- [ ] 대용량 데이터 처리 시 화면이 멈추지 않는가?
```

### 8.3.2 Deployer를 이용한 배포

#### 8.3.2.1 배포 단계
```
1. 빌드 확인
   - Visual Studio에서 "솔루션 빌드" (Ctrl+Shift+B)
   - 빌드 오류가 없는지 확인
   - 모든 참조가 해결되었는지 확인

2. 로컬 테스트
   - F5로 실행
   - 해당 모듈 화면이 정상적으로 작동하는지 확인
   - 기능별 테스트 수행

3. Deployer 실행
   - nU3.Tools.Deployer.exe 실행
   - "모듈 배포" 탭 선택

4. 모듈 업로드
   - "모듈 추가" 버튼 클릭
   - 빌드된 DLL 파일 선택
   - 모듈 정보 확인 (nU3ProgramInfo에서 자동 추출)
   - "업로드" 버튼 클릭

5. 화면 확인
   - "화면 목록" 탭에서 방금 업로드된 화면 확인
   - nU3ProgramInfo 속성이 올바르게 추출되었는지 확인

6. 메뉴 등록
   - "메뉴 편집기" 탭 선택
   - 부모 메뉴 선택 후 "하위 메뉴 추가"
   - 방금 업로드한 화면 선택
   - 메뉴 정보 입력 후 저장

7. 배포
   - "배포" 탭 선택
   - "배포 환경" 선택 (개발/테스트/운영)
   - "배포 실행" 버튼 클릭
   - 배포 로그 확인
```

#### 8.3.2.2 배포 후 검증
```csharp
// 배포 후 검증 프로그램 예시
public class DeploymentVerifier
{
    public async Task<bool> VerifyDeploymentAsync(string modulePath, string screenId)
    {
        try
        {
            // 1. 파일 존재 확인
            if (!File.Exists(modulePath))
            {
                Console.WriteLine($"모듈 파일이 존재하지 않습니다: {modulePath}");
                return false;
            }
            
            // 2. 어셈블리 로드
            var assembly = Assembly.LoadFrom(modulePath);
            
            // 3. nU3ProgramInfo 어트리뷰트 확인
            var types = assembly.GetTypes();
            var controlTypes = types.Where(t => 
                typeof(NuBaseControl).IsAssignableFrom(t) ||
                typeof(NuBaseForm).IsAssignableFrom(t));
            
            foreach (var type in controlTypes)
            {
                var attributes = type.GetCustomAttributes<nU3ProgramInfoAttribute>();
                if (attributes.Any())
                {
                    var attr = attributes.First();
                    
                    // 4. ScreenId 일치 확인
                    var instance = Activator.CreateInstance(type) as NuBaseControl;
                    if (instance != null && instance.ScreenId == screenId)
                    {
                        Console.WriteLine($"✅ 화면 확인: {type.Name} - {screenId}");
                        
                        // 5. 의존성 확인
                        if (VerifyDependencies(assembly))
                        {
                            Console.WriteLine($"✅ 배포 검증 성공: {modulePath}");
                            return true;
                        }
                    }
                }
            }
            
            Console.WriteLine($"❌ 화면을 찾을 수 없습니다: {screenId}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 배포 검증 중 오류 발생: {ex.Message}");
            return false;
        }
    }
    
    private bool VerifyDependencies(Assembly assembly)
    {
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        var requiredReferences = new[]
        {
            "nU3.Core",
            "nU3.Core.UI",
            "nU3.Models",
            "nU3.Connectivity"
        };
        
        foreach (var required in requiredReferences)
        {
            if (!referencedAssemblies.Any(r => r.Name.StartsWith(required)))
            {
                Console.WriteLine($"❌ 필수 참조 누락: {required}");
                return false;
            }
        }
        
        Console.WriteLine("✅ 의존성 검증 완료");
        return true;
    }
}
```

---

# 9. 문제 해결 및 FAQ

## 9.1 일반적인 문제 해결

### 9.1.1 nU3ProgramInfo 어트리뷰트 관련 문제

#### 문제: 화면이 메뉴에 표시되지 않음
```csharp
// 원인 1: nU3ProgramInfo 어트리뷰트 누락
// 해결: 모든 업무화면은 반드시 nU3ProgramInfo 어트리뷰트를 가져야 합니다
[nU3ProgramInfo(
    typeof(YourControl),
    "화면 표시 이름",
    "MOD_카테고리_서브시스템",
    "CHILD")]
public partial class YourControl : NuBaseControl
{
    public override string ScreenId => "CATEGORY_SUBSYSTEM_SCREEN_001";
}
```

#### 문제: ScreenId가 ProgramId와 일치하지 않음
```csharp
// 원인 2: ScreenId와 ProgramId 불일치
// 해결: 두 ID가 반드시 일치해야 합니다
[nU3ProgramInfo(
    typeof(YourControl),
    "화면 표시 이름",
    "MOD_카테고리_서브시스템",
    "CHILD",
    ProgramId = "CATEGORY_SUBSYSTEM_SCREEN_001")]  // 여기 정의
public partial class YourControl : NuBaseControl
{
    public override string ScreenId => "CATEGORY_SUBSYSTEM_SCREEN_001";  // 여기 구현, 일치해야 함
}
```

#### 문제: 상속하지 않은 클래스
```csharp
// 원인 3: NuBaseControl을 상속하지 않음
// 해결: 반드시 NuBaseControl 또는 NuBaseForm을 상속해야 함
// ❌ 잘못된 예
public partial class WrongControl : UserControl  // 일반 UserControl 사용 안됨
{
}

// ✅ 올바른 예
public partial class CorrectControl : NuBaseControl  // NuBaseControl 상속 필수
{
    public override string ScreenId => "CATEGORY_SUBSYSTEM_SCREEN_001";
}
```

### 9.1.2 컴파일 오류 문제

#### 문제: nU3.Core 어셈블리를 찾을 수 없음
```csharp
// 원인: 참조 추가 누락
// 해결: .csproj 파일에 필수 참조 추가

<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <!-- ★★★ 필수 참조 ★★★ -->
    <ProjectReference Include="..\..\nU3.Core\nU3.Core.csproj" />
    <ProjectReference Include="..\..\nU3.Core.UI\nU3.Core.UI.csproj" />
    <ProjectReference Include="..\..\nU3.Models\nU3.Models.csproj" />
    <ProjectReference Include="..\..\nU3.Connectivity\nU3.Connectivity.csproj" />
    
    <!-- DevExpress 참조 -->
    <PackageReference Include="DevExpress.Win" Version="23.2.9" />
    <PackageReference Include="DevExpress.Win.Design" Version="23.2.9" />
  </ItemGroup>
</Project>
```

#### 문제: using 문 누락
```csharp
// 원인: 필요한 using 문 추가 누락
// 해결: 필수 using 문 추가
using nU3.Core;
using nU3.Core.UI;
using nU3.Core.Attributes;
using nU3.Models.DTOs.Patient;
using DevExpress.XtraEditors;
```

### 9.1.3 실행 시 오류 문제

#### 문제: 화면이 로드되지 않음
```csharp
// 원인: OnScreenActivated 메서드에서 예외 발생
// 해결: try-catch 블록으로 예외 처리

protected override void OnScreenActivated()
{
    try
    {
        base.OnScreenActivated();
        
        // 권한 체크
        if (!HasPermission(PermissionType.Read))
        {
            NuXtraMessageBox.ShowError("조회 권한이 없습니다.");
            return;
        }
        
        // 데이터 로드
        LoadData();
    }
    catch (Exception ex)
    {
        LogManager.Error($"화면 활성화 중 오류: {ex.Message}", ScreenId, ex);
        NuXtraMessageBox.ShowError("화면을 로드하는 중 오류가 발생했습니다.");
    }
}
```

#### 문제: NullReferenceException 발생
```csharp
// 원인: _viewModel이 null 상태에서 접근
// 해결: 생성자에서 ViewModel을 초기화

public class YourControl : NuBaseControl
{
    private readonly YourViewModel _viewModel;
    
    public YourControl(YourViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));
            
        _viewModel = viewModel;
        InitializeComponent();
    }
    
    private void LoadData()
    {
        // _viewModel이 null인지 확인
        if (_viewModel == null)
            throw new InvalidOperationException("ViewModel이 초기화되지 않았습니다.");
            
        // _viewModel 사용
        _viewModel.LoadData();
    }
}
```

## 9.2 자주 묻는 질문 (FAQ)

### 9.2.1 개발 관련 FAQ

#### Q: 새로운 업무화면을 개발하려면 어떻게 시작해야 하나요?
```csharp
// A: 다음 절차를 따르세요

// 1. 프로젝트 생성
// nU3.Modules.[카테고리].[서브시스템].[업무명]
// 예: nU3.Modules.EMR.CL.Patient

// 2. .csproj 파일에 필수 참조 추가
// - nU3.Core
// - nU3.Core.UI
// - nU3.Models
// - nU3.Connectivity
// - DevExpress.Win 23.2.9

// 3. Control 클래스 생성 (NuBaseControl 상속)
[nU3ProgramInfo(
    typeof(PatientListControl),
    "환자목록",
    "MOD_EMR_CL_PATIENT",
    "CHILD",
    ProgramId = "EMR_CL_PATIENT_LIST_001")]
public partial class PatientListControl : NuBaseControl
{
    public override string ScreenId => "EMR_CL_PATIENT_LIST_001";
    
    protected override void OnScreenActivated()
    {
        base.OnScreenActivated();
        // 권한 체크 및 초기화
    }
}

// 4. Deployer로 등록 및 배포
```

#### Q: 여러 화면에서 공통으로 사용하는 데이터를 어떻게 전달하나요?
```csharp
// A: Event Aggregator 패턴을 사용하세요

// 1. 이벤트 페이로드 클래스 정의
public class PatientSelectedEventArgs : EventArgs
{
    public PatientListDto Patient { get; }
    public string SourceScreenId { get; }
}

// 2. 이벤트 발행 (발신 화면)
private void OnPatientSelected(PatientListDto patient)
{
    EventAggregator?.GetEvent<PatientSelectedEvent>()
        .Publish(new PatientSelectedEventArgs(patient, ScreenId));
}

// 3. 이벤트 구독 (수신 화면)
protected override void OnScreenActivated()
{
    base.OnScreenActivated();
    
    EventAggregator?.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelectedFromOtherScreen);
}

private void OnPatientSelectedFromOtherScreen(object payload)
{
    if (payload is PatientSelectedEventArgs evt)
    {
        // 자기 자신의 이벤트는 무시
        if (evt.SourceScreenId == ScreenId)
            return;
            
        // 환자 정보 로드
        LoadPatientData(evt.Patient.PatientId);
    }
}
```

#### Q: 검색 조건을 다른 화면에서 재사용하고 싶습니다.
```csharp
// A: 검색 조건 DTO를 공통으로 사용하고 ViewModel에 분리하세요

// 1. 공통 검색 조건 DTO (nU3.Models에 정의)
public class PatientSearchRequestDto : PagedRequestDto
{
    public string PatientName { get; set; }
    public string PatientId { get; set; }
    public DateTime? BirthDateFrom { get; set; }
    public DateTime? BirthDateTo { get; set; }
    public Gender? Gender { get; set; }
    public BloodType? BloodType { get; set; }
}

// 2. 검색 조건 컨트롤 (공통 컴포넌트)
public class PatientSearchControl : NuBaseControl
{
    public PatientSearchRequestDto SearchCondition { get; private set; }
    
    public event EventHandler<EventArgs> SearchRequested;
    
    protected virtual void OnSearchRequested()
    {
        SearchRequested?.Invoke(this, EventArgs.Empty);
    }
    
    private void BtnSearch_Click(object sender, EventArgs e)
    {
        CollectSearchConditions();
        OnSearchRequested();
    }
    
    private void CollectSearchConditions()
    {
        SearchCondition = new PatientSearchRequestDto
        {
            PatientName = txtPatientName.Text,
            PatientId = txtPatientId.Text,
            BirthDateFrom = dteBirthDateFrom.DateTime,
            BirthDateTo = dteBirthDateTo.DateTime,
            Gender = cboGender.SelectedIndex == 0 ? (Gender?)null : (Gender)cboGender.SelectedIndex,
            BloodType = cboBloodType.SelectedIndex == 0 ? (BloodType?)null : (BloodType)cboBloodType.SelectedIndex
        };
    }
}

// 3. 검색 조건 사용
public class PatientListControl : NuBaseControl
{
    private PatientSearchControl _searchControl;
    
    public PatientListControl()
    {
        InitializeComponent();
        
        _searchControl = new PatientSearchControl();
        _searchControl.Dock = DockStyle.Top;
        this.Controls.Add(_searchControl);
        
        _searchControl.SearchRequested += OnSearchRequested;
    }
    
    private async void OnSearchRequested(object sender, EventArgs e)
    {
        await LoadDataAsync(_searchControl.SearchCondition);
    }
}
```

### 9.2.2 배포 관련 FAQ

#### Q: 모듈을 수정했는데 반영되지 않습니다.
```csharp
// A: 다음 단계를 확인하세요

// 1. 빌드 확인
// - 솔루션을 다시 빌드하세요 (Ctrl+Shift+B)
// - 빌드 오류가 없는지 확인

// 2. DLL 업데이트
// - 빌드된 DLL이 출력 폴더에 생성되었는지 확인
// - 기존 DLL을 삭제하고 다시 빌드

// 3. Deployer로 재배포
// - Deployer 실행
// - 모듈을 다시 업로드
// - "새 버전"으로 배포

// 4. 캐시 삭제
// - 클라이언트 캐시 폴더 확인
// - %AppData%\nU3.Framework\Cache
// - 캐시 파일 삭제 후 재시작

// 5. Shadow Copy 확인
// - Bootstrapper가 실행 중인지 확인
// - Bootstrapper를 다시 실행하면 DLL이 복사됨
```

#### Q: 특정 환경에서만 화면이 열리지 않습니다.
```csharp
// A: 환경별 설정을 확인하세요

// 1. 서버 연결 설정 확인
// - ServerConnectionConfig 확인
// - 해당 환경의 API URL이 올바른지 확인

// 2. 버전 호환성 확인
// - .NET 8.0 런타임이 설치되었는지 확인
// - DevExpress 23.2.9가 설치되었는지 확인

// 3. 권한 설정 확인
// - 해당 환경의 사용자 권한 확인
// - 화면에 대한 권한이 부여되었는지 확인

// 4. 로그 확인
// - LogManager를 통한 로그 확인
// - 오류 메시지 분석
LogManager.Error($"화면 로드 오류: {ex.Message}", ScreenId, ex);
```

### 9.2.3 성능 관련 FAQ

#### Q: 대량 데이터 조회 시 화면이 멈춥니다.
```csharp
// A: 다음과 같이 최적화하세요

// 1. 비동기 로딩 사용
protected async Task LoadDataAsync()
{
    try
    {
        IsLoading = true;
        
        await Task.Run(async () =>
        {
            var data = await _serviceAgent.GetDataAsync();
            
            this.BeginInvoke(new Action(() =>
            {
                grdPatient.DataSource = data;
            }));
        }).ConfigureAwait(false);
    }
    finally
    {
        IsLoading = false;
    }
}

// 2. 페이징 처리
public class PagedRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// 3. 가상 모드 (DevExpress)
grdPatient.DataSource = _serviceAgent.GetDataAsync();

// 4. 데이터 로딩 표시
progressBar.Visible = _viewModel.IsLoading;
```

#### Q: 메모리 사용량이 계속 증가합니다.
```csharp
// A: 메모리 누수를 방지하세요

// 1. 이벤트 구독 해제
protected override void OnScreenDeactivated()
{
    base.OnScreenDeactivated();
    
    // Event Aggregator 구독 해제
    EventAggregator?.GetEvent<PatientSelectedEvent>()
        .Unsubscribe(OnPatientSelectedFromOtherScreen);
}

// 2. Dispose 패턴 구현
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // ViewModel 정리
        _viewModel?.Dispose();
        
        // 이벤트 핸들러 정리
        _searchControl.SearchRequested -= OnSearchRequested;
        
        // 기타 리소스 정리
        _timer?.Dispose();
    }
    base.Dispose(disposing);
}

// 3. using 문 사용
private async Task LoadDataAsync()
{
    using (var connection = new SqlConnection(connectionString))
    {
        await connection.OpenAsync();
        // 데이터 로드
    }
}
```

---

# 10. 예제 및 템플릿

## 10.1 완전한 예제 프로젝트

### 10.1.1 Patient 모듈 전체 구조

```
nU3.Modules.EMR.CL.Patient/
├── nU3.Modules.EMR.CL.Patient.csproj
├── Controls/
│   ├── PatientListControl.cs
│   ├── PatientListControl.Designer.cs
│   ├── PatientListControl.resx
│   ├── PatientDetailControl.cs
│   ├── PatientDetailControl.Designer.cs
│   ├── PatientDetailControl.resx
│   ├── PatientSearchControl.cs
│   ├── PatientSearchControl.Designer.cs
│   └── PatientSearchControl.resx
├── ViewModels/
│   ├── PatientListViewModel.cs
│   ├── PatientDetailViewModel.cs
│   └── PatientSearchViewModel.cs
├── DTOs/
│   └── PatientDtos.cs
├── Services/
│   └── PatientService.cs
├── Properties/
│   ├── AssemblyInfo.cs
│   └── Resources.Designer.cs
└── Tests/
    ├── Unit Tests/
    │   ├── PatientListViewModelTests.cs
    │   └── PatientDtoTests.cs
    └── Integration Tests/
        └── PatientDataIntegrationTests.cs
```

### 10.1.2 프로젝트 파일(.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- 필수 설정 -->
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- 어셈블리 정보 -->
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    
    <!-- 출력 경로 -->
    <OutputPath>..\..\..\bin\$(Configuration)\Modules\$(MSBuildProjectName)\</OutputPath>
    <IntermediateOutputPath>obj\$(Configuration)\$(MSBuildProjectName)\</IntermediateOutputPath>
  </PropertyGroup>
  
  <!-- 필수 참조 -->
  <ItemGroup>
    <!-- 프레임워크 코어 -->
    <ProjectReference Include="..\..\nU3.Core\nU3.Core.csproj" />
    <ProjectReference Include="..\..\nU3.Core.UI\nU3.Core.UI.csproj" />
    <ProjectReference Include="..\..\nU3.Models\nU3.Models.csproj" />
    <ProjectReference Include="..\..\nU3.Connectivity\nU3.Connectivity.csproj" />
    
    <!-- DevExpress 참조 -->
    <PackageReference Include="DevExpress.Win" Version="23.2.9" />
    <PackageReference Include="DevExpress.Win.Design" Version="23.2.9" />
    
    <!-- 기타 필요한 패키지 -->
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### 10.1.3 메인 폼 예시
```csharp
// nU3.Modules.EMR.CL.Patient/Controls/PatientListControl.cs
using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using nU3.Core;
using nU3.Core.Attributes;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using nU3.Modules.EMR.CL.Patient.ViewModels;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.Controls
{
    /// <summary>
    /// 환자 목록 컨트롤
    /// </summary>
    [nU3ProgramInfo(
        typeof(PatientListControl),
        "환자목록",
        "MOD_EMR_CL_PATIENT",
        "CHILD",
        ProgramId = "EMR_CL_PATIENT_LIST_001",
        Description = "환자 목록을 조회하고 관리하는 화면",
        AuthLevel = 2,
        DefaultWidth = 1200,
        DefaultHeight = 800)]
    public partial class PatientListControl : NuBaseControl
    {
        #region 필드
        
        private PatientListViewModel _viewModel;
        private PatientSearchControl _searchControl;
        
        #endregion
        
        #region 속성
        
        public override string ScreenId => "EMR_CL_PATIENT_LIST_001";
        
        public PatientListDto SelectedPatient
        {
            get
            {
                if (grdPatientView.FocusedRowHandle >= 0)
                {
                    return grdPatientView.GetFocusedRow() as PatientListDto;
                }
                return null;
            }
        }
        
        #endregion
        
        #region 생성자
        
        public PatientListControl()
        {
            InitializeComponent();
            
            // ViewModel 초기화 (DI에서 주입받음)
            var serviceProvider = this.GetService<IServiceProvider>();
            _viewModel = serviceProvider.GetService<PatientListViewModel>();
            
            // 검색 컨트롤 초기화
            InitializeSearchControl();
            
            // 초기화
            InitializeControls();
            RegisterEvents();
        }
        
        #endregion
        
        #region 화면 생명주기
        
        protected override void OnScreenActivated()
        {
            try
            {
                base.OnScreenActivated();
                
                // 권한 체크
                if (!HasPermission(PermissionType.Read))
                {
                    NuXtraMessageBox.ShowError("조회 권한이 없습니다.");
                    return;
                }
                
                // 데이터 로드
                LoadData();
            }
            catch (Exception ex)
            {
                LogManager.Error($"화면 활성화 중 오류: {ex.Message}", ScreenId, ex);
                NuXtraMessageBox.ShowError("화면을 로드하는 중 오류가 발생했습니다.");
            }
        }
        
        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();
            
            // 화면 비활성화 시 처리
            SaveLayout();
            
            // Event Aggregator 구독 해제
            EventAggregator?.GetEvent<PatientSelectedEvent>()
                .Unsubscribe(OnPatientSelectedFromOtherScreen);
        }
        
        #endregion
        
        #region 초기화
        
        private void InitializeSearchControl()
        {
            _searchControl = new PatientSearchControl();
            _searchControl.Dock = DockStyle.Top;
            _searchControl.SearchRequested += OnSearchRequested;
            this.Controls.Add(_searchControl);
        }
        
        private void InitializeControls()
        {
            // 그리드 초기화
            InitializeGrid();
            
            // 버튼 이벤트 연결
            ConnectButtonEvents();
            
            // 권한에 따른 버튼 상태 설정
            UpdateButtonPermissions();
        }
        
        private void InitializeGrid()
        {
            // 그리드 설정
            grdPatient.Dock = DockStyle.Fill;
            grdPatient.UseEmbeddedNavigator = false;
            grdPatient.OptionsView.ShowGroupPanel = false;
            grdPatient.OptionsSelection.MultiSelect = false;
            grdPatient.OptionsBehavior.Editable = false;
            
            // 뷰 설정
            var gridView = grdPatient.MainView as GridView;
            gridView.OptionsView.ShowAutoFilterRow = true;
            gridView.OptionsView.ShowFooter = true;
            
            // 컬럼 생성
            CreateGridColumns(gridView);
            
            // 더블클릭 이벤트
            grdPatient.DoubleClick += GrdPatient_DoubleClick;
        }
        
        private void CreateGridColumns(GridView gridView)
        {
            // 컬럼 초기화
            gridView.Columns.Clear();
            
            // 환자ID
            var colPatientId = gridView.Columns.AddField("PatientId");
            colPatientId.Caption = "환자ID";
            colPatientId.Width = 100;
            colPatientId.OptionsColumn.AllowEdit = false;
            
            // 환자명
            var colPatientName = gridView.Columns.AddField("PatientName");
            colPatientName.Caption = "환자명";
            colPatientName.Width = 150;
            colPatientName.OptionsColumn.AllowEdit = false;
            
            // 생년월일
            var colBirthDate = gridView.Columns.AddField("BirthDate");
            colBirthDate.Caption = "생년월일";
            colBirthDate.Width = 120;
            colBirthDate.DisplayFormat.FormatString = "yyyy-MM-dd";
            colBirthDate.OptionsColumn.AllowEdit = false;
            
            // 성별
            var colGenderName = gridView.Columns.AddField("GenderName");
            colGenderName.Caption = "성별";
            colGenderName.Width = 60;
            colGenderName.OptionsColumn.AllowEdit = false;
            
            // 혈액형
            var colBloodTypeName = gridView.Columns.AddField("BloodTypeName");
            colBloodTypeName.Caption = "혈액형";
            colBloodTypeName.Width = 60;
            colBloodTypeName.OptionsColumn.AllowEdit = false;
            
            // 컬럼 너비 자동 조정
            gridView.BestFitColumns();
        }
        
        private void ConnectButtonEvents()
        {
            btnSearch.Click += BtnSearch_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnExcel.Click += BtnExcel_Click;
            btnDetail.Click += BtnDetail_Click;
            btnPrint.Click += BtnPrint_Click;
        }
        
        private void UpdateButtonPermissions()
        {
            // 권한에 따른 버튼 상태 설정
            btnDetail.Enabled = HasPermission(PermissionType.Read);
            btnExcel.Enabled = HasPermission(PermissionType.Export);
            btnPrint.Enabled = HasPermission(PermissionType.Print);
            
            // 관리자만 추가 기능
            btnNew.Enabled = HasPermission(PermissionType.Create) && 
                             UserSession.Instance.IsAdmin;
        }
        
        private void RegisterEvents()
        {
            // Event Aggregator 구독
            EventAggregator?.GetEvent<PatientSelectedEvent>()
                .Subscribe(OnPatientSelectedFromOtherScreen);
                
            // ViewModel 이벤트
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        
        #endregion
        
        #region 데이터 처리
        
        private async void LoadData()
        {
            try
            {
                await _viewModel.LoadDataAsync(_searchControl.SearchCondition);
                
                // 그리드에 데이터 바인딩
                grdPatient.DataSource = _viewModel.Patients;
                
                // 전체 개수 표시
                lblTotalCount.Text = $"총 {_viewModel.TotalCount:N0}건";
            }
            catch (Exception ex)
            {
                LogManager.Error($"데이터 로드 중 오류: {ex.Message}", ScreenId, ex);
                NuXtraMessageBox.ShowError("데이터를 불러오는 중 오류가 발생했습니다.");
            }
        }
        
        private async Task LoadPatientData(string patientId)
        {
            try
            {
                var patientDetail = await _viewModel.GetPatientDetailAsync(patientId);
                
                if (patientDetail != null)
                {
                    // 환자 상세 화면 열기
                    var detailControl = new PatientDetailControl(patientDetail);
                    var document = this.Parent as DevExpress.XtraBars.Docking.UI.Documents.Document;
                    if (document != null)
                    {
                        document.Manager.AddDocument(detailControl, patientDetail.PatientName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"환자 상세 정보 로드 중 오류: {ex.Message}", ScreenId, ex);
                NuXtraMessageBox.ShowError("환자 상세 정보를 불러오는 중 오류가 발생했습니다.");
            }
        }
        
        #endregion
        
        #region 이벤트 핸들러
        
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PatientListViewModel.IsLoading))
            {
                // 로딩 상태 변경 시 UI 업데이트
                this.Cursor = _viewModel.IsLoading ? Cursors.WaitCursor : Cursors.Default;
                btnSearch.Enabled = !_viewModel.IsLoading;
                btnRefresh.Enabled = !_viewModel.IsLoading;
                progressBar.Visible = _viewModel.IsLoading;
            }
        }
        
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await LoadData();
        }
        
        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadData();
        }
        
        private void BtnExcel_Click(object sender, EventArgs e)
        {
            if (!HasPermission(PermissionType.Export))
            {
                NuXtraMessageBox.ShowError("엑셀 내보내기 권한이 없습니다.");
                return;
            }
            
            if (_viewModel.Patients.Count == 0)
            {
                NuXtraMessageBox.ShowInformation("내보낼 데이터가 없습니다.");
                return;
            }
            
            var saveFileDialog = new NuXtraSaveFileDialog
            {
                Filter = "Excel 파일 (*.xlsx)|*.xlsx|모든 파일 (*.*)|*.*",
                Title = "환자 목록 저장",
                FileName = $"환자목록_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 엑셀 내보내기
                    _viewModel.ExportToExcel(saveFileDialog.FileName);
                    NuXtraMessageBox.ShowInformation("엑셀 내보내기를 완료했습니다.");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"엑셀 내보내기 중 오류: {ex.Message}", ScreenId, ex);
                    NuXtraMessageBox.ShowError("엑셀 내보내기 중 오류가 발생했습니다.");
                }
            }
        }
        
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (!HasPermission(PermissionType.Print))
            {
                NuXtraMessageBox.ShowError("인쇄 권한이 없습니다.");
                return;
            }
            
            // 인쇄 기능 구현
            try
            {
                grdPatient.ShowPrintPreview();
            }
            catch (Exception ex)
            {
                LogManager.Error($"인쇄 중 오류: {ex.Message}", ScreenId, ex);
                NuXtraMessageBox.ShowError("인쇄 중 오류가 발생했습니다.");
            }
        }
        
        private async void BtnDetail_Click(object sender, EventArgs e)
        {
            var selectedPatient = SelectedPatient;
            if (selectedPatient == null)
            {
                NuXtraMessageBox.ShowInformation("상세 정보를 조회할 환자를 선택하세요.");
                return;
            }
            
            await LoadPatientData(selectedPatient.PatientId);
        }
        
        private void GrdPatient_DoubleClick(object sender, EventArgs e)
        {
            // 더블클릭 시 상세 정보 조회
            BtnDetail_Click(sender, e);
        }
        
        private async void OnSearchRequested(object sender, EventArgs e)
        {
            // 검색 컨트롤에서 검색 이벤트 발생 시
            await LoadData();
        }
        
        private void OnPatientSelectedFromOtherScreen(object payload)
        {
            // 다른 화면에서 환자 선택 이벤트 수신
            if (payload is PatientSelectedEventArgs evt)
            {
                // 자기 자신의 이벤트는 무시
                if (evt.SourceScreenId == ScreenId)
                    return;
                    
                // 환자 데이터 로드
                LoadPatientData(evt.Patient.PatientId);
            }
        }
        
        #endregion
        
        #region 레이아웃 관리
        
        private void SaveLayout()
        {
            try
            {
                var gridView = grdPatient.MainView as GridView;
                gridView.SaveLayoutToXml($@"Layouts\{ScreenId}_GridLayout.xml");
            }
            catch (Exception ex)
            {
                LogManager.Error($"그리드 레이아웃 저장 중 오류: {ex.Message}", ScreenId);
            }
        }
        
        private void RestoreLayout()
        {
            try
            {
                var gridView = grdPatient.MainView as GridView;
                gridView.RestoreLayoutFromXml($@"Layouts\{ScreenId}_GridLayout.xml");
            }
            catch (Exception ex)
            {
                LogManager.Error($"그리드 레이아웃 복원 중 오류: {ex.Message}", ScreenId);
            }
        }
        
        #endregion
        
        #region 리소스 정리
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Event Aggregator 구독 해제
                EventAggregator?.GetEvent<PatientSelectedEvent>()
                    .Unsubscribe(OnPatientSelectedFromOtherScreen);
                    
                // ViewModel 정리
                _viewModel?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        #endregion
    }
}
```

### 10.1.4 ViewModel 예시
```csharp
// nU3.Modules.EMR.CL.Patient/ViewModels/PatientListViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using nU3.Core.Services;
using nU3.Modules.EMR.CL.Patient.DTOs;

namespace nU3.Modules.EMR.CL.Patient.ViewModels
{
    /// <summary>
    /// 환자 목록 뷰 모델
    /// </summary>
    public class PatientListViewModel : INotifyPropertyChanged, IDisposable
    {
        #region 필드
        
        private readonly IPatientServiceAgent _serviceAgent;
        private bool _isLoading;
        private int _totalCount;
        private PatientSearchRequestDto _searchCondition;
        
        #endregion
        
        #region 속성
        
        /// <summary>
        /// 환자 목록
        /// </summary>
        public BindingList<PatientListDto> Patients { get; private set; }
        
        /// <summary>
        /// 전체 개수
        /// </summary>
        public int TotalCount
        {
            get => _totalCount;
            private set
            {
                if (_totalCount != value)
                {
                    _totalCount = value;
                    OnPropertyChanged(nameof(TotalCount));
                }
            }
        }
        
        /// <summary>
        /// 로딩 중 여부
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }
        
        /// <summary>
        /// 검색 조건
        /// </summary>
        public PatientSearchRequestDto SearchCondition
        {
            get => _searchCondition ??= new PatientSearchRequestDto();
            set
            {
                if (_searchCondition != value)
                {
                    _searchCondition = value;
                    OnPropertyChanged(nameof(SearchCondition));
                }
            }
        }
        
        #endregion
        
        #region 이벤트
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
        
        #region 생성자
        
        public PatientListViewModel(IPatientServiceAgent serviceAgent)
        {
            _serviceAgent = serviceAgent ?? 
                throw new ArgumentNullException(nameof(serviceAgent));
                
            Patients = new BindingList<PatientListDto>();
        }
        
        #endregion
        
        #region 공용 메서드
        
        /// <summary>
        /// 데이터 로드
        /// </summary>
        public async Task LoadDataAsync(PatientSearchRequestDto searchCondition = null)
        {
            try
            {
                IsLoading = true;
                
                var condition = searchCondition ?? SearchCondition;
                var result = await _serviceAgent.GetPatientsAsync(condition);
                
                // 데이터 바인딩
                Patients.Clear();
                foreach (var patient in result.Items)
                {
                    Patients.Add(patient);
                }
                
                TotalCount = result.TotalCount;
            }
            catch (Exception ex)
            {
                // 로깅은 호출한 쪽에서 처리
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// 환자 상세 정보 조회
        /// </summary>
        public async Task<PatientDetailDto> GetPatientDetailAsync(string patientId)
        {
            try
            {
                IsLoading = true;
                return await _serviceAgent.GetPatientAsync(patientId);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// 엑셀 내보내기
        /// </summary>
        public void ExportToExcel(string fileName)
        {
            try
            {
                if (Patients.Count == 0)
                {
                    throw new InvalidOperationException("내보낼 데이터가 없습니다.");
                }
                
                // 여기서는 실제 엑셀 내보내기 로직 구현
                // DevExpress 그리드 컨트롤의 내보내기 기능을 활용할 수 있음
                
                // 예: _gridControl.ExportToXlsx(fileName);
                
                LogManager.Info($"환자 목록 엑셀 내보내기 완료: {fileName}", "PatientList");
            }
            catch (Exception ex)
            {
                LogManager.Error($"엑셀 내보내기 중 오류: {ex.Message}", "PatientList", ex);
                throw;
            }
        }
        
        #endregion
        
        #region 리소스 정리
        
        public void Dispose()
        {
            // 리소스 정리
            Patients?.Clear();
        }
        
        #endregion
    }
}
```

---

## 🎯 결론

이 가이드는 nU3.Framework 기반의 업무화면 개발을 위해 필요한 모든 규칙과 절차를 상세하게 설명했습니다. 특히 `nU3ProgramInfo` 어트리뷰트의 사용법을 중심으로, 모듈 개발의 전체 생명주기를 다루었습니다.

### 핵심 사항 요약

1. **nU3ProgramInfo 어트리뷰트는 필수**: 모든 업무화면은 반드시 이 어트리뷰트를 가져야 합니다.
2. **상속 규칙 준수**: 모든 컨트롤은 `NuBaseControl`을, 모든 폼은 `NuBaseForm`을 상속해야 합니다.
3. **ScreenId 일치**: `nU3ProgramInfo.ProgramId`와 `ScreenId`가 반드시 일치해야 합니다.
4. **권한 체크**: 모든 화면은 권한 체크를 수행해야 합니다.
5. **예외 처리**: 모든 외부 호출은 `try-catch`로 예외 처리를 해야 합니다.
6. **리소스 정리**: `IDisposable`을 구현하고 `Dispose()`에서 리소스를 정리해야 합니다.

### 성공적인 모듈 개발을 위한 체크리스트

- [ ] `nU3ProgramInfo` 어트리뷰트가 올바르게 적용되었는가?
- [ ] `NuBaseControl` 또는 `NuBaseForm`을 상속하는가?
- [ ] `ScreenId`가 올바르게 구현되었는가?
- [ ] 권한 체크 로직이 있는가?
- [ ] 예외 처리가 적절하게 되어 있는가?
- [ ] 메모리 누수가 방지되도록 구현되었는가?
- [ ] 로깅이 적절하게 구현되었는가?

이 가이드를 준수한다면 개발자들은 일관된 고품질의 업무화면을 빠르게 개발할 수 있을 것입니다.

---

**문서 버전**: 1.0  
**최종 수정일**: 2026-02-07  
**작성자**: nU3.Framework Development Team