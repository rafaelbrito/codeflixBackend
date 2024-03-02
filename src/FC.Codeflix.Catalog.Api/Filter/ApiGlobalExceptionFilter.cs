using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FC.Codeflix.Catalog.Api.Filter
{
    public class ApiGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _environment;
        public ApiGlobalExceptionFilter(IHostEnvironment environment)
                    => _environment = environment;
        public void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails();
            var exception = context.Exception;

            if (_environment.IsDevelopment())
                details.Extensions.Add("StackTtrace", exception.StackTrace);

            if (exception is EntityValidationException)
            {
                details.Title = "One or more validation errors ocurred";
                details.Type = "UnprocessableEntity";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = exception!.Message;

            }
            else if (exception is NotFoundException)
            {
                details.Title = "Not Found";
                details.Type = "NotFound";
                details.Status = StatusCodes.Status404NotFound;
                details.Detail = exception!.Message;

            }
            else if(exception is RelatedAggregateException)
            {
                details.Title = "Invalid Related Aggregate";
                details.Type = "RelatedAggregate";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = exception!.Message;
            }
            else
            {
                details.Title = "An unexpected error ocurred";
                details.Type = "unexpectedError";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = exception.Message;
            }

            context.HttpContext.Response.StatusCode = (int)details.Status;
            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
