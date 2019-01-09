namespace Vue2API.Extensions
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using Auth;
    using Models;

    public static class StartupExtensions
    {
        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure strongly typed settings objects
            var settings = new AppSettings();
            configuration.Bind(nameof(AppSettings), settings);

            var jwtSigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.JwtSecretKey)), SecurityAlgorithms.HmacSha256);
            var jwtTwoFactorSigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.JwtTwoFactorSecretKey)), SecurityAlgorithms.HmacSha256);

            // Get Jwt options and configure it
            var jwtOptions = configuration.JwtOptions();
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtOptions.Issuer;
                options.Audience = jwtOptions.Audience;
                options.JwtSigningCredentials = jwtSigningCredentials;
                options.JwtTwoFactorSigningCredentials = jwtTwoFactorSigningCredentials;
            });

            services.AddAuthentication().AddJwtBearer(opts =>
            {
                opts.ClaimsIssuer = jwtOptions.Issuer;
                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = GetTokenValidationParameters(jwtOptions, jwtSigningCredentials);
            }).AddJwtBearer(JwtIssuerOptions.TwoFactorScheme, opts =>
            {
                opts.ClaimsIssuer = jwtOptions.Issuer;
                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = GetTokenValidationParameters(jwtOptions, jwtTwoFactorSigningCredentials);
            });

            services.AddSingleton<IJwtFactory, JwtFactory>();
        }

        /// <summary>
        /// Gets the Jwt Issuer settings from appsettings.json
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        /// <returns><see cref="JwtIssuerOptions"/></returns>
        internal static JwtIssuerOptions JwtOptions(this IConfiguration configuration)
        {
            var options = new JwtIssuerOptions();
            configuration.Bind(nameof(JwtIssuerOptions), options);

            return options;
        }

        private static TokenValidationParameters GetTokenValidationParameters(JwtIssuerOptions jwtOptions, SigningCredentials credentials)
        {
            return new TokenValidationParameters
            {
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,

                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = credentials.Key,

                RequireExpirationTime = false,

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
