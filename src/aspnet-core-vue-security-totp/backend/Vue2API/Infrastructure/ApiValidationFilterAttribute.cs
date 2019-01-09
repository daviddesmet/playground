namespace Vue2API.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ApiValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST" && !context.ModelState.IsValid)
                context.Result = new BadRequestObjectResult(new ApiBadRequestResponse(context.ModelState));

            base.OnActionExecuting(context);
        }
    }
}
