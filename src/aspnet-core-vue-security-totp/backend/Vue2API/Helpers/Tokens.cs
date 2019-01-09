namespace Vue2API.Helpers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Auth;

    public class Tokens
    {
        public static async Task<string> GenerateJwtAsync(ClaimsPrincipal principal, IJwtFactory jwtFactory, TokenScheme scheme = TokenScheme.Default, JsonSerializerSettings serializerSettings = null)
        {
            var response = new
            {
                id = principal.FindFirstValue(ClaimTypes.NameIdentifier),
                auth_token = await jwtFactory.CreateTokenAsync(principal, scheme),
                expires_in = (int)jwtFactory.GetExpiration()
            };

            return JsonConvert.SerializeObject(response, serializerSettings ?? new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}
