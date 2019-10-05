namespace WebAuthn.Security
{
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;

    public class WebAuthnUserTwoFactorTokenProvider : IUserTwoFactorTokenProvider<IdentityUser>
    {
        public const string ProviderName = "WebAuthn";

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(true);
        }

        public Task<string> GenerateAsync(string purpose, UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(ProviderName);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(true);
        }
    }
}
