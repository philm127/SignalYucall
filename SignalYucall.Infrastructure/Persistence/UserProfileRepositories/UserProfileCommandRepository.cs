using Dapper;
using Microsoft.Extensions.Configuration;
using SignalYucall.Application.Persistence.Commands;
using SignalYucall.Application.SubscriberService.Commands;
using System.Data.SqlClient;

namespace SignalYucall.Infrastructure.Persistence.UserProfileRepositories
{
    public class UserProfileCommandRepository : IUserProfileCommandRepository
    {
        private readonly IConfiguration _configuration;

        public UserProfileCommandRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task Add(UserProfileModelDto subscriber)
        {

            var sql = @"INSERT INTO [dbo].[UserProfile] (UserId, MSISDN, CountryId, Gender, DOB, ProviderId, MaskedId) 
                            Values(@UserId, @MSISDN, @CountryId, @Gender, @DOB, @ProviderId, @MaskedId);";
            if (subscriber.UserId != 0)
            {

                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    subscriber.UserId = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        UserId = subscriber.UserId,
                        MSISDN = subscriber.MSISDN,
                        CountryId = subscriber.CountryId,
                        Gender = subscriber.Gender,
                        DOB = subscriber.DOB,
                        ProviderId = subscriber.ProviderId,
                        MaskedId = subscriber.MaskedId
                    });

                }
            }
        }

        public async Task Update(UserProfileModelDto subscriber)
        {
            var sql = @"UPDATE [dbo].[UserProfile] set MSISDN=@MSISDN, UpdatedDate=GETDATE(), Active=1, OperatorId=@OperatorId, Gender=@Gender, DOB=@DOB WHERE UserID=@UserId;";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                await connection.ExecuteScalarAsync<int>(sql, new { subscriber.MSISDN, subscriber.Gender, subscriber.DOB, subscriber.UserId });

            }
        }
    }
}