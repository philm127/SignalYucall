using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SignalYucall.Application.AdtonesSubscriberService;
using SignalYucall.Application.Helpers;
using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Application.SubscriberService.Handlers;
using SignalYucall.Application.SubscriberService.Models;
using SignalYucall.Infrastructure.Helpers;
using Xunit;
using static Dapper.SqlMapper;

namespace SignalYucall.Tests.SignalYucall.Application.SubscriberService.Handlers
{
    public class RegisterSubscriberCommandHandlerTests
    {
        private readonly RegisterSubscriberCommandHandler _handler;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<RegisterSubscriberCommandHandler>> _loggerMock;
        private readonly RegisterSubscriberCommand _validCommand;

        public RegisterSubscriberCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<RegisterSubscriberCommandHandler>>();
            _handler = new RegisterSubscriberCommandHandler(_mediatorMock.Object, _loggerMock.Object);
            _validCommand = new RegisterSubscriberCommand
            {
                MSISDN = "0123456789",
                Country = "country",
                Gender = 1,
                Dob = "dob",
                Yucall = 1,
                CallID = "callid"
            };
        }

        [Fact]
        public async Task RegisterSubscriberCommandHandler_HandlesValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var msisdn = "123456789";
            var country = "US";
            var dob = "01/01/2000";
            var gender = 1;
            var yucall = 1;

            var request = new RegisterSubscriberCommand(msisdn, gender, country, dob, yucall);
            var mediatorMock = new Mock<IMediator>();
            var subscriberHelperMock = new Mock<ISubscriberHelper>();
            var countryHelperMock = new Mock<ICountryHelper>();
            var adtonesRemoteSubscriberServiceMock = new Mock<IAdtonesRemoteSubscriberService>();
            var loggerMock = new Mock<ILogger<RegisterSubscriberCommandHandler>>();

            subscriberHelperMock.Setup(sh => sh.IsSubscriberAlreadyRegistered(It.IsAny<string>())).ReturnsAsync(false);
            countryHelperMock.Setup(ch => ch.IsCountryShortNameValid(It.IsAny<string>())).ReturnsAsync(true);
            adtonesRemoteSubscriberServiceMock.Setup(arss => arss.GetSubscriberRegister(It.IsAny<string>()))
                .ReturnsAsync("123456");

