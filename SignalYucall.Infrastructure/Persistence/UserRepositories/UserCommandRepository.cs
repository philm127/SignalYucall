using Dapper;
using Microsoft.Extensions.Configuration;
using SignalYucall.Application.Persistence.Commands;
using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalYucall.Infrastructure.Persistence.UserRepositories
{
    public class UserCommandRepository : IUserCommandRepository
    {
        private readonly IConfiguration _configuration;

        public UserCommandRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<int> Add(UserModelDto subscriber)
        {

            var sql = @"INSERT INTO [dbo].[Users] (CreatedDate, UpdatedDate, Active, OperatorId, RoleId) 
                            Values(GetDate(), @DateUpdated, @Active, @OperatorId, @RoleId);
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    return await connection.ExecuteScalarAsync<int>(sql, new { DateCreated = DateTime.Now, DateUpdated = subscriber.UpdatedDate, Active = 1, subscriber.OperatorId, subscriber.RoleId });

                }
        }


        public async Task Update(Subscriber subscriber)
        {
            var sql = @"UPDATE [dbo].[Subscribers] set MSISDN=@MSISDN, UpdatedDate=GETDATE(), Active=1, OperatorId=@OperatorId, Gender=@Gender, DOB=@DOB WHERE UserID=@UserId;";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                await connection.ExecuteScalarAsync<int>(sql, new { subscriber.MSISDN, subscriber.OperatorId, subscriber.Gender, subscriber.DOB, subscriber.UserId });

            }
        }
    }
}