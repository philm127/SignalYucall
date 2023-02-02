using MediatR;
using Microsoft.Extensions.Logging;
using SignalYucall.Application.ProfileService.Queries;
using SignalYucall.Application.Persistence.Queries;

namespace SignalYucall.Application.ProfileService.Handlers
{
    public class SubscriberProfilesQueryHandler : IRequestHandler<SubscriberProfilesQueryCommand, SubscriberProfilesQueryResult>
    {
        private readonly IUserProfileQueryRepository _userProfileRepository;
        private readonly ILogger<SubscriberProfilesQueryHandler> _logger;

        public SubscriberProfilesQueryHandler(IUserProfileQueryRepository userProfileRepository, ILogger<SubscriberProfilesQueryHandler> logger)
        {
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<SubscriberProfilesQueryResult> Handle(SubscriberProfilesQueryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userProfiles = await _userProfileRepository.GetSubscriberMarketingList(request.Gender, request.StartDob, request.EndDob);
                var userList = userProfiles.Select(up => new SubscriberProfilesString { UserID = up }).ToList();
                return new SubscriberProfilesQueryResult { Status = 0, CallID = request.CallID, UserList = userList };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user profiles by gender and dob");
                throw;
            }
        }
    }
}
