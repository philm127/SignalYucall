using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignalYucall.Application.Messages;
using SignalYucall.Application.ProfileService.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace SignalYucall.API.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IMediator mediator, ILogger<ProfileController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Fetches a list of Subscribers by Profile", Description = "Gets a list of subscribers who match Gender and Dob between dates")]
        [SwaggerResponse(200, "Success", typeof(SubscriberProfilesQueryResult))]
        [SwaggerResponse(400, "Bad Request", typeof(Error))]
        public async Task<IActionResult> GetSubscriberMarketingList([FromQuery] SubscriberProfilesQueryCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch subscribers by profile");
                return BadRequest(new Error { Message = ex.Message });
            }
        }
    }
}
