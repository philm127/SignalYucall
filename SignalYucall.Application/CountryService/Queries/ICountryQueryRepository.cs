using System;
using SignalYucall.Domain.Entities;

namespace SignalYucall.Application.CountryService.Queries;

public interface ICountryQueryRepository
{
    Task<int> GetCountryIdByCountryCode(string countryCode);
    Task<Country> GetCountryByShortName(string shortName);
    Task<string> GetCountryName(int countryId);
}
