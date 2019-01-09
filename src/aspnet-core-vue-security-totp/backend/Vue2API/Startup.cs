namespace Vue2API
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using AutoMapper;
    using FluentValidation.AspNetCore;
    using Serilog;
    using Swashbuckle.AspNetCore.Swagger;

    using Auth;
    using Data;
    using Extensions;
    using Helpers;
    using Infrastructure;
    using Models;
    using Models.Entities;

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("apidb"));

            //services.AddMediatR();
            services.AddAutoMapper(GetType().Assembly);
            services.AddDistributedMemoryCache(); // provides IDistributedCache cache
            services.AddSession(o =>
            {
                // Set a short timeout for easy testing
                o.IdleTimeout = TimeSpan.FromMinutes(2);
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.Name = "Vue2API.Session";
                o.Cookie.HttpOnly = true;
                //o.Cookie.SameSite = SameSiteMode.None;
            });

            // Add and Setup Identity
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<AppUser>>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings.
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

                options.User.RequireUniqueEmail = true;
            });

            // Setup API policies
            services.AddAuthorization(options =>
            {
                // Policy for users
                //options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Policy, Constants.Strings.JwtClaims.ApiAccess));
            });

            services.AddJwt(Configuration);

            services.AddLocalization(x => x.ResourcesPath = "Properties");

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
                c.SwaggerDoc("v1", new Info { Title = "Secure API", Version = "v1" });
                c.CustomSchemaIds(y => y.FullName);
                c.DocInclusionPredicate((version, apiDescription) => true);
                //c.TagActionsBy(y => y.GroupName);
            });

            // services.AddHttpContextAccessor(); // use when IHttpContextAccessor needs to be injected
            services.AddCors(o => o.AddPolicy("SiteCorsPolicy", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:8080", "https://localhost:8080")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.Configure<MvcOptions>(x => x.Conventions.Add(new ApiValidationConvention()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsProduction())
                app.UseHsts();

            app.UseHttpsRedirection();

            app.UseSession();
            app.UseAuthentication();
            app.UseCors("SiteCorsPolicy"); // enable specified CORS policy in entire app

            if (env.IsDevelopment())
                SetupDemoUser(serviceProvider).Wait();

            app.UseStaticFiles();

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure API V1");
            });
        }

        private async Task SetupDemoUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            var role = await roleManager.FindByNameAsync("user");
            if (role is null)
            {
                role = new AppRole
                {
                    Id = "user",
                    Name = "user"
                };

                await roleManager.CreateAsync(role);
            }

            var email = "admin@playground.git"; // Fictional email for demo purposes!
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "David De Smet",
                    UserName = email,
                    Email = email,
                    NormalizedUserName = email.ToUpper(),
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    TwoFactorEnabled = false
                };

                var result = await userManager.CreateAsync(user, "123456"); // Super secure password for demo purposes!
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role.Name);

                    var claims = new List<Claim>
                    {
                        new Claim(type: Constants.Strings.JwtClaimIdentifiers.Policy, value: Constants.Strings.JwtClaims.ApiAccess)
                    };
                    await userManager.AddClaimsAsync(user, claims);
                }

                var context = serviceProvider.GetService<ApplicationDbContext>();

                context.Profiles.Add(new Models.Entities.Profile
                {
                    Id = Guid.NewGuid().ToString(),
                    Identity = user,
                    Location = "localhost"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
