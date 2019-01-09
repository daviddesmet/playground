namespace Vue2API.Infrastructure
{
    using System.Net;

    public class ApiOkResponse : ApiResponse
    {
        public ApiOkResponse(object result) : base(HttpStatusCode.OK)
        {
            Result = result;
        }

        public object Result { get; }
    }
}
