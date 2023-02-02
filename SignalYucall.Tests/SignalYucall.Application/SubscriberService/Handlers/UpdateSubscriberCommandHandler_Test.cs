using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SignalYucall.Application.Helpers;
using SignalYucall.Application.Persistence.Commands;
using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Application.SubscriberService.Handlers;
using SignalYucall.Application.SubscriberService.Repositories;
using Xunit;

namespace SignalYucall.Tests.SignalYucall.Application.SubscriberService.Handlers
{
    public class UpdateSubscriberCommandHandler_Test
    {
        private readonly Mock<ISubscriberHelper> _subscriberHelperMock;
        private readonly Mock<IUserProfileCommandRepository> _userProfileCommandRepositoryMock;
        private readonly Mock<ICountryHelper> _countryHelperMock;
        private readonly Mock<ILogger<UpdateSubscriberCommandHandler>> _loggerMock;

        public UpdateSubscriberCommandHandler_Test()
        {
            _subscriberHelperMock = new Mock<ISubscriberHelper>();
            _userProfileCommandRepositoryMock = new Mock<IUserProfileCommandRepository>();
            _countryHelperMock = new Mock<ICountryHelper>();
            _loggerMock = new Mock<ILogger<UpdateSubscriberCommandHandler>>();
        }


        [Fact]
        public async Task UpdateSubscriberCommandHandler_Should_Return_Error_When_Country_Is_Invalid()
        {
            // Arrange
            var mockSubscriberHelper = new Mock<ISubscriberHelper>();
            var mockUserProfileCommand = new Mock<IUserProfileCommandRepository>();
            var mockCountryHelper = new Mock<ICountryHelper>();
            mockCountryHelper.Setup(x => x.IsCountryShortNameValid("InvalidCountry")).ReturnsAsync(false);
            var mockLogger = new Mock<ILogger<UpdateSubscriberCommandHandler>>();

            var request = new UpdateSubscriberCommand
            {
                CallID = "TestCallID",
                MSISDN = "TestMSISDN",
                Country = "InvalidCountry"
            };

            var sut = new UpdateSubscriberCommandHandler(mockSubscriberHelper.Object, mockUserProfileCommand.Object, mockCountryHelper.Object, mockLogger.Object);

            // Act
            var response = await sut.Handle(request, default);

            // Assert
            response.Status.Should().Be(-1);
            response.CallID.Should().Be("TestCallID");
            response.UserID.Should().Be("TestMSISDN");
        }



        [Fact]
        public async Task UpdateSubscriberCommandHandler_Should_Return_Error_When_Subscriber_Is_Not_Registered()
        {
            // Arrange
            var countryHelper = new Mock<ICountryHelper>();
            countryHelper.Setup(x => x.IsCountryShortNameValid(It.IsAny<string>())).Returns(Task.FromResult(true));

            var subscriberHelper = new Mock<ISubscriberHelper>();
            subscriberHelper.Setup(x => x.GetSubscriberByMaskedId(It.IsAny<string>())).Returns(Task.FromResult<UserProfile>(null));

            var userProfileCommand = new Mock<IUserProfileCommandRepository>();

            var logger = new Mock<ILogger<UpdateSubscriberCommandHandler>>();

            var sut = new UpdateSubscriberCommandHandler(subscriberHelper.Object, userProfileCommand.Object, countryHelper.Object, logger.Object);

            // Act
            var result = await sut.Handle(new UpdateSubscriberCommand { CallID = "123456", MSISDN = "1234567890", Country = "US" }, CancellationToken.None);

            // Assert
            result.Status.Should().Be(-1);
            result.CallID.Should().Be("123456");
            result.Network.Should().Be(0);
            result.UserID.Should().Be("1234567890");
        }


