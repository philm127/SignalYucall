using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalYucall.API.Controllers;
using SignalYucall.Application.SubscriberService.Commands;
using Xunit;

namespace SignalYucall.Tests.SignalYucall.API.Controllers.SubscriberController_Tests
{
    public class RegisterSubscriber_Test
    {
        private readonly SubscriberController _controller;
        private readonly Mock<IMediator> _mediatorMock;

        public RegisterSubscriber_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new SubscriberController(_mediatorMock.Object);
        }

        [Fact]
        public async void RegisterSubscriber_ReturnsOkResult_WhenCommandIsValid()
        {
            // Arrange
            var command = new RegisterSubscriberCommand { MSISDN = "123456789", Gender = 1, Country = "US", Dob = "01/01/2000", Yucall = 1 };
            var expectedResult = new RegisterResponse { Status = 0, CallID = "123456789" };
            _mediatorMock.Setup(x => x.Send(command, default))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.RegisterSubscriber(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<RegisterResponse>(okResult.Value);
            Assert.Equal(0,response.Status);
            Assert.Equal("123456789", response.CallID);
        }

        [Fact]
        public async void RegisterSubscriber_ReturnsBadRequest_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RegisterSubscriberCommand { MSISDN = "123456789", Gender = "", Country = "", Dob = "01/01/2000", Yucall = 0 };
            var expectedResult = new RegisterResponse { Result = "Error", CallID = "123456789" };
            _mediatorMock.Setup(x => x.Send(command, default))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.RegisterSubscriber(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsAssignableFrom<RegisterResponse>(badRequestResult.Value);
            Assert.Equal("Error", response.Result);
            Assert.Equal("123456789", response.CallID);
        }

        [Fact]
        public async Task RegisterSubscriber_ReturnsBadRequest_WhenMediatorThrowsException()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RegisterSubscriberCommand>(), default))
            .Throws(new Exception());
            var controller = new SubscriberController(mediator.Object);

            // Act
            var result = await controller.RegisterSubscriber(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}