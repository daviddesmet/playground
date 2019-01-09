namespace Vue2API.Auth
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using Models;

    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public double GetExpiration() => _jwtOptions.ValidFor.TotalSeconds;

        public async Task<string> CreateTokenAsync(ClaimsPrincipal principal, TokenScheme scheme = TokenScheme.Default)
        {
            var claims = new List<Claim>(principal.Claims)
            {
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            SigningCredentials credentials;
            switch (scheme)
            {
                case TokenScheme.TwoFactor:
                    // Create a JWT that can be used only for two-factor authentication
                    //claims.Add(new Claim(ClaimTypes.AuthenticationMethod, IdentityConstants.TwoFactorUserIdScheme));
                    claims.Add(new Claim(ClaimTypes.AuthenticationMethod, JwtIssuerOptions.TwoFactorScheme));
                    credentials = _jwtOptions.JwtTwoFactorSigningCredentials;
                    break;
                case TokenScheme.TwoFactorRememberMe:
                    claims.Add(new Claim(ClaimTypes.AuthenticationMethod, IdentityConstants.TwoFactorRememberMeScheme));
                    credentials = _jwtOptions.JwtTwoFactorSigningCredentials;
                    break;
                default:
                    credentials = _jwtOptions.JwtSigningCredentials;
                    break;
            }

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.JwtSigningCredentials is null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JwtSigningCredentials));

            if (options.JwtTwoFactorSigningCredentials is null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JwtTwoFactorSigningCredentials));

            if (options.JtiGenerator is null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }
}