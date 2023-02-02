using System.Collections.Generic;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SignalYucall.Application.AdvertService.Queries;
using SignalYucall.Application.Messages;
using Swashbuckle.AspNetCore.Annotations;

namespace SignalYucall.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertQueryController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AdvertQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [SwaggerOperation(Summary = "Fetches a list of adverts seen by a particular subscriber", Description = "Used by the Signal Server to get data from the Adtones Ad Server to generate the list of a ads received by a user over a set period.")]
        [SwaggerResponse(200, "Advert history query successful", typeof(AdvertHistoryListQueryResponse))]
        [SwaggerResponse(400, "Bad request", typeof(BadRequestResult))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundResult))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorResult))]
        public async Task<ActionResult<AdvertHistoryListQueryResponse>> AdvertHistoryQuery([FromBody] AdvertHistoryListQuery request)
        {
            try
            {
                var result = await _mediator.Send(request);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}