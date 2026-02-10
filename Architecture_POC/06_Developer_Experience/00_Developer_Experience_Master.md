# 개발자 경험 마스터 문서 (Developer Experience Master Document)

**버전:** 1.0 (통합본)
**날짜:** 2026-02-10
**컨텍스트:** nU3.Framework (표준화, 가이드, 패턴)

---

## 1. 개발 표준

### 1.1 아키텍처: 3-Tier Layered
1.  **프레젠테이션 (UI):** WinForms + DevExpress.
2.  **서비스 에이전트 (클라이언트 로직):** HTTP 통신 추상화 (`IPatientServiceAgent`).
3.  **서버 (API):** ASP.NET Core REST API.

### 1.2 공통 DTO
*   **요청:** `BaseRequestDto` (사용자, 부서, IP), `PagedRequestDto` (페이지, 크기, 정렬).
*   **응답:** `BaseResponseDto` (성공 여부, 코드, 메시지), `PagedResultDto<T>`.
*   **명명:** `[Entity]ListDto`, `[Entity]DetailDto`.

### 1.3 서비스 에이전트 패턴
`HttpClient` 호출을 추상화합니다.
```csharp
public class PatientServiceAgent : IPatientServiceAgent {
    public async Task<PagedResultDto<PatientListDto>> GetPatientsAsync(...) {
        // 토큰 주입, 401/500 처리, DTO 반환
    }
}
```

---

## 2. UI 컴포넌트 표준화 (래퍼)

DevExpress 컨트롤을 래핑하여 표준을 강제하고 프레임워크 로직을 주입합니다.

| 래퍼 | 기본 컨트롤 | 기능 |
|---------|--------------|----------|
| **`NuGridControl`** | `GridControl` | 자동 스타일, 엑셀 내보내기, 레이아웃 저장/복원. |
| **`NuBaseControl`** | `XtraUserControl` | 권한, 라이프사이클, 컨텍스트. |
| **`NuSearchControl`** | Custom | 표준 검색 레이아웃 및 이벤트. |

**왜 래핑하는가?**
1.  **표준화:** 일관된 룩앤필 강제.
2.  **통합:** 버튼에 대한 자동 권한 확인.
3.  **독립성:** 벤더 변경으로부터 비즈니스 코드 보호.

---

## 3. 모듈 개발 가이드

### 3.1 프로젝트 구조
`nU3.Modules.[Category].[SubSystem].[Name]`
*   **Controls/**: UI (`PatientListControl.cs`)
*   **ViewModels/**: 로직 (`PatientListViewModel.cs`)
*   **DTOs/**: 데이터 전송 객체
*   **Services/**: 로컬 비즈니스 로직

### 3.2 `[nU3ProgramInfo]` 속성
모든 화면에 필수입니다.
```csharp
[nU3ProgramInfo(typeof(MyControl), "제목", "PROG_ID", "CHILD")]
public partial class MyControl : BaseWorkControl { ... }
```

### 3.3 개발 워크플로우
1.  **분석:** 요구사항 정의.
2.  **프로젝트 생성:** Deployer 템플릿 사용.
3.  **DTO 정의:** 서버 사양과 일치.
4.  **VM 구현:** 서비스 에이전트 바인딩.
5.  **UI 구현:** `BaseWorkControl` 상속.
6.  **등록:** Deployer를 사용하여 DLL 업로드 및 메뉴 설정.

---

## 4. 체크리스트

### 코드 품질
*   [ ] `BaseWorkControl` / `BaseWorkForm` 상속.
*   [ ] `nU3ProgramInfo` 사용.
*   [ ] `ScreenId` 구현.
*   [ ] 권한 확인 (`HasPermission`).
*   [ ] 예외 처리 (`try-catch`).
*   [ ] 리소스 해제 (Dispose).

### 배포
*   [ ] Release 모드로 빌드.
*   [ ] Deployer로 검증.
*   [ ] 리소스에 대한 `InstallPath` 확인.

---

## 5. 도구 및 템플릿

*   **Deployer:** 스마트 업로드, 메뉴 편집기.
*   **생성기:** DTO 생성기 (DB 스키마 기반), 서비스 에이전트 생성기.
*   **VS 템플릿:** 화면/VM용 아이템 템플릿.