using MediatR;

namespace SignalYucall.Application.SubscriberService.Commands
{
    public class UpdateSubscriberCommand : IRequest<UpdateResponse>
    {
        public string MSISDN { get; set; }
        public string Country { get; set; }
        public int Gender { get; set; }
        public string Dob { get; set; }
        public string CallID { get; set; }
    }

    public class UpdateResponse
    {
        public int Status { get; set; }
        public string UserID { get; set; } = "0";
        public int Network { get; set; } = 0;
        public string CallID { get; set; }
    }
}
