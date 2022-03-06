using Domain.Entities.CommodEntities;

namespace Domain.Entities.Teledolar.SingleDomiciliation
{
    public class ExecuteTransaction
    {
        public Source source { get; set; }
        public Target target { get; set; }
        public string reference_number { get; set; }
        public string description { get; set; }
        public int transaction_type_id { get; set; }
        public int currency_id { get; set; }
        public string amount { get; set; }
        public int channel_id { get; set; }
        public string device_id { get; set; }
        public string timestamp { get; set; }
        
        public class Source
        {
            public string client { get; set; }
            public int enterprise_service_id { get; set; }
            public string ada_service { get; set; }
        }

        public class Target
        {
            public string client { get; set; }
        }
       

    }
}
