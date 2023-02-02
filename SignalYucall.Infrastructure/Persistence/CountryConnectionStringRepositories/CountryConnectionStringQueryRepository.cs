using Dapper;
using Microsoft.Extensions.Configuration;
using SignalYucall.Application.Persistence;
using SignalYucall.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalYucall.Infrastructure.Persistence.CountryConnectionStringRepositories
{
    public class CountryConnectionStringQueryRepository : ICountryConnectionStringRepository
    {
        private readonly IConfiguration _configuration;

        public CountryConnectionStringQueryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> GetConnectionStringAsync(int countryId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT Id FROM CountryConnectionString WHERE CountryId = @CountryId",
                    new { CountryId = countryId }
                );
            }
        }
    }
}