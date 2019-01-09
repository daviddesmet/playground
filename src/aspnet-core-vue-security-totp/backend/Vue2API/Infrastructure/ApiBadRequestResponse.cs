namespace Vue2API.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class ApiBadRequestResponse : ApiResponse
    {
        public ApiBadRequestResponse(ModelStateDictionary modelState) : base(HttpStatusCode.BadRequest)
        {
            if (modelState.IsValid)
                throw new ArgumentException("ModelState must be invalid", nameof(modelState));

            Errors = modelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();
        }

        public ApiBadRequestResponse(ModelStateDictionary modelState, params string[] messages) : base(HttpStatusCode.BadRequest)
        {
            if (messages is null || messages.Any(m => string.IsNullOrWhiteSpace(m)))
                throw new ArgumentException("ModelState message should not be empty", nameof(modelState));

            foreach (var msg in messages)
                modelState.AddModelError(string.Empty, msg);

            Errors = modelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();
        }

        public IEnumerable<string> Errors { get; }
    }
}
