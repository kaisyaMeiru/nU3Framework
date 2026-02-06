using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace nU3.Core.Security
{
    /// <summary>
    /// Jwt 검증을 위한 TokenValidationParameters 생성 헬퍼
    /// - 간단히 대칭 키(HS256) 기반 검증 파라미터를 생성합니다.
    /// - 실제 운영에서는 JWKS(OpenID Connect) 기반 공개키(권장 RS256) 검증을 사용하세요.
    /// </summary>
    public static class JwtValidationHelper
    {
        /// <summary>
        /// 대칭 키(문자열 시크릿)를 사용한 TokenValidationParameters를 생성합니다.
        /// </summary>
        /// <param name="secret">시크릿 문자열(예: "my-very-secret")</param>
        /// <param name="validIssuer">검증할 issuer(iss)</param>
        /// <param name="validAudience">검증할 audience(aud)</param>
        /// <param name="clockSkewSeconds">허용할 클럭 스큐(초)</param>
        /// <returns>TokenValidationParameters</returns>
        public static TokenValidationParameters CreateSymmetricValidationParameters(
            string secret,
            string validIssuer,
            string validAudience,
            int clockSkewSeconds = 60)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentException("secret is required", nameof(secret));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidateIssuer = !string.IsNullOrEmpty(validIssuer),
                ValidIssuer = validIssuer,

                ValidateAudience = !string.IsNullOrEmpty(validAudience),
                ValidAudience = validAudience,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromSeconds(Math.Max(0, clockSkewSeconds))
            };
        }
    }
}
