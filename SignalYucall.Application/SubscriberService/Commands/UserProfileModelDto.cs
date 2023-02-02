namespace SignalYucall.Application.SubscriberService.Commands
{
    public class UserProfileModelDto
    {
        public int UserProfileId { get; set; }
        public int UserId { get; set; }
        public string DOB { get; set; }
        public int Gender { get; set; }
        public string MSISDN { get; set; }
        public int CountryId { get; set; }
        public int OperatorId { get; set; } = 0;
        public int ProviderId { get; set; } = 1;
        public string MaskedId { get; set; } = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();


        public UserProfileModelDto(RegisterSubscriberCommand command)
        {
            MSISDN = command.MSISDN;
            DOB = command.Dob;
            Gender = command.Gender;
            ProviderId = command.Yucall;
        }
    }

}