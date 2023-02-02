using Dapper;
using Microsoft.Extensions.Configuration;
using SignalYucall.Application.Persistence.Queries;
using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Domain.Entities;
using System.Data.SqlClient;

namespace SignalYucall.Infrastructure.Persistence.UserProfileRepositories
{
    public class UserProfileQueryRepository : IUserProfileQueryRepository
    {
        private readonly IConfiguration _configuration;

        public UserProfileQueryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<bool> GetUserProfileByMsisdn(string msisdn)
        {
            var sql = @"SELECT UserId, MSISDN, CountryId, Gender, DOB FROM UserProfile WHERE MSISDN = @MSISDN";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var user = await connection.QueryFirstOrDefaultAsync<Subscriber>(sql, new { MSISDN = msisdn });

                if (user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public async Task<UserProfileModelDto> GetSubscriberByMaskedId(string maskedId)
        {
            var sql = @"SELECT UserId, MSISDN, CountryId, Gender, DOB FROM UserProfile WHERE MaskedId = @MaskedId";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<UserProfileModelDto>(sql, new { MaskedId = maskedId });
                
            }
        }

        public async Task<IEnumerable<string>> GetSubscriberMarketingList(int Gender, string StartDob, string EndDob)
        {
            var sql = @"SELECT MaskedId FROM UserProfile WHERE Gender=@gender AND DOB BETEEN @DobStart AND @DobEnd";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                return await connection.QueryAsync<string>(sql, new { gender = Gender, DobStart = StartDob, DobEnd = EndDob });

            }
        }


    }
}