using Microsoft.AspNetCore.Mvc;
using nU3.Core.Interfaces;
using System.Threading.Tasks;

namespace nU3.Server.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        public AuthController(IAuthenticationService authService) { _authService = authService; }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenticateAsync(request.Id, request.Password);
            return result.Success ? Ok(result) : Unauthorized(result);
        }
    }

    public class LoginRequest { public string Id { get; set; } = ""; public string Password { get; set; } = ""; }
}
