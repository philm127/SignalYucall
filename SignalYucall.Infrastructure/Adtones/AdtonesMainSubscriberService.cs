
using SignalYucall.Application.AdtonesSubscriberService;
using SignalYucall.Domain.Entities;
using System.Threading.Tasks;

namespace SignalYucall.Infrastructure.Adtones
{
    public class AdtonesMainSubscriberService : IAdtonesMainSubscriberService
    {

        public AdtonesMainSubscriberService()
        {

        }

        public async Task<Subscriber> GetNewSubscriberModel(Subscriber subscriber, string connectionString, string shortName, string countryCode)
        {
            return await Task.FromResult(subscriber);
        }
        
    }
}
