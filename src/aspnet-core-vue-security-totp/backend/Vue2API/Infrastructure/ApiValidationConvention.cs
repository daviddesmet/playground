namespace Vue2API.Infrastructure
{
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    public class ApiValidationConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controllerModel in application.Controllers)
                controllerModel.Filters.Add(new ApiValidationFilterAttribute());
        }
    }
}
