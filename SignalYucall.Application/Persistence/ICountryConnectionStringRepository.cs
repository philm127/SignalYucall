using SignalYucall.Domain.Entities;

namespace SignalYucall.Application.Persistence;
public interface ICountryConnectionStringRepository
{
    Task<int> GetConnectionStringAsync(int countryId);
}
