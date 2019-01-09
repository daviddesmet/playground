namespace Vue2API.Infrastructure
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Extensions;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IStringLocalizer<ErrorHandlingMiddleware> _localizer;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IStringLocalizer<ErrorHandlingMiddleware> localizer)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //await HandleExceptionAsync(context, ex, _logger, _localizer);
                _logger.LogError(EventIds.GlobalException, ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            if (!context.Response.HasStarted)
            {
                //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.ContentType = "application/json";

                var response = new ApiResponse((HttpStatusCode)context.Response.StatusCode);
                var json = JsonConvert.SerializeObject(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
