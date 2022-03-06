using System.Collections.Generic;

namespace Domain.Entities.Teledolar.CreateBatch
{
    public class Source
    {
        public int enterprise_service_id { get; set; }
        public string client { get; set; }
        public string ada_service { get; set; }
    }

    public class Target
    {
        public string client { get; set; }
    }

    public class Transaction
    {
        public Source source { get; set; }
        public Target target { get; set; }
        public string reference_number { get; set; }
        public string description { get; set; }
        public int transaction_type_id { get; set; }
        public int currency_id { get; set; }
        public string amount { get; set; }
    }

    public class BatchRequest
    {
        public int channel_id { get; set; }
        public string device_id { get; set; }
        public string timestamp { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}
