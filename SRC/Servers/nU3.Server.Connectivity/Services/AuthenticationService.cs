using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nU3.Core.Enums;
using nU3.Core.Interfaces;
using nU3.Core.Repositories;
using nU3.Server.Connectivity.Security;

namespace nU3.Server.Connectivity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;
        private const string DemoSecret = "dev-secret-please-change-0123456789";

        public AuthenticationService(IUserRepository userRepo) { _userRepo = userRepo; }

        public async Task<AuthResult> AuthenticateAsync(string id, string password)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Password) && !string.Equals(user.Password, password, StringComparison.Ordinal))
                        return new AuthResult { Success = false, ErrorMessage = "비밀번호가 일치하지 않습니다." };

                    var deptCodes = await _userRepo.GetUserDepartmentCodesAsync(id);
                    int roleCode = int.TryParse(user.Remarks, out var code) ? code : 1;
                    var roles = new[] { roleCode.ToString() };

                    var issuer = new LocalJwtIssuer(DemoSecret);
                    var token = issuer.GenerateDotNetToken(id, user.UserName, roles, deptCodes.ToArray(), 60);

                    return new AuthResult { Success = true, Token = token, DeptCodes = deptCodes.ToArray(), Roles = roles };
                }                

                return new AuthResult { Success = false, ErrorMessage = "아이디를 찾을 수 없습니다." };
            }
            catch (Exception ex) { return new AuthResult { Success = false, ErrorMessage = ex.Message }; }
        }        
    }
}
