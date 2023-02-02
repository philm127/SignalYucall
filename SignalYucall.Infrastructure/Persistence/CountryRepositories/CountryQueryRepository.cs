using Dapper;
using Microsoft.Extensions.Configuration;
using SignalYucall.Application.CountryService.Queries;
using SignalYucall.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalYucall.Infrastructure.Persistence.CountryRepositories
{
    public class CountryQueryRepository : ICountryQueryRepository
    {
        private readonly IConfiguration _configuration;

        public CountryQueryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> GetCountryIdByCountryCode(string countryCode)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT CountryId FROM Country WHERE CountryCode = @CountryCode",
                    new { CountryCode = countryCode }
                );
            }
        }


        public async Task<Country> GetCountryByShortName(string shortName)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<Country>(
                    "SELECT * FROM Country WHERE LOWER(ShortName) = @ShortName",
                    new { ShortName = shortName.ToLower() }
                );
            }
        }

        public async Task<string> GetCountryName(int countryId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM Country WHERE Id = @countryID",
                    new { countryID = countryId }
                );
            }
        }
    }
}