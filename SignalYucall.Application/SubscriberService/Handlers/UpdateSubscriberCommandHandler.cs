using MediatR;
using Microsoft.Extensions.Logging;
using SignalYucall.Application.Helpers;
using SignalYucall.Application.Persistence.Commands;
using SignalYucall.Application.SubscriberService.Commands;


namespace SignalYucall.Application.SubscriberService.Handlers
{
    public class UpdateSubscriberCommandHandler :  IRequestHandler<UpdateSubscriberCommand, UpdateResponse>
    {
        private readonly IUserProfileCommandRepository _userProfileCommand;
        private readonly ICountryHelper _countryHelper;
        private readonly ISubscriberHelper _subscriberHelper;
        private readonly ILogger<UpdateSubscriberCommandHandler> _logger;

        public UpdateSubscriberCommandHandler(ISubscriberHelper subscriberHelper
                                                , IUserProfileCommandRepository userProfileCommand, ICountryHelper countryHelper,
                                                ILogger<UpdateSubscriberCommandHandler> logger)
        {
            _userProfileCommand = userProfileCommand;
            _countryHelper = countryHelper;
            _subscriberHelper = subscriberHelper;
            _logger = logger;
        }

    public async Task<UpdateResponse> Handle(UpdateSubscriberCommand request, CancellationToken cancellationToken)
    {
            // Log start of processing the request
            _logger.LogInformation("Start processing UpdateSubscriberCommand, CallID: {callID}", request.CallID);

            // Check if country name is valid
            var isCountryValid = await _countryHelper.IsCountryShortNameValid(request.Country);
            if (!isCountryValid)
            {
                _logger.LogError("Invalid country name: {countryName}", request.Country);
                return await Task.FromResult(GetErrorResponse(request.CallID, request.MSISDN));
            }

            // Check if subscriber is already registered
            var userProfile = await _subscriberHelper.GetSubscriberByMaskedId(request.MSISDN);
            if (userProfile == null)
            {
                _logger.LogError("Subscriber is not registered: {msisdn}", request.MSISDN);
                return await Task.FromResult(GetErrorResponse(request.CallID, request.MSISDN));
            }

            // Update user profile
            await _userProfileCommand.Update(userProfile);

            _logger.LogInformation("End processing UpdateSubscriberCommand, CallID: {callID}, UserID: {userID}", request.CallID, userProfile.MaskedId);

            return await Task.FromResult(new UpdateResponse { Status = 0, CallID = request.CallID, Network = userProfile.OperatorId, UserID = userProfile.MaskedId });
        }

        private UpdateResponse GetErrorResponse(string callID, string userId)
        {
            return new UpdateResponse { Status = -1, CallID = callID, Network = 0, UserID = userId };
        }
    }
}