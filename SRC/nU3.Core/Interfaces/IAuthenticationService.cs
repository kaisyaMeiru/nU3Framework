using System.Threading.Tasks;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 사용자 인증 및 세션 관리를 위한 서비스를 제공합니다.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// 사용자 아이디와 비밀번호를 사용하여 인증을 수행합니다.
        /// </summary>
        /// <param name="id">사용자 아이디</param>
        /// <param name="password">사용자 비밀번호</param>
        /// <returns>인증 결과 (성공 시 토큰, 부서 목록, 역할 목록 포함)</returns>
        Task<AuthResult> AuthenticateAsync(string id, string password);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string[]? DeptCodes { get; set; }
        public string[]? Roles { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
