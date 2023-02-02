using System.Reflection;

namespace SignalYucall.Application.SubscriberService.Commands
{
    public class UserModelDto
    {
        public int UserId { get; set; } = 0;
        public int OperatorId { get; set; } = 0;
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public int RoleId { get; set; } = 2;

    }
}
