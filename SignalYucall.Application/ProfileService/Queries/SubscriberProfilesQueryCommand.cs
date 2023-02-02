using MediatR;

namespace SignalYucall.Application.ProfileService.Queries
{
    public class SubscriberProfilesQueryCommand : IRequest<SubscriberProfilesQueryResult>
    {
        public int Gender { get; set; }
        public string StartDob { get; set; }
        public string EndDob { get; set; }
        public string CallID { get; set; }
    }

    public class SubscriberProfilesQueryResult
    {
        public int Status { get; set; }
        public string CallID { get; set; }
        public List<SubscriberProfilesString> UserList { get; set; }
    }

    public class SubscriberProfilesString
    {
        public string UserID { get; set; }
    }


}
