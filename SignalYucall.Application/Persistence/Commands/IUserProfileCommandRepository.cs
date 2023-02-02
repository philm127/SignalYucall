using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Domain.Entities;

namespace SignalYucall.Application.Persistence.Commands
{
    public interface IUserProfileCommandRepository
    {
        Task Add(UserProfileModelDto subscriber);
        Task Update(UserProfileModelDto subscriber);
    }
}
