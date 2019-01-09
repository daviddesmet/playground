namespace Vue2API.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;

    using Data;
    using Helpers;
    using Models;
    using Models.Entities;
    using Infrastructure;
    using ViewModels;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _appDbContext;
        private readonly UrlEncoder _urlEncoder;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;

        public ProfileController(UserManager<AppUser> userManager, ApplicationDbContext appDbContext, UrlEncoder urlEncoder, IOptions<JwtIssuerOptions> jwtOptions, ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _urlEncoder = urlEncoder;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        // GET api/profile/me
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // Retrieve the user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unable to load user id claim.");
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to load user profile data."));
            }

            // Retrieve the profile info
            var profile = await _appDbContext.Profiles.Include(c => c.Identity).FirstOrDefaultAsync(c => c.Identity.Id == userId);
            if (profile is null)
            {
                _logger.LogWarning("Unable to load user profile with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to load user profile data."));
            }

            var enabled2fa = await _userManager.GetTwoFactorEnabledAsync(profile.Identity);

            return Ok(new ApiOkResponse(new
            {
                profile.Identity.Name,
                profile.Identity.Email,
                profile.Location,
                TwoFactorEnabled = enabled2fa
            }).Result);
        }

        // POST api/profile/password/change
        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to load user profile for password change."));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Unable to change user with ID {UserId} password. {Errors}", user.Id, result.Errors);
                return BadRequest(new ApiBadRequestResponse(ModelState, result.Errors.FirstOrDefault().Description));
            }

            _logger.LogInformation("User changed their password successfully.");

            return Ok();
        }

        // POST api/profile/password/set
        [HttpPost("password/set")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to load user profile for password set."));
            }

            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Unable to set user with ID {UserId} password.", user.Id);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to set user's password."));
            }

            return Ok();
        }

        // GET api/profile/authenticator
        [HttpGet("authenticator")]
        public async Task<IActionResult> Authenticator()
        {
            // Retrieve the user info
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                //var userId = _userManager.GetUserId(User);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to load user profile for authenticator."));
            }

            // Generate SharedKey and QR Code
            var model = new AuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return Ok(new ApiOkResponse(model).Result);
        }

        // POST api/profile/authenticator
        [HttpPost("authenticator")]
        public async Task<IActionResult> Authenticator([FromBody]AuthenticatorViewModel model)
        {
            // Retrieve the user info
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                //var userId = _userManager.GetUserId(User);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to load user profile for authenticator."));
            }

            // Strip spaces and hypens
            var pin = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var valid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, pin);
            if (!valid)
            {
                _logger.LogInformation("User with ID {UserId} used an invalid verification code.", user.Id);
                // TODO: Return the AuthenticatorViewModel with the error
                return BadRequest(new ApiBadRequestResponse(Errors.AddErrorToModelState("verification_failure", "Verification code is invalid.", ModelState)));
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true); // Does it stores any info regarding when it was activated? And if so, can we retrieve it?
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);

            // Below could be skipped and instead retrieve the code when GET api/profile/authenticator/codes
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return Ok();
        }

        // POST api/profile/authenticator/disable
        [HttpPost("authenticator/disable")]
        public async Task<IActionResult> DisableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to disable 2FA."));
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Unexpected error occured while disabling 2FA for user with ID {UserId}.", user.Id);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unexpected error occured while disabling 2FA."));
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2FA.", user.Id);
            return Ok();
        }

        // POST api/profile/authenticator/reset
        [HttpPost("authenticator/reset")]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to reset 2FA."));
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);
            return Ok();
        }

        // GET api/profile/authenticator/codes
        [HttpGet("authenticator/codes")]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            // Get the recovery codes generated when enabling 2FA
            var firstCodes = TempData[RecoveryCodesKey];
            if (firstCodes != null)
                return new OkObjectResult(firstCodes);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogWarning("Unable to load user with ID {UserId}.", userId);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to generate 2FA codes."));
            }

            if (!user.TwoFactorEnabled)
            {
                _logger.LogInformation("Unable to generate recovery codes for user with ID {UserId}.", user.Id);
                return BadRequest(new ApiBadRequestResponse(ModelState, "Unable to generate recovery codes as 2FA is not enabled."));
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return new OkObjectResult(recoveryCodes.ToArray());
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(AppUser user, AuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();

            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
                result.Append(unformattedKey.Substring(currentPosition));

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            // Provision a TOTP key for user 'email', to use with a service provided by Example, Inc:
            // Valid types are hotp and totp, to distinguish whether the key will be used for counter-based HOTP or for TOTP.
            // otpauth://TYPE/LABEL?PARAMETERS
            return string.Format(AuthenticatorUriFormat, _urlEncoder.Encode(_jwtOptions.Issuer), _urlEncoder.Encode(email), unformattedKey);
        }
    }
}
