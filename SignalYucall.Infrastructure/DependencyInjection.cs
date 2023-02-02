using Microsoft.Extensions.DependencyInjection;
using SignalYucall.Application.AdtonesSubscriberService;
using SignalYucall.Application.CountryService.Queries;
using SignalYucall.Application.Persistence;
using SignalYucall.Application.Persistence.Commands;
using SignalYucall.Application.Persistence.Queries;
using SignalYucall.Infrastructure.Adtones;
using SignalYucall.Infrastructure.Persistence.AdvertRepositories;
using SignalYucall.Infrastructure.Persistence.CampaignAuditRepositories;
using SignalYucall.Infrastructure.Persistence.CountryRepositories;
using SignalYucall.Infrastructure.Persistence.UserProfileRepositories;
using SignalYucall.Infrastructure.Persistence.UserRepositories;

namespace SignalYucall.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserCommandRepository, UserCommandRepository>();
            services.AddScoped<IUserProfileCommandRepository, UserProfileCommandRepository>();
            services.AddScoped<IUserProfileQueryRepository, UserProfileQueryRepository>();
            services.AddScoped<ICountryQueryRepository, CountryQueryRepository>();
            services.AddScoped<IAdtonesMainSubscriberService, AdtonesMainSubscriberService>();
            services.AddScoped<ICampaignAuditQueryRepository, CampaignAuditQueryRepository>();
            services.AddScoped<IAdvertQueryRepository, AdvertQueryRepository>();
            return services;
        }
    }
}