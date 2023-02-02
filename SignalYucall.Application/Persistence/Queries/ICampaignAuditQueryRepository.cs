using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalYucall.Application.Persistence.Queries
{
    public interface ICampaignAuditQueryRepository
    {
        Task<IEnumerable<CampaignAudit>> GetCampaignAuditsByUserProfileId(int userProfileId, string startDate, string endDate);
    }
}
