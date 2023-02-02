using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalYucall.API.Controllers;
using SignalYucall.Application.Commands.UpdateSubscriber;
using SignalYucall.Application.Messages;
using SignalYucall.Application.SubscriberService.Commands;
using Xunit;

namespace SignalYucall.Tests.SignalYucall.API.Controllers.SubscriberController_Tests
{
    public class UpdateSubscriber_Test
    {
        [Fact]
        public async Task UpdateSubscriber_Should_Return_CreatedAtAction_Result_When_Successful()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var controller = new SubscriberController(mediatorMock.Object);
            var command = new UpdateSubscriberCommand();

            var result = new UpdateResponse
            {
                Status = 0,
                UserID = "8765432",
                Network = 0,
                CallID = "1"
            };
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateSubscriberCommand>(), default)).ReturnsAsync(result);

            // Act
            var actionResult = await controller.UpdateSubscriber(command);

            // Assert
            actionResult.Should().BeOfType<CreatedAtActionResult>();
            var createdAtActionResult = actionResult as CreatedAtActionResult;
            createdAtActionResult.Value.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task UpdateSubscriber_Should_Return_BadRequest_Result_When_BadRequestException_Is_Thrown()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var controller = new SubscriberController(mediatorMock.Object);
            var command = new UpdateSubscriberCommand();

            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateSubscriberCommand>(), default)).ThrowsAsync(new BadRequestException("error message"));

            // Act
            var actionResult = await controller.UpdateSubscriber(command);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Value.Should().BeOfType<Error>();
            var error = badRequestResult.Value as Error;
            error.Message.Should().Be("error message");
        }
    }
}