            var handler = new RegisterSubscriberCommandHandler(
                mediatorMock.Object,
                subscriberHelperMock.Object,
                countryHelperMock.Object,
                adtonesRemoteSubscriberServiceMock.Object,
                loggerMock.Object);

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RegisterResponse>(result);
            Assert.Equal("123456", result.MaskedId);
        }


        //In this test, we are using Moq to mock the dependencies of the RegisterSubscriberCommandHandler.
        //We are setting up the msisdnHelper mock to return true for the IsInvalidMsisdn method, which should result in an 
        //InvalidMsisdnException being thrown by the handler when it processes the request.Finally, we use the 
        //Assert.ThrowsAsync method to verify that the expected exception is thrown when the Handle method is executed.
        [Fact]
        public async Task RegisterSubscriberCommandHandler_ThrowsException_WhenMsisdnIsInvalid()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var msisdnHelper = new Mock<IMsisdnHelper>();
            var countryHelper = new Mock<ICountryHelper>();
            var subscriberHelper = new Mock<ISubscriberHelper>();
            var adtonesRemoteSubscriberService = new Mock<IAdtonesRemoteSubscriberService>();
            var logger = new Mock<ILogger<RegisterSubscriberCommandHandler>>();

            var invalidMsisdn = "InvalidMSISDN";

            msisdnHelper.Setup(x => x.IsInvalidMsisdn(invalidMsisdn)).Returns(true);

            var request = new RegisterSubscriberCommand
            {
                MSISDN = invalidMsisdn,
                Country = "InvalidCountry",
                Gender = 4,
                Dob = "01/01/2000",
                Yucall = false
            };

            var handler = new RegisterSubscriberCommandHandler(mediator.Object, msisdnHelper.Object, countryHelper.Object, subscriberHelper.Object, adtonesRemoteSubscriberService.Object, logger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidMsisdnException>(() => handler.Handle(request, default));
        }


        [Fact]
        public async void RegisterSubscriberCommandHandler_ThrowsException_WhenCountryShortNameIsInvalid()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var countryHelperMock = new Mock<ICountryHelper>();
            var subscriberHelperMock = new Mock<ISubscriberHelper>();
            var loggerMock = new Mock<ILogger<RegisterSubscriberCommandHandler>>();
            var adtonesRemoteSubscriberServiceMock = new Mock<IAdtonesRemoteSubscriberService>();

            var request = new RegisterSubscriberCommand
            {
                MSISDN = "1235551234",
                Gender = 1,
                Country = "InvalidCountry",
                Dob = "01/01/1980",
                Yucall = 1
            };

            countryHelperMock.Setup(ch => ch.IsCountryShortNameValid(request.Country)).ReturnsAsync(false);

            var handler = new RegisterSubscriberCommandHandler(mediatorMock.Object, countryHelperMock.Object, subscriberHelperMock.Object, loggerMock.Object, adtonesRemoteSubscriberServiceMock.Object);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidCountryException>(() => handler.Handle(request, default));

            // Assert
            Assert.Equal("Invalid country name", exception.Message);
            loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetErrorResponse>(), default), Times.Never);
            adtonesRemoteSubscriberServiceMock.Verify(a => a.GetSubscriberRegister(It.IsAny<string>()), Times.Never);
            subscriberHelperMock.Verify(s => s.IsSubscriberAlreadyRegistered(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async void RegisterSubscriberCommandHandler_ThrowsException_WhenSubscriberIsAlreadyRegistered()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockCountryHelper = new Mock<ICountryHelper>();
            var mockMsisdnHelper = new Mock<IMsisdnHelper>();
            var mockSubscriberHelper = new Mock<ISubscriberHelper>();
            var mockAdtonesRemoteSubscriberService = new Mock<IAdtonesRemoteSubscriberService>();
            var mockLogger = new Mock<ILogger<RegisterSubscriberCommandHandler>>();
            var command = new RegisterSubscriberCommand
            {
                MSISDN = "InvalidMsisdn",
                Country = "InvalidCountry",
                Gender = 1,
                Dob = "01/01/2000",
                Yucall = 1
            };

            mockMsisdnHelper.Setup(x => x.IsInvalidMsisdn(It.IsAny<string>())).Returns(false);
            mockCountryHelper.Setup(x => x.IsCountryShortNameValid(It.IsAny<string>())).ReturnsAsync(true);
            mockSubscriberHelper.Setup(x => x.IsSubscriberAlreadyRegistered(It.IsAny<string>())).ReturnsAsync(true);

            var handler = new RegisterSubscriberCommandHandler(mockMediator.Object, mockCountryHelper.Object, mockMsisdnHelper.Object, mockSubscriberHelper.Object, mockAdtonesRemoteSubscriberService.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            mockSubscriberHelper.Verify(x => x.IsSubscriberAlreadyRegistered(It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task RegisterSubscriberCommandHandler_CallsGetSubscriberRegister_WhenRequestIsValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<RegisterSubscriberCommandHandler>>();
            var mockMsisdnHelper = new Mock<IMsisdnHelper>();
            var mockCountryHelper = new Mock<ICountryHelper>();
            var mockSubscriberHelper = new Mock<ISubscriberHelper>();
            var mockAdtonesRemoteSubscriberService = new Mock<IAdtonesRemoteSubscriberService>();
            mockMsisdnHelper.Setup(x => x.IsInvalidMsisdn(It.IsAny<string>())).Returns(false);
            mockCountryHelper.Setup(x => x.IsCountryShortNameValid(It.IsAny<string>())).ReturnsAsync(true);
            mockSubscriberHelper.Setup(x => x.IsSubscriberAlreadyRegistered(It.IsAny<string>())).ReturnsAsync(false);
            mockAdtonesRemoteSubscriberService.Setup(x => x.GetSubscriberRegister(It.IsAny<string>())).ReturnsAsync("maskedID");

            var request = new RegisterSubscriberCommand
            {
                MSISDN = "msisdn",
                Gender = 2,
                Country = "country",
                Dob = "01/01/2000",
                Yucall = 1
            };

            var sut = new RegisterSubscriberCommandHandler(mockMediator.Object, mockLogger.Object, mockMsisdnHelper.Object,
                mockCountryHelper.Object, mockSubscriberHelper.Object, mockAdtonesRemoteSubscriberService.Object);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            mockAdtonesRemoteSubscriberService.Verify(x => x.GetSubscriberRegister(It.IsAny<string>()), Times.Once());
        }



        [Fact]
        public async Task RegisterSubscriberCommandHandler_ReturnsErrorResponse_WhenGetSubscriberRegisterThrowsException()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockMsisdnHelper = new Mock<IMsisdnHelper>();
            var mockCountryHelper = new Mock<ICountryHelper>();
            var mockSubscriberHelper = new Mock<ISubscriberHelper>();
            var mockAdtonesRemoteSubscriberService = new Mock<IAdtonesRemoteSubscriberService>();
            var mockLogger = new Mock<ILogger<RegisterSubscriberCommandHandler>>();
            var request = new RegisterSubscriberCommand { MSISDN = "invalid", Country = "invalid", Gender = 1, Dob = "01/01/2000" };

            mockMsisdnHelper.Setup(x => x.IsInvalidMsisdn(request.MSISDN)).Returns(false);
            mockCountryHelper.Setup(x => x.IsCountryShortNameValid(request.Country)).Returns(Task.FromResult(true));
            mockSubscriberHelper.Setup(x => x.IsSubscriberAlreadyRegistered(request.MSISDN)).Returns(Task.FromResult(false));
            mockAdtonesRemoteSubscriberService.Setup(x => x.GetSubscriberRegister(request.MSISDN)).ThrowsAsync(new Exception());

            var handler = new RegisterSubscriberCommandHandler(mockMediator.Object, mockMsisdnHelper.Object, mockCountryHelper.Object, mockSubscriberHelper.Object, mockAdtonesRemoteSubscriberService.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsType<ErrorResponse>(result);
            mockMsisdnHelper.Verify(x => x.IsInvalidMsisdn(request.MSISDN), Times.Once);
            mockCountryHelper.Verify(x => x.IsCountryShortNameValid(request.Country), Times.Once);
            mockSubscriberHelper.Verify(x => x.IsSubscriberAlreadyRegistered(request.MSISDN), Times.Once);
            mockAdtonesRemoteSubscriberService.Verify(x => x.GetSubscriberRegister(request.MSISDN), Times.Once);
        }


        [Fact]
        public async Task RegisterSubscriberCommandHandler_MapsValidResponse_ToSuccessResponse()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var subscriberHelper = new Mock<ISubscriberHelper>();
            var countryHelper = new Mock<ICountryHelper>();
            var adtonesRemoteSubscriberService = new Mock<IAdtonesRemoteSubscriberService>();
            var logger = new Mock<ILogger<RegisterSubscriberCommandHandler>>();
            var command = new RegisterSubscriberCommand { MSISDN = "1234567890", Country = "US", Gender = 1, Dob = "01/01/2000", Yucall = true };
            var maskedID = "XXXXXXXXXX";
            var expectedResponse = new RegisterResponse { CallID = command.CallID, MaskedID = maskedID };
            var handler = new RegisterSubscriberCommandHandler(mediator.Object, subscriberHelper.Object, countryHelper.Object, adtonesRemoteSubscriberService.Object, logger.Object);
            subscriberHelper.Setup(x => x.IsSubscriberAlreadyRegistered(command.MSISDN)).ReturnsAsync(false);
            countryHelper.Setup(x => x.IsCountryShortNameValid(command.Country)).ReturnsAsync(true);
            adtonesRemoteSubscriberService.Setup(x => x.GetSubscriberRegister(command.MSISDN)).ReturnsAsync(maskedID);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.Should().BeOfType<SuccessResponse>();
            result.As<SuccessResponse>().MaskedID.Should().Be(expectedResponse.MaskedID);
        }


    }
}
