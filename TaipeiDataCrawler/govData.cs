using System.Collections.Generic;

namespace TaipeiDataCrawler
{
    public class govData
    {
        public string categoryCode { set; get; }
        public string identifier { set; get; }
        public string tite { set; get; }
        public string description { set; get; }
        public string fieldDescription { set; get; }
        public string publisher { set; get; }
        public string accrualPeriodicity { set; get; }
        public string modified { set; get; }
        public List<resourceData> distrbution { set; get; }

    }

    public class resourceData
    {
        public string resourceID { set; get; }
        public string resourceDescription { set; get; }
        public string format { set; get; }
        public string resourceModified { set; get; }
        public string downloadURL { set; get; }
        public string metadataSourceOfData { set; get; }
        public string characterSetCode { set; get; }

    }
}