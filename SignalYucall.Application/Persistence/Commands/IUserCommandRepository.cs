using SignalYucall.Application.SubscriberService.Commands;
using SignalYucall.Domain.Entities;

namespace SignalYucall.Application.Persistence.Commands;

public interface IUserCommandRepository
{
    Task<int> Add(UserModelDto subscriber);
    Task Update(Subscriber subscriber);
}