using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SignalYucall.Application.ProfileService.Handlers;
using SignalYucall.Application.ProfileService.Queries;
using Xunit;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SignalYucall.Tests.SignalYucall.Infrastructure.UserProfileRepositories_Tests.UserProfileQuery_Tests;
using SignalYucall.Application.Persistence.Queries;

namespace SignalYucall.Tests.SignalYucall.API.Controllers.Profiles_Tests
{
    public class GetSubscriberMarketingList_Test : IClassFixture<ProfileControllerTestFixture>
    {
        private readonly ProfileControllerTestFixture _fixture;

        public GetSubscriberMarketingList_Test(ProfileControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetSubscriberProfiles_ShouldReturnListOfSubscriberProfiles()
        {
            // Arrange
            var mockUserProfileRepository = new Mock<IUserProfileQueryRepository>();
            mockUserProfileRepository
                .Setup(repo => repo.GetSubscriberMarketingList(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "User1", "User2" });

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(mediator => mediator.Send(It.IsAny<SubscriberProfilesQueryCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SubscriberProfilesQueryResult { UserList = new List<SubscriberProfilesString> { new SubscriberProfilesString { UserID = "User1" }, new SubscriberProfilesString { UserID = "User2" } } });

            var controller = _fixture.GetProfileController(mockUserProfileRepository.Object, mockMediator.Object);

            // Act
            var result = await controller.GetSubscriberProfiles(1, "2000-01-01", "2020-01-01");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<SubscriberProfilesQueryResult>();
            var subscriberProfiles = objectResult.Value as SubscriberProfilesQueryResult;
            subscriberProfiles.UserList.Should().HaveCount(2);
            subscriberProfiles.UserList.Should().Contain(x => x.UserID == "User1");
            subscriberProfiles.UserList.Should().Contain(x => x.UserID == "User2");
        }

        [Fact]
        public async Task GetSubscriberProfiles_ShouldReturnNotFoundResult_WhenNoDataFound()
        {
            // Arrange
            var mockUserProfileRepository = new Mock<IUserProfileQueryRepository>();
            mockUserProfileRepository
            .Setup(repo => repo.GetSubscriberMarketingList(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string>());
            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(mediator => mediator.Send(It.IsAny<SubscriberProfilesQueryCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SubscriberProfilesQueryResult { UserList = new List<SubscriberProfilesString>() });

            var controller = _fixture.GetProfileController(mockUserProfileRepository.Object, mockMediator.Object);

            // Act
            var result = await controller.GetSubscriberProfiles(1, "2000-01-01", "2020-01-01");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }


        [Fact]
        public async Task GetSubscriberProfiles_ShouldReturnBadRequestResult_WhenGenderIsNullOrEmpty()
        {
            // Arrange
            var mockUserProfileRepository = new Mock<IUserProfileQueryRepository>();
            var mockMediator = new Mock<IMediator>();

            var controller = _fixture.GetProfileController(mockUserProfileRepository.Object, mockMediator.Object);

            // Act
            var result = await controller.GetSubscriberProfiles("", "2000-01-01", "2020-01-01");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Gender is required.");
        }


        [Fact]
        public async Task GetSubscriberProfiles_ShouldReturnBadRequestResult_WhenDateRangeIsInvalid()
        {
            // Arrange
            var mockUserProfileRepository = new Mock<IUserProfileQueryRepository>();
            var mockMediator = new Mock<IMediator>();

            var controller = _fixture.GetProfileController(mockUserProfileRepository.Object, mockMediator.Object);

            // Act
            var result = await controller.GetSubscriberProfiles(1, "2000-01-01", "2010-01-01");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Start date must be before end date.");
        }



    }
}
