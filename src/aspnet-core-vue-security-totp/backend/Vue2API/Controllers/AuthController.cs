namespace Vue2API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;

    using Auth;
    using Helpers;
    using Infrastructure;
    using Models;
    using Models.Entities;
    using ViewModels;

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly ILogger _logger;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtFactory jwtFactory, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtFactory = jwtFactory;
            _logger = logger;
        }

        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]CredentialsViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                _logger.LogInformation("User with ID {UserId} is logged in.", user.Id);

                var jwt = await GetUserTokenAsync(user);
                return Ok(new ApiOkResponse(jwt).Result);
            }

            if (result.RequiresTwoFactor)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var jwt = await GetUserTokenAsync(user, TokenScheme.TwoFactor);

                return StatusCode((int)HttpStatusCode.PartialContent, jwt);
            }

            if (result.IsLockedOut)
            {
                //_logger.LogWarning("User with ID {UserId} account locked out.", _userManager.GetUserId(User));
                return Unauthorized(new ApiUnauthorizedResponse("Account is locked."));
            }

            return Unauthorized(new ApiUnauthorizedResponse("Invalid username or password."));
        }

        // GET api/auth/refresh
        [HttpGet("refresh")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Scheme)]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId} for token refresh.", userId);
                return Unauthorized(new ApiUnauthorizedResponse("Unable to refresh token."));
            }

            var jwt = await GetUserTokenAsync(user);
            return Ok(new ApiOkResponse(jwt).Result);
        }

        // POST api/auth/2fa
        [HttpPost("2fa")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.TwoFactorScheme)]
        public async Task<IActionResult> LoginWith2fa([FromBody]LoginWithAuthenticatorViewModel model)
        {
            var userId = await RetrieveBearerTwoFactorUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to login with 2FA."));

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to login with 2FA."));
            }

            if (!await _signInManager.CanSignInAsync(user) || (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)))
            {
                _logger.LogWarning("Unable to sign in user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "The specified user cannot sign in."));
            }

            // Strip spaces and hypens
            var pin = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var provider = _userManager.Options.Tokens.AuthenticatorTokenProvider ?? TokenOptions.DefaultAuthenticatorProvider;
            var verified = await _userManager.VerifyTwoFactorTokenAsync(user, provider, pin);
            if (verified)
            {
                // When token is verified correctly, clear the access failed count used for lockout
                if (_userManager.SupportsUserLockout)
                    await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("User with ID {UserId} logged in with 2FA.", user.Id);

                if (model.RememberDevice)
                {
                    // TODO: Implement 2FA Remember Device Cookie
                    //var jwt = await GetUserTokenAsync(user, TokenScheme.TwoFactorRememberMe);
                    //return Ok(new ApiOkResponse(jwt).Result);

                    throw new NotSupportedException("TwoFactorRememberMeScheme Token is not supported!");
                }

                var jwt = await GetUserTokenAsync(user);
                return Ok(new ApiOkResponse(jwt).Result);
            }

            // If the token is incorrect, record the failure which also may cause the user to be locked out (protect against brute force attacks)
            await _userManager.AccessFailedAsync(user);

            _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
            return Unauthorized(new ApiUnauthorizedResponse("Invalid authenticator code."));
        }

        // POST api/auth/2fa/recovery
        [HttpPost("2fa/recovery")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.TwoFactorScheme)]
        public async Task<IActionResult> LoginWithRecoveryCode([FromBody]LoginWithRecoveryCodeViewModel model)
        {
            var userId = await RetrieveBearerTwoFactorUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to login with 2FA recovery code."));

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to login with 2FA recovery code."));
            }

            if (!await _signInManager.CanSignInAsync(user) || (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user)))
            {
                _logger.LogWarning("Unable to sign in user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "The specified user cannot sign in."));
            }

            // Strip spaces and hypens
            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
            if (result.Succeeded)
            {
                // When token is verified correctly, clear the access failed count used for lockout
                if (_userManager.SupportsUserLockout)
                    await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("User with ID {UserId} logged in with 2FA recovery code.", user.Id);

                var jwt = await GetUserTokenAsync(user);
                return Ok(new ApiOkResponse(jwt).Result);
            }

            // We don't protect against brute force attacks since codes are expected to be random
            _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}.", user.Id);
            return Unauthorized(new ApiUnauthorizedResponse("Invalid recovery code or already used."));
        }

        private async Task<string> RetrieveBearerTwoFactorUserIdAsync()
        {
            var result = await _signInManager.Context.AuthenticateAsync(JwtIssuerOptions.TwoFactorScheme);
            if (result?.Principal != null)
                return result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return null;
        }

        private async Task<string> GetUserTokenAsync(AppUser user, TokenScheme scheme = TokenScheme.Default)
        {
            async Task<ClaimsPrincipal> GetPrincipalAsync(string loginProvider = null)
            {
                if (scheme == TokenScheme.Default)
                    return await _signInManager.CreateUserPrincipalAsync(user);

                var authType = scheme == TokenScheme.TwoFactor ? JwtIssuerOptions.TwoFactorScheme : IdentityConstants.TwoFactorRememberMeScheme;
                var identity = new ClaimsIdentity(authType);

                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

                if (scheme == TokenScheme.TwoFactorRememberMe && _userManager.SupportsUserSecurityStamp)
                {
                    var stamp = await _userManager.GetSecurityStampAsync(user);
                    identity.AddClaim(new Claim(_signInManager.Options.ClaimsIdentity.SecurityStampClaimType, stamp));
                }

                return new ClaimsPrincipal(identity);
            }

            var principal = await GetPrincipalAsync();
            return await Tokens.GenerateJwtAsync(principal, _jwtFactory, scheme);
        }
    }
}