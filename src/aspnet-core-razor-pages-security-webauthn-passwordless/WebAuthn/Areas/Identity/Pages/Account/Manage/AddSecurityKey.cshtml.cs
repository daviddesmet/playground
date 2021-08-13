using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;
using Rsk.AspNetCore.Fido.Models;
using WebAuthn.Extensions;

namespace WebAuthn.Areas.Identity.Pages.Account.Manage
{
    public class AddSecurityKeyModel : PageModel
    {
        //[TempData] // Does not work with complex data, feed it at OnGet
        public Base64FidoRegistrationChallenge KeyChallenge { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            KeyChallenge = TempData.Get<Base64FidoRegistrationChallenge>(nameof(KeyChallenge));
            if (KeyChallenge is null)
                return RedirectToPage("./TwoFactorAuthentication"); //throw new InvalidOperationException($"Unable to load key challenge."); //return NotFound($"Unable to load key challenge.");

            return Page();
        }
    }
}