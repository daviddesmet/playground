namespace Vue2API.Auth
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IJwtFactory
    {
        double GetExpiration();

        Task<string> CreateTokenAsync(ClaimsPrincipal principal, TokenScheme scheme = TokenScheme.Default);
    }
}