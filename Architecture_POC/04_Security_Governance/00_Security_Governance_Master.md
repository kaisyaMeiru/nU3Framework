# 보안 및 거버넌스 마스터 문서 (Security & Governance Master Document)

**버전:** 1.0 (통합본)
**날짜:** 2026-02-10
**컨텍스트:** nU3.Framework (보안, 인증, 속성)

---

## 1. 개요

nU3.Framework의 보안은 인증을 위한 **JWT (JSON Web Tokens)**와 강력한 **역할 기반 접근 제어 (RBAC)** 시스템에 의존합니다. SSO(Single Sign-On)를 위해 외부 IdP(Identity Provider)와 통합됩니다.

---

## 2. 인증 흐름 (`UserSession`)

### 2.1 `UserSession` 싱글톤
클라이언트 애플리케이션의 중앙 보안 컨텍스트입니다.
*   **저장소:** 원시 JWT 및 파싱된 클레임(Claims) 저장.
*   **상태:** 현재 `UserId`, `UserName`, `DeptCode`, `AuthLevel`.

### 2.2 로그인 프로세스
1.  **로그인 UI:** 사용자가 자격 증명 입력.
2.  **요청:** 클라이언트가 IdP에 자격 증명 전송.
3.  **응답:** IdP가 서명된 JWT(RS256) 반환.
4.  **토큰 처리:**
    *   `UserSession.SetJwt(token)` 호출.
    *   클레임 파싱 (`roles`, `depts`, `auth_level`).
    *   **부서 선택:** 사용자가 여러 부서에 속한 경우 `SetJwtAndEnsureDepartment()`를 통해 UI 선택기 트리거.
5.  **검증:**
    *   서명 확인 (JWKS).
    *   만료 확인 (`exp`).
    *   대상 확인 (`aud`).

### 2.3 토큰 갱신
*   액세스 토큰은 수명이 짧습니다.
*   재로그인 없이 세션을 갱신하기 위해 리프레시 토큰이 안전하게 저장됩니다(OS 자격 증명 저장소).

---

## 3. 거버넌스 및 메타데이터

### 3.1 메타데이터 속성 (Attributes)
프레임워크는 디자인 타임에 보안 및 동작을 정의하기 위해 속성을 사용합니다.

#### `[nU3ProgramInfo]` (핵심 보안 속성)
화면의 보안 컨텍스트를 정의합니다.
*   **`AuthLevel`**: 필요한 최소 권한 수준 (예: 1=일반, 9=관리자).
*   **`ProgramId`**: 권한 매핑을 위한 고유 식별자.

#### WinForms 디자인 타임 속성
Visual Studio에서의 개발자 경험을 위해 사용됩니다.
*   **`[ToolboxItem]`**: 도구 상자 표시 제어.
*   **`[Browsable(false)]`**: 내부 속성 숨김.
*   **`[Description]`**: 속성에 대한 한글 도움말 제공.

### 3.2 권한 시스템
권한은 세분화되어 있으며 프로그램별로 정의됩니다.
*   **`CanRead`**, **`CanCreate`**, **`CanUpdate`**, **`CanDelete`**
*   **`CanPrint`**, **`CanExport`**
*   **`CanApprove`**, **`CanCancel`**

권한은 사용자의 역할과 화면의 `AuthLevel`에 따라 `WorkContext.Permissions`에 로드됩니다.

---

## 4. 구현 상태 (Phase 4)

*   **완료됨:**
    *   JWT 파싱을 포함한 `UserSession` 클래스.
    *   속성 기반 메타데이터 시스템.
    *   부서 선택 로직.
    *   `WorkContext` 내 권한 플래그.

*   **다음 단계:**
    *   실제 OAuth2/OIDC 제공자 통합.
    *   리프레시 토큰을 위한 보안 저장소 구현.