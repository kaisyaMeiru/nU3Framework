using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using nU3.Core.Enums;

namespace nU3.Core.Security
{
    /// <summary>
    /// 애플리케이션 전역에서 현재 로그인된 사용자의 세션(컨텍스트)을 보관하는 싱글톤 클래스입니다.
    ///
    /// 설계 변경 요약 (JWT + IdP 기반 로그인 흐름)
    /// 1) 사용자는 로그인 화면에서 사용자ID와 패스워드를 입력합니다.
    /// 2) 클라이언트는 IdP(Identity Provider)에 인증 요청을 보냅니다.
    ///    - IdP는 사용자 인증에 성공하면 액세스 토큰(JWT)을 발행하여 응답합니다.
    ///    - JWT에는 사용자의 역할(Role) 정보 및 사용자가 소속된 부서 목록(예: claim 'dept' 또는 'depts')이 포함되어야 합니다.
    /// 3) 클라이언트는 받은 JWT를 UserSession에 저장하고(JwtToken), JWT에서 파싱된 부서 목록을 AvailableDeptCodes에 설정합니다.
    ///    - 만약 부서 리스트가 1개이면 자동으로 해당 부서를 선택하여 DeptCode에 저장합니다.
    ///    - 부서 리스트가 여러 개인 경우에는 UI에서 선택 팝업을 띄워 사용자가 로그인할 부서를 선택하도록 합니다. 선택된 부서는 DeptCode에 저장됩니다.
    /// 4) 이후 보호된 리소스 접근 시 UserSession에 저장된 JWT를 사용하거나 필요 시 서버로 토큰 갱신을 요청합니다.
    ///
    /// 구현 세부사항:
    /// - 이 클래스는 JWT의 페이로드를 파싱하여 ClaimsPrincipal, 만료시간, 역할, 사용가능 부서 목록을 채웁니다.
    /// - SetJwt는 서명 검증을 수행하지 않습니다(클라이언트에서 서명 검증이 필요하면 ValidateJwtWithParameters를 호출하세요).
    /// - SetJwt는 또한 JWT의 표준/비표준 클레임에서 사용자 식별자(sub, name 등), 표시이름, 그리고 auth 레벨(auth_level 등)을 시도하여 채웁니다.
    /// - 새로 추가된 `SetJwtAndEnsureDepartment`는 JWT를 세션에 저장한 후 부서 자동선택/선택콜백 흐름을 처리합니다.
    ///
    /// 보안 주의사항:
    /// - JWT 원문은 안전하게 취급하세요(로그에 남기지 말 것).
    /// - 운영 환경에서는 IdP의 공개키(JWKS)를 사용한 RS256 검증을 권장합니다.
    ///
    /// 문서: 자세한 설계/지침은 프로젝트의 DOC 파일(`DOC_USERSESSION_JWT.md`)을 참조하세요.
    /// </summary>
    public class UserSession
    {
        // 내부 싱글톤 인스턴스
        private static UserSession _instance;

        /// <summary>
        /// 전역(앱 전체)에서 접근 가능한 현재 세션 인스턴스를 반환합니다.
        /// 인스턴스가 없으면 지연 초기화로 생성합니다.
        /// </summary>
        public static UserSession Current => _instance ??= new UserSession();

        /// <summary>
        /// 로그인된 사용자의 고유 ID (주로 JWT의 'sub' 클레임에서 채워짐)
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// 로그인된 사용자의 표시 이름 (JWT의 'name' 또는 'preferred_username' 등에서 추출)
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 사용자가 속한 부서 코드(예: 진료과 코드)
        /// - 여러 부서에 속한 사용자의 경우, 로그인 시 선택된 부서 코드가 이 값에 저장됩니다.
        /// </summary>
        public string DeptCode { get; private set; }
        
        /// <summary>
        /// 기관코드 프로퍼티생성
        /// </summary>
        public string OrganCode { get; private set; }

