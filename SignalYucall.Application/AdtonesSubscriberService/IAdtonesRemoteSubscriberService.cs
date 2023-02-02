
namespace SignalYucall.Application.AdtonesSubscriberService
{
    public interface IAdtonesRemoteSubscriberService
    {
        Task<Domain.Entities.Subscriber> GetSubscriberRegister(Domain.Entities.Subscriber subscriber, string connectionString, string shortName, string countryCode);
        Task<int> GetCountryId(string connectionString, string shortName);
    }
}
