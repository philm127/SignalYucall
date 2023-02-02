

namespace SignalYucall.Application.AdvertService.Queries
{
    public class AdvertHistoryListQuery
    {
        public string UserID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CallID { get; set; }
    }

    
    public class AdvertHistoryListQueryResponse
    {
        public int Status { get; set; }
        public string CallID { get; set; }
        public List<AdvertHistoryList> AdList { get; set; }
    }


    public class AdvertHistoryList
    {
        public string AdPlayID { get; set; }
        public string Brand { get; set; }
        public string BrandImg { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }

        public string StartTime { get; set; }
    }

}
