
using Microsoft.Extensions.Configuration;
using SignalYucall.Application.CountryService.Queries;
using SignalYucall.Application.AdtonesSubscriberService;
using MediatR;
using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Application.Helpers;
using Microsoft.Extensions.Logging;
using SignalYucall.Application.Persistence.Commands;

namespace SignalYucall.Application.SubscriberService.Handlers
{
    public class RegisterSubscriberCommandHandler : IRequestHandler<RegisterSubscriberCommand, RegisterResponse>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly ICountryQueryRepository _countryRepository;
        private readonly IAdtonesRemoteSubscriberService _adtonesRemoteSubscriberService;
        private readonly IUserProfileCommandRepository _userProfileCommand;
        private readonly ICountryHelper _countryHelper;
        private readonly ISubscriberHelper _subscriberHelper;
        private readonly ILogger<RegisterSubscriberCommandHandler> _logger;

        public RegisterSubscriberCommandHandler(IUserCommandRepository userRepository, IAdtonesRemoteSubscriberService adtonesRemoteSubscriberService,
                                                ICountryQueryRepository countryRepository, ISubscriberHelper subscriberHelper
                                                , IUserProfileCommandRepository userProfileCommand, ICountryHelper countryHelper,
                                                ILogger<RegisterSubscriberCommandHandler> logger)
        {
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            _adtonesRemoteSubscriberService = adtonesRemoteSubscriberService;
            _userProfileCommand = userProfileCommand;
            _countryHelper = countryHelper;
            _subscriberHelper = subscriberHelper;
            _logger = logger;
        }

        public async Task<RegisterResponse> Handle(RegisterSubscriberCommand request, CancellationToken cancellationToken)
        {
            // Log start of processing the request
            _logger.LogInformation("Start processing RegisterSubscriberCommand, CallID: {callID}", request.CallID);
            
            request.MSISDN = MsisdnHelper.GetCleanMsisdn(request.MSISDN);
            
            if (MsisdnHelper.IsInvalidMsisdn(request.MSISDN) || await _countryHelper.IsCountryShortNameValid(request.Country)
                || await _subscriberHelper.IsSubscriberAlreadyRegistered(request.MSISDN))
            {
                _logger.LogError("MSISDN is invalid or country name is invalid or subscriber is already registered");
                return await Task.FromResult(GetErrorResponse(request.CallID));
            }

            var country = await _countryRepository.GetCountryByShortName(request.Country);

            var userProfile = new UserProfileModelDto(request);
            var user = new UserModelDto();

            if (await _countryRepository.GetCountryName(country.Id) == "Kenya")
            {
                // Call external API to get masked ID
                _logger.LogInformation("Getting masked ID from external API, MSISDN: {msisdn}", request.MSISDN);
                var maskedID = "xxxxxxx"; //await _adtonesRemoteSubscriberService.GetSubscriberRegister(request.MSISDN);
                if (maskedID != "0")
                {
                    userProfile.MaskedId = maskedID;
                    userProfile.OperatorId = 2;
                    user.OperatorId = 2;
                }
            }

            userProfile.CountryId = country.Id;
            userProfile.UserId = await _userRepository.Add(user);

            await _userProfileCommand.Add(userProfile);

            _logger.LogInformation("End processing RegisterSubscriberCommand, CallID: {callID}, UserID: {userID}", request.CallID, userProfile.MaskedId);

            return await Task.FromResult(new RegisterResponse { Status = 0, CallID = request.CallID, Network = userProfile.OperatorId, UserID = userProfile.MaskedId });
        }

        private RegisterResponse GetErrorResponse(string callID)
        {
            return new RegisterResponse { Status = -1, CallID = callID, Network = 0, UserID = null };
        }
    }
}