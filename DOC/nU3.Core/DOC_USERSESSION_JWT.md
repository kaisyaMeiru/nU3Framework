# UserSession JWT 기반 설계 및 SSO 지침 (상세)

이 문서는 `UserSession`의 JWT 기반 세션 동작, IdP 연동, 부서 선택 플로우, 검증/갱신 전략 및 UI 통합 예제를 상세히 정리합니다.

요약
- 클라이언트(데스크톱)는 IdP에 사용자 자격증명(ID/PW)을 전달하여 JWT 액세스 토큰을 받아옵니다.
- IdP가 반환한 JWT는 사용자 정보, 역할, 사용자가 속한 부서 목록을 포함해야 합니다.
- 클라이언트는 JWT를 `UserSession.SetJwt`로 저장하고, `SetJwtAndEnsureDepartment`를 통해 단일/다중 부서 처리(자동 선택 또는 UI 콜백 선택)를 수행합니다.
- 서명 검증(RS256 권장), 클레임 검증, 만료/리프레시 흐름은 운영 환경에서 반드시 구현해야 합니다.

목차
1. 토큰(Claims) 표준 및 권장 클레임
2. 로그인/부서 선택 플로우 (클라이언트 측)
3. UserSession 동작(핵심 메서드)
4. 토큰 검증 권장 절차 및 코드 예시
5. 리프레시 토큰 전략
6. UI 통합 예시 (LoginForm)
7. 보안 권장사항
8. 디버깅/운영 팁

---

1) 토큰(Claims) 표준 및 권장 클레임
- 표준 클레임
  - `iss`(issuer), `aud`(audience), `exp`, `nbf`, `iat`, `sub`(subject)
  - `name`, `preferred_username` 등 사용자 식별용
- 권장 커스텀/비표준 클레임
  - `role` 또는 `roles`: 역할(문자열 또는 배열)
  - `dept`, `depts`, `dept_code`, `department`: 사용자가 소속된 부서 코드(문자열 또는 배열)
  - `auth_level` 또는 `level`: 수치형 권한 레벨(선택적)

토큰 예시 (JSON payload)
{
  "iss": "https://idp.example.com",
  "aud": "nU3-client",
  "sub": "user123",
  "name": "홍길동",
  "roles": ["Doctor","Admin"],
  "depts": ["CARD","ER"],
  "auth_level": 9,
  "exp": 1700000000
}

2) 로그인/부서 선택 플로우 (클라이언트 측)
- 사용자가 로그인 폼에 ID/PW 입력 후 IdP로 인증 요청
- IdP 응답: 액세스 토큰(JWT) [+ 리프레시 토큰]
- 클라이언트 동작:
  1. `UserSession.SetJwt(jwt)` 호출 → 페이로드 파싱, ClaimsPrincipal 구성, AvailableDeptCodes 채움
  2. `UserSession.SetJwtAndEnsureDepartment(jwt, selector)` 호출
     - 만약 AvailableDeptCodes.Count == 1 → 자동 선택, `DeptCode` 설정
     - 다중일 경우 → UI에서 `selector(availableDepts)` 호출하여 사용자가 선택
  3. 선택된 부서를 `UserSession.DeptCode`로 설정하고 메인 화면 진입

3) UserSession 동작(핵심 메서드)
- `SetJwt(string jwt)`
  - JWT 원문 저장(JwtToken)
  - 페이로드(base64url) 파싱하여 Claims, 만료(exp), role들, dept 목록, 사용자 id/name, auth_level 파싱
  - 서명 검증은 수행하지 않음(운영 시 ValidateJwtWithParameters 권장)
- `SetJwtAndEnsureDepartment(string jwt, Func<IReadOnlyList<string>, string?>? departmentSelector)`
  - `SetJwt` 호출 후 AvailableDeptCodes를 확인
  - 부서가 1개면 자동 선택, 여러 개면 `departmentSelector` 콜백으로 선택 요청
  - 선택된 부서를 반환