        /// <summary>
        /// 클라이언트가 알고 있는 사용자의 사용 가능한 부서 코드 목록(로그인 시 선택용).
        /// </summary>
        private readonly List<string> _availableDeptCodes = new();
        public IReadOnlyList<string> AvailableDeptCodes => _availableDeptCodes.AsReadOnly();

        /// <summary>
        /// 사용자가 선택한(로그인에 사용된) 부서 코드. DeptCode와 동일합니다.
        /// </summary>
        public string SelectedDeptCode => DeptCode;

        /// <summary>
        /// 사용자의 권한 레벨(숫자 기반). 토큰에 'auth_level' 같은 클레임이 있을 경우 채워집니다.
        /// </summary>
        public int AuthLevel { get; private set; }

        /// <summary>
        /// 기본(첫번째) 역할을 유지합니다(기존 호환성용).
        /// </summary>
        public UserRole Role { get; private set; }

        /// <summary>
        /// 다중 역할 지원: JWT 또는 시스템에서 부여된 여러 역할을 저장합니다.
        /// </summary>
        private readonly List<UserRole> _roles = new();
        public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

        /// <summary>
        /// 저장된 JWT 원문입니다(있을 수도, 업그레이드될 수도 있음). 보안 주의: 로그에 남기지 마세요.
        /// </summary>
        public string? JwtToken { get; private set; }

        /// <summary>
        /// JWT에서 추출한 만료 시각(UTC). 파싱 불가 시 null.
        /// </summary>
        public DateTimeOffset? JwtExpiresAt { get; private set; }

        /// <summary>
        /// JWT에서 생성한 ClaimsPrincipal (검증 없이 생성된 경우도 있음). 검증 시 재설정됩니다.
        /// </summary>
        public ClaimsPrincipal? JwtPrincipal { get; private set; }

        /// <summary>
        /// 현재 세션이 로그인된 상태인지 여부를 반환합니다.
        /// UserId가 null 또는 빈 문자열이 아니면 true를 반환합니다.
        /// </summary>
        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        // 생성자는 외부에서 인스턴스를 생성하지 못하도록 private으로 제한
        private UserSession() { }

        /// <summary>
        /// 현재 세션 정보를 설정합니다. 로그인 처리 후 호출하여 사용자 정보를 초기화합니다.
        /// 이 메서드는 기존 세션 값을 덮어씁니다.
        /// </summary>
        /// <param name="userId">사용자 ID (필수)</param>
        /// <param name="userName">사용자 표시 이름</param>
        /// <param name="deptCode">부서 코드</param>
        /// <param name="authLevel">권한 레벨(정수)</param>
        public void SetSession(string userId, string userName, string deptCode, int authLevel)
        {
            SetSession(userId, userName, deptCode, authLevel, default);
        }

        /// <summary>
        /// 편의 오버로드: 역할만 함께 설정.
        /// </summary>
        /// <param name="userId">사용자 ID (필수)</param>
        /// <param name="userName">사용자 표시 이름</param>
        /// <param name="role">사용자 역할</param>
        /// <param name="authLevel">권한 레벨(정수)</param>
        public void SetSession(string userId, string userName, UserRole role, int authLevel)
        {
            SetSession(userId, userName, null, authLevel, role);
        }

        /// <summary>
        /// 메인 SetSession 구현.
        /// </summary>
        /// <param name="userId">사용자 ID (필수)</param>
        /// <param name="userName">사용자 표시 이름</param>
        /// <param name="deptCode">부서 코드</param>
        /// <param name="authLevel">권한 레벨(정수)</param>
        /// <param name="role">사용자 역할</param>
        public void SetSession(string userId, string userName, string deptCode, int authLevel, UserRole role)
        {
            UserId = userId;
            UserName = userName;
            DeptCode = deptCode;
            AuthLevel = authLevel;

            _roles.Clear();
            if (role != default)
            {
                _roles.Add(role);
                Role = role;
            }
            else
            {
                Role = default;
            }
        }

