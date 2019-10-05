using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Stores;
using WebAuthn.Extensions;
using WebAuthn.Models;

namespace WebAuthn.Areas.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFidoAuthentication _fido;
        private readonly IFidoKeyStore _store;
        private readonly ILogger<TwoFactorAuthenticationModel> _logger;

        public TwoFactorAuthenticationModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IFidoAuthentication fido, IFidoKeyStore store, ILogger<TwoFactorAuthenticationModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fido = fido;
            _store = store;
            _logger = logger;
        }

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        public bool HasSecurityKey { get; set; }

        [BindProperty]
        public List<RegisteredSecurityKeyModel> SecurityKeys { get; set; }

        [BindProperty]
        public InputSecurityKeyModel Input { get; set; }

        public class InputSecurityKeyModel
        {
            [Required]
            [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [DataType(DataType.Text)]
            [Display(Name = "Device Name")]
            public string DeviceName { get; set; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            SecurityKeys = new List<RegisteredSecurityKeyModel>();
            var credentialIds = (await _store.GetCredentialIdsForUser(user.Email)).ToList();
            HasSecurityKey = credentialIds.Any();

            if (HasSecurityKey)
            {
                var id = 0;
                foreach (var credId in credentialIds)
                {
                    id += 1;
                    var cred = await _store.GetCredentialById(credId);
                    SecurityKeys.Add(new RegisteredSecurityKeyModel { Id = WebEncoders.Base64UrlEncode(cred.CredentialId), DeviceName = cred.DisplayFriendlyName });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _signInManager.ForgetTwoFactorClientAsync();
            StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveKeyAsync(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var cred = await _store.GetCredentialById(WebEncoders.Base64UrlDecode(id));
            if (cred != null)
            {
                // TODO: Add FIDO credential removal logic
                //await _store.Remove(cred);
                (_store as InMemoryFidoKeyStore).Keys.Remove(cred); // Workaround...
            }

            // Check if there's any fido key left and any authenticator configured
            var credentialIds = (await _store.GetCredentialIdsForUser(user.Email)).ToList();
            if (!credentialIds.Any() && await _userManager.GetAuthenticatorKeyAsync(user) is null)
                await _userManager.SetTwoFactorEnabledAsync(user, false);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddKeyAsync()
        {
            /*
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            
            //var userId = await _userManager.GetUserIdAsync(user);
            */

            // Creates the required fields for the `publicKeyCredentialCreationOptions` which are: 
            // Base64Challenge, RelyingPartyId & UserId
            var challenge = await _fido.InitiateRegistration(User.Identity.Name, Input.DeviceName); // could use: user.Email

            TempData.Set("KeyChallenge", challenge); // Complex data, cannot use: TempData["KeyChallenge"] = challenge;

            return RedirectToPage("./AddSecurityKey");
        }
    }
}