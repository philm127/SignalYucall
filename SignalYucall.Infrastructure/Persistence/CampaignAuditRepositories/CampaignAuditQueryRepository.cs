
using SignalYucall.Application.Persistence.Queries;

namespace SignalYucall.Infrastructure.Persistence.CampaignAuditRepositories
{
    public class CampaignAuditQueryRepository : ICampaignAuditQueryRepository
    {
        private readonly SignalYucallContext _context;

        public CampaignAuditQueryRepository(SignalYucallContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CampaignAudit>> GetCampaignAuditsByUserProfileId(int userProfileId, string startDate, string endDate)
        {
            var start = Convert.ToDateTime(startDate);
            var end = Convert.ToDateTime(endDate);

            return await _context.CampaignAudit
                .Where(ca => ca.UserProfileId == userProfileId && ca.AddedDate >= start && ca.AddedDate <= end)
                .ToListAsync();
        }
    }
}
