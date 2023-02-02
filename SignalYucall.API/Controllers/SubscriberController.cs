using MediatR;
using Microsoft.AspNetCore.Mvc;
using SignalYucall.Application.Messages;
using SignalYucall.Application.SubscriberService.Commands;
using Swashbuckle.AspNetCore.Annotations;


namespace SignalYucall.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubscriberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new subscriber", Description = "Creates a new subscriber with MSISDN, Gender, Country, Dob and Yucall")]
        [SwaggerResponse(201, "User created successfully", typeof(RegisterResponse))]
        [SwaggerResponse(400, "Bad request", typeof(Error))]
        public async Task<IActionResult> RegisterSubscriber([FromBody] RegisterSubscriberCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(RegisterSubscriber), result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new Error { Message = ex.Message });
            }
        }


        [HttpPut]
        [SwaggerOperation(Summary = "Updates an existing subscriber", Description = "Updates an existing subscriber's MSISDN, Gender, Country, Dob and Call ID")]
        [SwaggerResponse(200, "User updated successfully", typeof(UpdateResponse))]
        [SwaggerResponse(400, "User not found", typeof(Error))]
        public async Task<IActionResult> UpdateSubscriber([FromBody] UpdateSubscriberCommand command)
        {
            try
            {
                var result = await _mediator.Send(command); 
                return CreatedAtAction(nameof(UpdateSubscriber), result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new Error { Message = ex.Message });
            }
        }

    }
}
