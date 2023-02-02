namespace SignalYucall.Domain.Entities;

public class Subscriber
{
    public int UserId { get; set; } = 0;
    public int OperatorId { get; set; } = 0;
    public bool Active { get; set; } = true;

    public int UserProfileId { get; set; }
    public DateTime DOB { get; set; }
    public int Gender { get; set; }
    public string MSISDN { get; set; } = "0";
    public int CountryId { get; set; }
    public int ProviderId { get; set; }
    public int AdtoneServerUserProfileId { get; set; } = 0;

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    
}