        [Fact]
        public async Task UpdateSubscriberCommandHandler_Should_Update_User_Profile_Successfully()
        {
            // Arrange
            var userProfileCommandMock = new Mock<IUserProfileCommandRepository>();
            var countryHelperMock = new Mock<ICountryHelper>();
            var subscriberHelperMock = new Mock<ISubscriberHelper>();
            var loggerMock = new Mock<ILogger<UpdateSubscriberCommandHandler>>();
            var request = new UpdateSubscriberCommand { CallID = "123", MSISDN = "987654321", Country = "US" };

            countryHelperMock.Setup(c => c.IsCountryShortNameValid(request.Country)).ReturnsAsync(true);
            subscriberHelperMock.Setup(s => s.GetSubscriberByMaskedId(request.MSISDN)).ReturnsAsync(new UserProfile { MaskedId = request.MSISDN, OperatorId = "123" });
            userProfileCommandMock.Setup(u => u.Update(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

            var handler = new UpdateSubscriberCommandHandler(subscriberHelperMock.Object, userProfileCommandMock.Object, countryHelperMock.Object, loggerMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Status.Should().Be(0);
            result.CallID.Should().Be(request.CallID);
            result.Network.Should().Be("123");
            result.UserID.Should().Be(request.MSISDN);
            countryHelperMock.Verify(c => c.IsCountryShortNameValid(request.Country), Times.Once);
            subscriberHelperMock.Verify(s => s.GetSubscriberByMaskedId(request.MSISDN), Times.Once);
            userProfileCommandMock.Verify(u => u.Update(It.IsAny<UserProfile>()), Times.Once);
        }



        [Fact]
        public async Task UpdateSubscriberCommandHandler_Should_Log_Start_And_End_Of_Processing()
        {
            // Arrange
            var subscriberHelperMock = new Mock<ISubscriberHelper>();
            var userProfileCommandMock = new Mock<IUserProfileCommandRepository>();
            var countryHelperMock = new Mock<ICountryHelper>();
            var loggerMock = new Mock<ILogger<RegisterSubscriberCommandHandler>>();

            subscriberHelperMock.Setup(x => x.GetSubscriberByMaskedId(It.IsAny<string>())).ReturnsAsync((UserProfile)null);
            countryHelperMock.Setup(x => x.IsCountryShortNameValid(It.IsAny<string>())).ReturnsAsync(true);

            var request = new UpdateSubscriberCommand { CallID = "call-id", Country = "country", MSISDN = "msisdn" };
            var handler = new UpdateSubscriberCommandHandler(subscriberHelperMock.Object, userProfileCommandMock.Object,
                countryHelperMock.Object, loggerMock.Object);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            loggerMock.Verify(x => x.LogInformation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            loggerMock.Verify(x => x.LogInformation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }


        [Fact]
        public async Task UpdateSubscriberCommandHandler_Should_Log_Error_When_Country_Is_Invalid()
        {
            // Arrange
            var mockCountryHelper = new Mock<ICountryHelper>();
            var mockSubscriberHelper = new Mock<ISubscriberHelper>();
            var mockUserProfileCommand = new Mock<IUserProfileCommandRepository>();
            var mockLogger = new Mock<ILogger<UpdateSubscriberCommandHandler>>();

            mockCountryHelper.Setup(x => x.IsCountryShortNameValid(It.IsAny<string>()))
                .ReturnsAsync(false);

            var sut = new UpdateSubscriberCommandHandler(mockSubscriberHelper.Object,
                                                        mockUserProfileCommand.Object,
                                                        mockCountryHelper.Object,
                                                        mockLogger.Object);

            // Act
            var result = await sut.Handle(new UpdateSubscriberCommand { CallID = "123", MSISDN = "msisdn", Country = "invalidCountry" }, CancellationToken.None);

            // Assert
            mockLogger.Verify(x => x.LogError(It.IsAny<string>(), "invalidCountry"), Times.Once);
        }



        [Fact]
        public async Task UpdateSubscriberCommandHandler_Should_Log_Error_When_Subscriber_Is_Not_Registered()
        {
            // Arrange
            var subscriberHelper = new Mock<ISubscriberHelper>();
            var userProfileCommand = new Mock<IUserProfileCommandRepository>();
            var countryHelper = new Mock<ICountryHelper>();
            var logger = new Mock<ILogger<UpdateSubscriberCommandHandler>>();
            var request = new UpdateSubscriberCommand { MSISDN = "123456789", Country = "USA", CallID = "test-call-id" };
            subscriberHelper.Setup(x => x.GetSubscriberByMaskedId(It.IsAny<string>())).ReturnsAsync((UserProfileModelDto)null);
            var handler = new UpdateSubscriberCommandHandler(subscriberHelper.Object, userProfileCommand.Object, countryHelper.Object, logger.Object);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            logger.Verify(x => x.LogError(It.IsAny<string>(), request.MSISDN), Times.Once);
        }



    }
}
