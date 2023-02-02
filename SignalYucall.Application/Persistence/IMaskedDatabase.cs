namespace SignalYucall.Application.Persistence
{
    public interface IMaskedDatabase
    {
         string GetMaskedId(string MSISDN);
    }
}