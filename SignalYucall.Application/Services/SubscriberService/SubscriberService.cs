using Microsoft.Extensions.Configuration;
using SignalYucall.Application.Services.AdtonesSubscriber;
using SignalYucall.Application.SubscriberService;
using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Application.SubscriberService.Queries;
using SignalYucall.Application.SubscriberServiceService;
using SignalYucall.Domain.Common.Helpers;
using System.Diagnostics.Metrics;

namespace SignalYucall.Application.Services.Subscriber;

public class SubscriberService : IRegisterSubscriberCommandHandler
{

    private readonly IUserCommandRepository _subscriberRepository;
    private readonly IAdtonesRemoteSubscriberService _adtonesRemoteSubscriberService;
    private readonly IConfiguration _configuration;

    public SubscriberService(IUserCommandRepository subscriberRepository, IAdtonesRemoteSubscriberService adtonesRemoteSubscriberService, IConfiguration configuration)
    {
        _subscriberRepository = subscriberRepository;
        _adtonesRemoteSubscriberService = adtonesRemoteSubscriberService;
        _configuration = configuration;
    }


    public RegisterResult Register(string msisdn, int Yucall, string Dob, string Country, int Gender)
    {
        if (string.IsNullOrEmpty(msisdn) || msisdn.Length < 8 || string.IsNullOrEmpty(Country)) //|| string.IsNullOrEmpty(Dob) 
            return new RegisterResult();
        
        var cleanmsisdn = MsisdnHelper.GetCleanMsisdn(msisdn);
        var checkSub = _subscriberRepository.GetSubscriberByMsisdn(cleanmsisdn);

        if (checkSub != null)
            return new RegisterResult();

        var countryCode = MsisdnHelper.GetCountryCode(msisdn);
        var connectionString = _configuration.GetConnectionString("AdtonesMainConnectionString");

        var subscriber = new Domain.Entities.Subscriber();
        subscriber.CreatedDate = DateTime.Now;
        subscriber.MSISDN = cleanmsisdn;
        subscriber.ProviderId = Yucall;
        subscriber.DOB = Convert.ToDateTime(Dob);

        
        if (connectionString != null)
            subscriber = _adtonesRemoteSubscriberService.GetSubscriberRegister(subscriber, connectionString, Country, countryCode).Result;

        _subscriberRepository.Add(subscriber);

        var result = new RegisterResult() { Status = 1, UserID = subscriber.UserId.ToString(), Network = subscriber.OperatorId };

        return result;
        
    }


    public UpdateResult Update(string userId, string msisdn, string dob, string country, int gender)
    {
        var cleanmsisdn = MsisdnHelper.GetCleanMsisdn(msisdn);

        Domain.Entities.Subscriber subscriber = _subscriberRepository.GetSubscriberByMsisdn(cleanmsisdn);
        if (subscriber == null)
        {
            return new UpdateResult() { Status = 0 };
        }

        subscriber.Gender = gender;
        subscriber.DOB = Convert.ToDateTime(dob);
        subscriber.MSISDN = cleanmsisdn;

        var connectionString = _configuration.GetConnectionString("AdtonesMainConnectionString");
        if (connectionString != null)
            subscriber.CountryId = _adtonesRemoteSubscriberService.GetCountryId(connectionString, country).Result;


        _subscriberRepository.Update(subscriber);

        return new UpdateResult() { Status = 1 };
    }
}
