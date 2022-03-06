namespace Domain.Entities.Teledolar.Account
{
    public class ResponseAccountClient
    {
        public Execution execution { get; set; }
        public Data data { get; set; }
    }
    public class Execution
    {
        public int timestamp { get; set; }
        public string[] message { get; set; }
        public string[] external_message { get; set; }
        public int code { get; set; }
    }

    public class Data
    {
        public Bccr_Client_Account bccr_client_account { get; set; }
    }

    public class Bccr_Client_Account
    {
        public string number { get; set; }
        public string name { get; set; }
        public string identification { get; set; }
        public string entity { get; set; }
        public string currency_code { get; set; }
        public int identification_type_id { get; set; }
        public Identification_Type identification_type { get; set; }
    }

    public class Identification_Type
    {
        public string regexp_description { get; set; }
        public string regexp { get; set; }
        public string pattern { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public int id { get; set; }
        public int client_id { get; set; }
        public bool available_for_sign_up { get; set; }
        public bool available_for_individual { get; set; }
        public bool available_for_enterprise { get; set; }
    }
}
