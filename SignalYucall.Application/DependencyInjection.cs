using Microsoft.Extensions.DependencyInjection;
using SignalYucall.Application.Helpers;
using SignalYucall.Application.SubscriberService.Handlers;

namespace SignalYucall.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISubscriberHelper, SubscriberHelper>();
            services.AddScoped<ICountryHelper, CountryHelper>();
            return services;
        }
    }
}