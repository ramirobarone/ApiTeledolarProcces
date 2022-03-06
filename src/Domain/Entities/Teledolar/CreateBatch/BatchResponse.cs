using System;
using System.Collections.Generic;

namespace Domain.Entities.Teledolar.CreateBatch
{

    public class Execution
    {
        public int timestamp { get; set; }
        public List<string> message { get; set; }
        public List<string> external_message { get; set; }
        public int code { get; set; }
    }

    public class Data
    {
        public string state { get; set; }
        public DateTime inserted_at { get; set; }
        public int batch_id { get; set; }
    }

    public class Result
    {
        public Execution execution { get; set; }
        public Data data { get; set; }
    }

    public class BatchResponse
    {
        public Result Result { get; set; }

    }
}
