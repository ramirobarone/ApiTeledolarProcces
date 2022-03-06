using System.Collections.Generic;

namespace Domain.Entities.Teledolar.Ada
{
    public class RequestAda
    {
        public string identification { get; set; }
        public int identification_type_id { get; set; }
        public string number { get; set; }
        public string device_id { get; set; }
        public string timestamp { get; set; }
        public int client_account_type_id { get; set; }
        public List<string> available_operations { get; set; }
        public Ada ada { get; set; }
    }
}
