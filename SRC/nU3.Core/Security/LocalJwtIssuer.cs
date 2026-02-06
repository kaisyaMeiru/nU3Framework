using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace nU3.Core.Security
{
    /// <summary>
    /// 간단한 로컬 JWT 발행기입니다.
    /// - 개발/테스트 목적용으로 HMAC-SHA256(HS256) 방식으로 JWT를 생성합니다.
    /// - 실제 운영 환경에서는 중앙 인증서버(IdP)를 통해 토큰을 발행하도록 교체하세요.
    /// - 추후 RS256 등 비대칭 방식으로 변경하기 쉬운 구조로 작성되어 있습니다.
    /// </summary>
    public class LocalJwtIssuer
    {
        private readonly byte[] _signingKey; // HMAC key
        public string Issuer { get; }
        public string Audience { get; }

        /// <summary>
        /// 생성자. signingKey는 임시용(예: "secret-key") 문자열로 전달할 수 있습니다.
        /// 내부적으로 UTF8 바이트로 사용합니다. 테스트용으로만 사용하세요.
        /// </summary>
        public LocalJwtIssuer(string signingKey, string issuer = "local", string audience = "nU3")
        {
            if (string.IsNullOrEmpty(signingKey))
                throw new ArgumentException("signingKey is required", nameof(signingKey));

            _signingKey = Encoding.UTF8.GetBytes(signingKey);
            Issuer = issuer ?? "local";
            Audience = audience ?? "nU3";
        }

        /// <summary>
        /// JWT를 생성합니다.
        /// - roles: 문자열 목록(예: "Admin", "Doctor")
        /// - deptCodes: 사용자가 소속된 부서 코드 목록(로그인 시 선택 가능)
        /// </summary>
        public string GenerateToken(
            string userId,
            string userName,
            IEnumerable<string>? roles = null,
            IEnumerable<string>? deptCodes = null,
            int expiresMinutes = 30)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("userId is required", nameof(userId));

            var header = new Dictionary<string, object>
            {
                ["alg"] = "HS256",
                ["typ"] = "JWT"
            };

            var now = DateTimeOffset.UtcNow;
            var exp = now.AddMinutes(Math.Max(1, expiresMinutes));

            var payload = new Dictionary<string, object>
            {
                ["iss"] = Issuer,
                ["aud"] = Audience,
                ["sub"] = userId,
                ["name"] = userName ?? string.Empty,
                ["iat"] = now.ToUnixTimeSeconds(),
                ["exp"] = exp.ToUnixTimeSeconds()
            };

            if (roles != null)
            {
                var rs = roles.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).ToArray();
                if (rs.Length == 1)
                    payload["role"] = rs[0];
                else if (rs.Length > 1)
                    payload["roles"] = rs;
            }

            if (deptCodes != null)
            {
                var ds = deptCodes.Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => d.Trim()).ToArray();
                if (ds.Length == 1)
                    payload["dept"] = ds[0];
                else if (ds.Length > 1)
                    payload["depts"] = ds;
            }

            string headerJson = JsonSerializer.Serialize(header);
            string payloadJson = JsonSerializer.Serialize(payload);

            string headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            string payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            string unsignedToken = headerBase64 + "." + payloadBase64;
            string signature = ComputeSignature(unsignedToken);

            return unsignedToken + "." + signature;
        }

        /// <summary>
        /// JwtSecurityTokenHandler로 생성된 토큰 문자열을 반환합니다. 라이브러리와 상호운용이 필요할 때 사용하세요.
        /// </summary>
        public string GenerateDotNetToken(string userId, string userName, IEnumerable<string>? roles = null, IEnumerable<string>? deptCodes = null, int expiresMinutes = 30)
        {
            var now = DateTime.UtcNow;
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, userId),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Name, userName ?? string.Empty),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), System.Security.Claims.ClaimValueTypes.Integer64)
            };

            if (roles != null)
            {
                foreach (var r in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
                    claims.Add(new System.Security.Claims.Claim("role", r.Trim()));
            }

            if (deptCodes != null)
            {
                foreach (var d in deptCodes.Where(d => !string.IsNullOrWhiteSpace(d)))
                    claims.Add(new System.Security.Claims.Claim("dept", d.Trim()));
            }

            var key = new SymmetricSecurityKey(_signingKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(Math.Max(1, expiresMinutes)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// JWT의 서명을 검증하고(exp는 옵션으로 검사) 유효하면 true를 반환합니다.
        /// 간단한 검증 로직으로, 운영 환경에서는 전문 라이브러리를 사용하세요.
        /// </summary>
        public bool ValidateToken(string token, bool validateExpiry = true)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            var parts = token.Split('.');
            if (parts.Length != 3) return false;

            var unsigned = parts[0] + "." + parts[1];
            var signature = parts[2];
            var expected = ComputeSignature(unsigned);

            if (!CryptographicEquals(signature, expected)) return false;

            if (!validateExpiry) return true;

            try
            {
                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                using var doc = JsonDocument.Parse(payloadJson);
                var root = doc.RootElement;
                if (root.TryGetProperty("exp", out var expProp))
                {
                    if (expProp.ValueKind == JsonValueKind.Number && expProp.TryGetInt64(out var expSeconds))
                    {
                        var exp = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                        return exp > DateTimeOffset.UtcNow;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private string ComputeSignature(string unsignedToken)
        {
            using var hmac = new HMACSHA256(_signingKey);
            var bytes = Encoding.UTF8.GetBytes(unsignedToken);
            var hash = hmac.ComputeHash(bytes);
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            var s = Convert.ToBase64String(bytes);
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
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

        private static bool CryptographicEquals(string a, string b)
        {
            var ba = Encoding.UTF8.GetBytes(a);
            var bb = Encoding.UTF8.GetBytes(b);
            if (ba.Length != bb.Length) return false;
            int diff = 0;
            for (int i = 0; i < ba.Length; i++) diff |= ba[i] ^ bb[i];
            return diff == 0;
        }
    }
}
