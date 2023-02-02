using MediatR;

namespace SignalYucall.Application.SubscriberService.Commands;

public class RegisterSubscriberCommand : IRequest<RegisterResponse>
{
    public string MSISDN { get; set; } = "";
    public int Yucall { get; set; }
    public string Dob { get; set; } = "2000-01-01";
    public string Country { get; set; } = "GB";
    public int Gender { get; set; }
    public string CallID { get; set; } = "0";
}

public class RegisterResponse
{
    public int Status { get; set; }
    public string CallID { get; set; } = "1";
    public string UserID { get; set; } = "0";
    public int Network { get; set; } = 0;
}