- `ValidateJwt(Func<string,bool> signatureValidator = null, bool validateExpiry = true)`
  - 간단한 검증(선택적 서명 검증 콜백, 만료 체크)
- `ValidateJwtWithParameters(TokenValidationParameters)`
  - `JwtSecurityTokenHandler`를 사용한 완전 검증(권장)

4) 토큰 검증 권장 절차 및 코드 예시
- 권장 검증 순서
  1. 서명 검증 (JWKS 기반 RS256 권장)
  2. issuer/audience 확인
  3. 만료(exp)/nbf 확인
  4. 필요한 추가 클레임 검증(role, scope 등)

- TokenValidationParameters 예시
```csharp
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
var tvp = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = key, // 또는 IssuerSigningKeyResolver를 사용해 JWKS 공개키 사용
    ValidateIssuer = true,
    ValidIssuer = "https://idp.example.com",
    ValidateAudience = true,
    ValidAudience = "nU3-client",
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromSeconds(30)
};

var ok = UserSession.Current.ValidateJwtWithParameters(tvp);
```
- JWKS 사용 권장: 공개키 교체/로테이션을 IdP가 관리하게 하고 클라이언트는 JWKS URL을 통해 공개키를 받아 검증.

5) 리프레시 토큰 전략
- 액세스 토큰 만료 시 리프레시 토큰으로 새 액세스 토큰 요청
- 리프레시 토큰은 안전하게 저장(운영: OS credential store 혹은 키체인)
- 리프레시 토큰은 서버에서 회전 및 무효화해야 함
- 실패 시 사용자 재로그인 유도

6) UI 통합 예시 (LoginForm)
- 로그인 성공 시(예시)
```csharp
var jwt = response.AccessToken; // IdP에서 받은 토큰
string selectedDept = UserSession.Current.SetJwtAndEnsureDepartment(jwt, availableDepts =>
{
    // UI 레이어: 다중 부서일 때 선택 팝업을 띄우고 결과를 반환
    using var dlg = new DeptSelectionForm(availableDepts);
    return dlg.ShowDialog() == DialogResult.OK ? dlg.SelectedDept : null;
});

if (!string.IsNullOrEmpty(selectedDept))
{
    // 세션에 기본 사용자 정보가 채워져 있으므로 세션 기반으로 메인 진입
    SetSession(UserSession.Current.UserId, UserSession.Current.UserName, selectedDept, UserSession.Current.AuthLevel);
    OpenMainForm();
}
else
{
    // 부서 선택 취소 또는 부서 없음 처리
}
```
- DeptSelectionForm은 단순 콤보박스를 보여주고 사용자가 선택한 값을 `SelectedDept`로 반환하도록 구현

7) 보안 권장사항
- 토큰 원문을 로그에 기록하지 마세요.
- 리프레시 토큰 저장은 OS 제공 안전 저장소 사용 권장.
- 프로덕션에서는 RS256 + JWKS(또는 OIDC 메타데이터) 사용 권장.
- 짧은 access token 수명 + refresh token 회전 적용 권장.
- HTTPS 전송 강제.

8) 디버깅/운영 팁
- 개발 단계에서는 payload를 확인하기 쉬우나, 운영에서는 토큰 노출 주의
- JWT 파싱 실패 시(unknown format) 사용자에게 재로그인 안내
- IdP와 토큰 스펙(클레임 이름)을 표준화하고 문서화하여 클라이언트와 협의

---

부록: LoginForm 연동 체크리스트
- [ ] IdP 엔드포인트(토큰 발급) URL 구성
- [ ] 클라이언트 ID/비밀(필요 시) 보관
- [ ] 토큰 응답 스펙에 `depts` 또는 `dept` 포함 확인
- [ ] `UserSession.SetJwtAndEnsureDepartment` 호출 후 선택 결과 처리
- [ ] `ValidateJwtWithParameters`로 서명/클레임 검증 로직 연결

끝.
