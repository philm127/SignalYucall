using SignalYucall.Application.SubscriberService.Commands;

namespace SignalYucall.Application.SubscriberService.Handlers
{
    public interface IRegisterSubscriberCommandHandler
    {
        Task<RegisterResponse> Handle(RegisterSubscriberCommand request);
    }
}