        /// <summary>
        /// 사용자가 소속된 부서 코드 목록을 설정합니다. 로그인 화면에서 표시할 목록을 세팅하세요.
        /// </summary>
        /// <param name="deptCodes">부서 코드 목록</param>
        public void SetAvailableDepartments(IEnumerable<string> deptCodes)
        {
            _availableDeptCodes.Clear();
            if (deptCodes == null) return;
            _availableDeptCodes.AddRange(deptCodes.Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => d.Trim()));
        }

        /// <summary>
        /// 사용자가 로그인 시 선택한 부서를 설정합니다. AvailableDeptCodes에 존재해야 선택 가능.
        /// Returns true if selection succeeded.
        /// </summary>
        /// <param name="deptCode">부서 코드</param>
        /// <returns>선택 성공 여부</returns>
        public bool SelectDepartment(string deptCode)
        {
            if (string.IsNullOrWhiteSpace(deptCode)) return false;
            var code = deptCode.Trim();
            if (_availableDeptCodes.Count == 0 || _availableDeptCodes.Contains(code))
            {
                DeptCode = code;
                return true;
            }
            return false;
        }

        /// <summary>
        /// JWT를 세션에 저장하고(원문), 토큰에서 만료시간과 Claims를 파싱합니다.
        /// 이 메서드는 토큰 서명 검증을 수행하지 않습니다. 검증이 필요하면 ValidateJwt(...)을 사용하세요.
        ///
        /// 추가 동작:
        /// - JWT의 부서 관련 클레임('dept','depts','dept_code','department')을 파싱하여 AvailableDeptCodes를 채웁니다.
        /// - JWT의 식별자(sub), 표시이름(name) 및 auth 레벨(auth_level 등)을 가능하면 추출하여 UserId/UserName/AuthLevel를 채웁니다.
        /// </summary>
        /// <param name="jwt">JWT 원문</param>
        public void SetJwt(string jwt)
        {
            JwtToken = string.IsNullOrWhiteSpace(jwt) ? null : jwt.Trim();
            JwtPrincipal = null;
            JwtExpiresAt = null;
            _roles.Clear();
            _availableDeptCodes.Clear();

            // 초기화: 사용자 관련 정보는 보존하지 않고 덮어씀
            UserId = null;
            UserName = null;
            AuthLevel = 0;

            if (string.IsNullOrWhiteSpace(JwtToken))
                return;

            try
            {
                var parts = JwtToken.Split('.');
                if (parts.Length < 2)
                    return;

                var payload = parts[1];
                var json = Encoding.UTF8.GetString(Base64UrlDecode(payload));

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // exp 처리: exp는 숫자(초)일 가능성이 높음
                if (root.TryGetProperty("exp", out var expProp))
                {
                    if (expProp.ValueKind == JsonValueKind.Number && expProp.TryGetInt64(out var expSeconds))
                    {
                        JwtExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expSeconds).ToUniversalTime();
                    }
                    else if (expProp.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(expProp.GetString(), out var dto))
                    {
                        JwtExpiresAt = dto.ToUniversalTime();
                    }
                }

                // Claims 생성
                var identity = new ClaimsIdentity("JWT");
                foreach (var prop in root.EnumerateObject())
                {
                    var name = prop.Name;
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in prop.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                                identity.AddClaim(new Claim(name, item.GetString() ?? string.Empty));
                        }
                    }
                    else if (prop.Value.ValueKind == JsonValueKind.String || prop.Value.ValueKind == JsonValueKind.Number || prop.Value.ValueKind == JsonValueKind.True || prop.Value.ValueKind == JsonValueKind.False)
                    {
                        identity.AddClaim(new Claim(name, prop.Value.ToString()));
                    }
                }

                JwtPrincipal = new ClaimsPrincipal(identity);

                // 사용자 식별자 및 표시이름 추출
                var sub = identity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? identity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? identity.FindFirst("uid")?.Value
                          ?? identity.FindFirst("user_id")?.Value;
                if (!string.IsNullOrWhiteSpace(sub))
                    UserId = sub;

                var nameClaim = identity.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                                ?? identity.FindFirst("preferred_username")?.Value
                                ?? identity.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrWhiteSpace(nameClaim))
                    UserName = nameClaim;

                // auth level 추출(선택적)
                var authClaim = identity.FindFirst("auth_level")?.Value
                                ?? identity.FindFirst("auth")?.Value
                                ?? identity.FindFirst("level")?.Value;
                if (!string.IsNullOrWhiteSpace(authClaim) && int.TryParse(authClaim, out var al))
                    AuthLevel = al;

                // 역할(claim "role" 또는 "roles") 파싱
                var roleClaims = identity.Claims.Where(c => string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase)
                                                            || string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase)
                                                            || c.Type == ClaimTypes.Role).ToList();
                foreach (var rc in roleClaims)
                {
                    var raw = rc.Value ?? string.Empty;
                    // 쉼표로 구분되어 있는 경우 분해
                    var partsRoles = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    foreach (var pr in partsRoles)
                    {
                        if (Enum.TryParse(typeof(UserRole), pr, true, out var parsed))
                        {
                            var ur = (UserRole)parsed;
                            if (!_roles.Contains(ur)) _roles.Add(ur);
                        }
                        else if (int.TryParse(pr, out var ival) && Enum.IsDefined(typeof(UserRole), ival))
                        {
                            var ur = (UserRole)ival;
                            if (!_roles.Contains(ur)) _roles.Add(ur);
                        }
                    }
                }

                // 부서 클레임 파싱: 'dept', 'depts', 'dept_code', 'department' 등
                var deptClaims = identity.Claims.Where(c => string.Equals(c.Type, "dept", StringComparison.OrdinalIgnoreCase)
                                                            || string.Equals(c.Type, "depts", StringComparison.OrdinalIgnoreCase)
                                                            || string.Equals(c.Type, "dept_code", StringComparison.OrdinalIgnoreCase)
                                                            || string.Equals(c.Type, "department", StringComparison.OrdinalIgnoreCase)).ToList();
                var deptList = new List<string>();
                foreach (var dc in deptClaims)
                {
                    var raw = dc.Value ?? string.Empty;
                    var partsDept = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    foreach (var pd in partsDept)
                    {
                        if (!string.IsNullOrWhiteSpace(pd) && !deptList.Contains(pd))
                            deptList.Add(pd);
                    }
                }

                if (deptList.Count > 0)
                {
                    SetAvailableDepartments(deptList);
                }

                // 기존 단일 Role 호환성 유지
                Role = _roles.FirstOrDefault();
            }
            catch
            {
                // 파싱 실패시 안전하게 null 유지
                JwtToken = null;
                JwtPrincipal = null;
                JwtExpiresAt = null;
                _roles.Clear();
                _availableDeptCodes.Clear();
                Role = default;
                UserId = null;
                UserName = null;
                AuthLevel = 0;
            }
        }

        /// <summary>
        /// JWT를 세션에 저장하고 부서 자동선택/선택콜백 흐름을 처리합니다.
        /// - jwt: IdP로부터 받은 JWT 원문
        /// - departmentSelector: 부서가 여러 개인 경우 UI 레이어에서 선택용 콜백을 제공합니다. (입력: AvailableDeptCodes, 반환: 선택된 부서코드 또는 null)
        /// 반환값: 선택된 부서코드(없으면 null)
        /// </summary>
        public string? SetJwtAndEnsureDepartment(string jwt, Func<IReadOnlyList<string>, string?>? departmentSelector = null)
        {
            SetJwt(jwt);

            if (AvailableDeptCodes == null || AvailableDeptCodes.Count == 0)
                return null;

            if (AvailableDeptCodes.Count == 1)
            {
                var single = AvailableDeptCodes[0];
                SelectDepartment(single);
                return single;
            }

            if (departmentSelector != null)
            {
                try
                {
                    var choice = departmentSelector(AvailableDeptCodes);
                    if (!string.IsNullOrWhiteSpace(choice) && SelectDepartment(choice))
                        return choice;
                }
                catch
                {
                    // selection callback failed - ignore and return null
                }
            }

            // 여기에 도달하면 다중 부서가 존재하지만 선택되지 않은 상태임
            return null;
        }

        /// <summary>
        /// JWT를 검증합니다. 서명 검증이 필요하면 signatureValidator 콜백을 전달하세요.
        /// signatureValidator는 JWT 원문을 받아 서명이 유효하면 true를 반환해야 합니다.
        /// 만료 검증(validateExpiry)이 true이면 exp 클레임을 확인합니다.
        /// </summary>
        /// <param name="signatureValidator">서명 검증 함수(선택)</param>
        /// <param name="validateExpiry">만료 검증 여부</param>
        /// <returns>검증 성공 여부</returns>
        public bool ValidateJwt(Func<string, bool>? signatureValidator = null, bool validateExpiry = true)
        {
            if (string.IsNullOrWhiteSpace(JwtToken))
                return false;

            // 서명 검증이 제공되면 호출
            if (signatureValidator != null)
            {
                try
                {
                    if (!signatureValidator(JwtToken))
                        return false;
                }
                catch
                {
                    return false;
                }
            }

            // 만료 검증
            if (validateExpiry && JwtExpiresAt.HasValue)
            {
                if (JwtExpiresAt.Value <= DateTimeOffset.UtcNow)
                    return false;
            }

            // JwtPrincipal은 SetJwt에서 생성됨. 여기서는 단순히 존재 여부로 검증 성공을 판단.
            return JwtPrincipal != null;
        }

        /// <summary>
        /// Microsoft.IdentityModel.Tokens 기반으로 토큰을 완전 검증합니다. 성공하면 JwtPrincipal을 검증된 principal로 교체합니다.
        /// </summary>
        /// <param name="validationParameters">토큰 검증 파라미터(TokenValidationParameters)</param>
        /// <returns>검증 성공 여부</returns>
        public bool ValidateJwtWithParameters(TokenValidationParameters validationParameters)
        {
            if (string.IsNullOrWhiteSpace(JwtToken) || validationParameters == null)
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(JwtToken, validationParameters, out var validatedToken);

                // 검증 성공: principal과 만료 정보 갱신
                JwtPrincipal = principal;

                if (validatedToken is JwtSecurityToken jst)
                {
                    var exp = jst.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp || c.Type == "exp")?.Value;
                    if (long.TryParse(exp, out var secs))
                        JwtExpiresAt = DateTimeOffset.FromUnixTimeSeconds(secs).ToUniversalTime();
                    else if (DateTimeOffset.TryParse(exp, out var dto))
                        JwtExpiresAt = dto.ToUniversalTime();
                }

                // 역할 claim에서 UserRole을 보충로딩
                var roleClaims = JwtPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role || string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase) || string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase));
                _roles.Clear();
                foreach (var rc in roleClaims)
                {
                    var raw = rc.Value ?? string.Empty;
                    var partsRoles = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    foreach (var pr in partsRoles)
                    {
                        if (Enum.TryParse(typeof(UserRole), pr, true, out var parsed))
                        {
                            var ur = (UserRole)parsed;
                            if (!_roles.Contains(ur)) _roles.Add(ur);
                        }
                        else if (int.TryParse(pr, out var ival) && Enum.IsDefined(typeof(UserRole), ival))
                        {
                            var ur = (UserRole)ival;
                            if (!_roles.Contains(ur)) _roles.Add(ur);
                        }
                    }
                }

                Role = _roles.FirstOrDefault();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// JWT가 만료되었는지 여부를 반환합니다(만료 정보가 없으면 false).
        /// </summary>
        public bool IsJwtExpired => JwtExpiresAt.HasValue && JwtExpiresAt.Value <= DateTimeOffset.UtcNow;

        /// <summary>
        /// 세션을 초기화(비우기)합니다. 로그아웃 시 호출하여 모든 사용자 관련 정보를 제거합니다.
        /// </summary>
        public void Clear()
        {
            UserId = null;
            UserName = null;
            DeptCode = null;
            AuthLevel = 0;
            Role = default;

            _availableDeptCodes.Clear();
            _roles.Clear();

            JwtToken = null;
            JwtPrincipal = null;
            JwtExpiresAt = null;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }
    }
}
