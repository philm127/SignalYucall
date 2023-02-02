namespace SignalYucall.Application.Persistence
{
    public interface IAdtoneSubscriber
    {
         string GetMaskedId(string MSISDN);
    }
}