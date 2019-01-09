namespace Vue2API.Infrastructure
{
    using System.Net;
    using Newtonsoft.Json;

    public class ApiResponse
    {
        public ApiResponse(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = (HttpStatusCode)statusCode;
            Message = message ?? GetDefaultMessageForStatusCode((HttpStatusCode)statusCode);
        }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        private static string GetDefaultMessageForStatusCode(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return "Resource not found";
                case HttpStatusCode.InternalServerError:
                    return "An unhandled error occurred";
                default:
                    return null;
            }
        }
    }
}
