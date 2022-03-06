using System.Collections.Generic;

namespace Domain.Entities.Teledolar.Ada
{
    public class Execution
    {
        public int timestamp { get; set; }
        public List<string> message { get; set; }
        public List<string> external_message { get; set; }
        public int code { get; set; }
    }
}
