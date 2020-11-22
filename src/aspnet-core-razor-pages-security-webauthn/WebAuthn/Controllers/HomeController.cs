namespace WebAuthn.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Rsk.AspNetCore.Fido;
    using Rsk.AspNetCore.Fido.Models;
    using Rsk.AspNetCore.Fido.Stores;

    using WebAuthn.Models;
    using WebAuthn.Security;

    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFidoAuthentication _fido;
        private readonly IFidoKeyStore _store;
        private readonly ILogger<HomeController> _logger;

        public HomeController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IFidoAuthentication fido, IFidoKeyStore store, ILogger<HomeController> logger)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _fido = fido ?? throw new ArgumentNullException(nameof(fido));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [Route("/CompleteSecurityKeyRegistration")]
        public async Task<IActionResult> CompleteSecurityKeyRegistration([FromBody] FidoRegistrationResponse registrationResponse)
        {
            // Parsing and Validating the Registration Data
            // The WebAuthn specification describes a 19-point procedure to validate the registration data.
            // https://w3c.github.io/webauthn/#registering-a-new-credential
            // Validates `clientDataJSON` like (challenge, origin, type, ...) and attestationObject (authData, fmt, attStmt)
            var result = await _fido.CompleteRegistration(registrationResponse);

            if (result.IsError)
                return BadRequest(result.ErrorDescription);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                RecoveryCodes = recoveryCodes.ToArray();
            }

            return Ok();
        }

        //[HttpPost]
        //[Route("/InitSecurityKeyAuthentication")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> InitSecurityKeyAuthentication(FidoLoginModel model)
        //{
        //    // Ensure the user has gone through the username & password screen first
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user is null)
        //        throw new InvalidOperationException($"Unable to load two-factor authentication user.");

        //    /*
        //    var credentials = (await _store.GetCredentialIdsForUser(user.Email)).ToList();
        //    if (!credentials.Any())
        //        throw new PublicKeyCredentialException("No keys registered for user");
        //    */

        //    var challenge = await _fido.InitiateAuthentication(user.Email); // model.UserId

        //    return View(challenge);
        //}

        [HttpPost]
        [Route("/CompleteSecurityKeyAuthentication")]
        public async Task<IActionResult> CompleteSecurityKeyAuthentication([FromBody] FidoAuthenticationResponse authenticationResponse)
        {
            // Parsing and Validating the Authentication Data
            // After the authentication data is fully validated, the signature is verified using the public key stored in the database during registration.
            // * The server retrieves the public key object associated with the user.
            // * The server uses the public key to verify the signature, which was generated using the `authenticatorData` bytes and a SHA-256 hash of the `clientDataJSON`.
            var result = await _fido.CompleteAuthentication(authenticationResponse);
            if (result.IsError)
                return BadRequest(result.ErrorDescription);

            if (result.IsSuccess)
                await _signInManager.TwoFactorSignInAsync(WebAuthnUserTwoFactorTokenProvider.ProviderName, string.Empty, false, false);

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
