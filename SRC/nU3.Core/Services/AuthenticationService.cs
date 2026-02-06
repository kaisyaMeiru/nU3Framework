using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nU3.Core.Enums;
using nU3.Core.Interfaces;
using nU3.Core.Repositories;
using nU3.Core.Security;

namespace nU3.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;
        private const string DemoSecret = "dev-secret-please-change-0123456789";

        public AuthenticationService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<AuthResult> AuthenticateAsync(string id, string password)
        {
            try
            {
                // 1. 저장소를 통한 사용자 조회
                var user = await _userRepo.GetUserByIdAsync(id);
                
                if (user != null)
                {
                    // 비밀번호 검증 (Plaintext for Demo)
                    if (!string.IsNullOrEmpty(user.Password) && !string.Equals(user.Password, password, StringComparison.Ordinal))
                    {
                        return new AuthResult { Success = false, ErrorMessage = "비밀번호가 일치하지 않습니다." };
                    }

                    // 부서 코드 목록 조회
                    var deptCodes = await _userRepo.GetUserDepartmentCodesAsync(id);
                    
                    // 역할 정보 (DB 스키마 상 ROLE_CODE)
                    var roleCode = string.IsNullOrEmpty(user.Remarks) ? "User" : user.Remarks;
                    var roles = new[] { roleCode };

                    var issuer = new LocalJwtIssuer(DemoSecret, issuer: "local", audience: "nU3");
                    var token = issuer.GenerateDotNetToken(id, user.UserName, roles: roles, deptCodes: deptCodes.ToArray(), expiresMinutes: 60);

                    return new AuthResult
                    {
                        Success = true,
                        Token = token,
                        DeptCodes = deptCodes.ToArray(),
                        Roles = roles
                    };
                }

                // 2. Demo Fallback (admin / 1234)
                if (id == "admin" && password == "1234")
                {
                    return CreateDemoAuthResult(id, "시스템 관리자", new[] { "Admin" });
                }

                return new AuthResult { Success = false, ErrorMessage = "아이디를 찾을 수 없거나 비활성화된 계정입니다." };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, ErrorMessage = $"인증 서비스 오류: {ex.Message}" };
            }
        }

        private AuthResult CreateDemoAuthResult(string id, string name, string[] roles)
        {
            var issuer = new LocalJwtIssuer(DemoSecret, issuer: "local", audience: "nU3");
            var defaultDepts = Enum.GetValues(typeof(Department))
                                   .Cast<Department>()
                                   .Take(2)
                                   .Select(d => d.ToString())
                                   .ToArray();

            var token = issuer.GenerateDotNetToken(id, name, roles: roles, deptCodes: defaultDepts, expiresMinutes: 60);

            return new AuthResult
            {
                Success = true,
                Token = token,
                DeptCodes = defaultDepts,
                Roles = roles
            };
        }
    }
}