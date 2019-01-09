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

            services.AddAuthentication(/*o =>
            {
                //o.DefaultSignInScheme = IdentityConstants.TwoFactorUserIdScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }*/).AddJwtBearer(opts =>
            {
                //opts.Events = new JwtBearerEvents
                //{
                //    // This will screw things up since it needs to have Bearer up front
                //    OnMessageReceived = context =>
                //    {
                //        var token = context.HttpContext.Request.Headers["Authorization"];
                //        if (token.Count > 0 && token[0].StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                //            context.Token = token[0].Substring("Token ".Length).Trim();

                //        return Task.CompletedTask;
                //    },
                //    OnTokenValidated = context =>
                //    {
                //        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                //        var userId = int.Parse(context.Principal.Identity.Name);
                //        var user = userService.GetById(userId);
                //        if (user == null)
                //        {
                //            // return unauthorized if user no longer exists
                //            context.Fail("Unauthorized");
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
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
            //.AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o => o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme)
            //.AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            //{
            //    o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
            //    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //    o.Cookie.Name = "app_user";
            //    o.Cookie.Expiration = TimeSpan.FromMinutes(30);
            //    o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            //    o.SlidingExpiration = true;
            //    o.LoginPath = new Microsoft.AspNetCore.Http.PathString("/login");
            //});
            /*
            .AddIdentityCookies(o => { });*/

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
