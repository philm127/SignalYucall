using SignalYucall.Application.ProfileService.Queries;
using SignalYucall.Application.SubscriberService.Commands;

namespace SignalYucall.Application.Persistence.Queries
{
    public interface IUserProfileQueryRepository
    {
        Task<bool> GetUserProfileByMsisdn(string msisdn);
        Task<UserProfileModelDto> GetSubscriberByMaskedId(string maskedId);
        Task<IEnumerable<string>> GetSubscriberMarketingList(int Gender, string StartDob, string EndDob);
    }
}
