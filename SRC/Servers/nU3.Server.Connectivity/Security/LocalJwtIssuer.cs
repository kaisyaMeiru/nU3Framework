using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace nU3.Server.Connectivity.Security
{
    public class LocalJwtIssuer
    {
        private readonly byte[] _signingKey;
        public string Issuer { get; }
        public string Audience { get; }

        public LocalJwtIssuer(string signingKey, string issuer = "local", string audience = "nU3")
        {
            _signingKey = Encoding.UTF8.GetBytes(signingKey);
            Issuer = issuer;
            Audience = audience;
        }

        public string GenerateDotNetToken(string userId, string userName, IEnumerable<string>? roles = null, IEnumerable<string>? deptCodes = null, int expiresMinutes = 30)
        {
            var now = DateTime.UtcNow;
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, userId),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Name, userName ?? string.Empty),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), System.Security.Claims.ClaimValueTypes.Integer64)
            };

            if (roles != null) foreach (var r in roles) claims.Add(new System.Security.Claims.Claim("role", r.Trim()));
            if (deptCodes != null) foreach (var d in deptCodes) claims.Add(new System.Security.Claims.Claim("dept", d.Trim()));

            var key = new SymmetricSecurityKey(_signingKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: Issuer, audience: Audience, claims: claims, notBefore: now, expires: now.AddMinutes(expiresMinutes), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
