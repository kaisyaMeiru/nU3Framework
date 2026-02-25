using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using nU3.Core.Interfaces;

namespace nU3.Connectivity.Implementations
{
    public class HttpAuthenticationClient : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public HttpAuthenticationClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<AuthResult> AuthenticateAsync(string id, string password)
        {
            try {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/auth/login", new { Id = id, Password = password });
                return await response.Content.ReadFromJsonAsync<AuthResult>() ?? new AuthResult { Success = false };
            } catch (Exception ex) { return new AuthResult { Success = false, ErrorMessage = ex.Message }; }
        }
    }
}
