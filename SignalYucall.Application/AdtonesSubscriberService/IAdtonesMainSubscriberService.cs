using SignalYucall.Domain.Entities;

namespace SignalYucall.Application.AdtonesSubscriberService
{
    public interface IAdtonesMainSubscriberService
    {
        Task<Subscriber> GetNewSubscriberModel(Subscriber subscriber, string connectionString, string shortName, string countryCode);
    }
}