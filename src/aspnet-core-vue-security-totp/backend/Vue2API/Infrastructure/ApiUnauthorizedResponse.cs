namespace Vue2API.Infrastructure
{
    using System.Net;

    public class ApiUnauthorizedResponse : ApiResponse
    {
        public ApiUnauthorizedResponse(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}
