using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;
using Rsk.AspNetCore.Fido.Models;

namespace WebAuthn.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithSecurityKey : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IFidoAuthentication _fido;
        private readonly ILogger<LoginWithSecurityKey> _logger;

        public LoginWithSecurityKey(SignInManager<IdentityUser> signInManager, IFidoAuthentication fido, ILogger<LoginWithSecurityKey> logger)
        {
            _signInManager = signInManager;
            _fido = fido;
            _logger = logger;
        }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public Base64FidoAuthenticationChallenge AuthChallenge { get; set; }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/";
            RememberMe = rememberMe;

            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user is null)
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");

            /*
            var credentials = (await _store.GetCredentialIdsForUser(user.Email)).ToList();
            if (!credentials.Any())
                throw new PublicKeyCredentialException("No keys registered for user");
            */

            // Creates the required fields for the `publicKeyCredentialRequestOptions` which are: 
            // Base64Challenge, RelyingPartyId, UserId & Base64KeyIds if any
            AuthChallenge = (await _fido.InitiateAuthentication(user.Email)).ToBase64Dto(); // model.UserId

            return Page();
        }
    }
}
