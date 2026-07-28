using MbaCrm.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReferenceDataController : ControllerBase
    {
        [HttpGet("service-request-statuses")]
        public IActionResult GetServiceRequestStatuses()
        {
            return Ok(ServiceRequestStatuses.All);
        }

        [HttpGet("service-request-types")]
        public IActionResult GetServiceRequestTypes()
        {
            return Ok(ServiceRequestTypes.All);
        }
    }
}