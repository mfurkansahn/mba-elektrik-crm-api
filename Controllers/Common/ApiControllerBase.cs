using Microsoft.AspNetCore.Mvc;

namespace MbaCrm.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ObjectResult ApiProblem(
    int statusCode,
    string title,
    string detail,
    object? errors = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = HttpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                HttpContext.TraceIdentifier;

            if (errors is not null)
            {
                problemDetails.Extensions["errors"] = errors;
            }

            return StatusCode(statusCode, problemDetails);
        }
    }
}