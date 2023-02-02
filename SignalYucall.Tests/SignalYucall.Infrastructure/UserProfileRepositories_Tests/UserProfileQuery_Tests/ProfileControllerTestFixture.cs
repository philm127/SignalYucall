using Microsoft.Extensions.Configuration;
using Moq;
using SignalYucall.Application.Persistence.Queries;

namespace SignalYucall.Tests.SignalYucall.Infrastructure.UserProfileRepositories_Tests.UserProfileQuery_Tests
{
    public class ProfileControllerTestFixture
    {
        public IUserProfileQueryRepository UserProfileQueryRepository { get; set; }
        public IConfiguration Configuration { get; set; }

        public ProfileControllerTestFixture()
        {
            var mockUserProfileQueryRepository = new Mock<IUserProfileQueryRepository>();
            var mockConfiguration = new Mock<IConfiguration>();

            UserProfileQueryRepository = mockUserProfileQueryRepository.Object;
            Configuration = mockConfiguration.Object;
        }
    }
}