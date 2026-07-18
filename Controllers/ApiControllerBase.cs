using Microsoft.AspNetCore.Mvc;

namespace MbaCrm.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ObjectResult ApiProblem(
            int statusCode,
            string title,
            string detail)
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

            return StatusCode(statusCode, problemDetails);
        }
    }
}