using System;

namespace Domain.Entities.Teledolar.SingleDomiciliation
{
    public class ResultDomiciliation
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
        public Transaction transaction { get; set; }
        public Entities entities { get; set; }
    }

    public class Transaction
    {
        public int transaction_type_id { get; set; }
        public int transaction_state_id { get; set; }
        public int id { get; set; }
        public DateTime inserted_at { get; set; }
        public string sinpe_reference { get; set; }
        public Summary summary { get; set; }
        public string exchange_rate { get; set; }
        public string description { get; set; }
        public int channel_id { get; set; }
    }

    public class Summary
    {
        public Target target { get; set; }
        public Source source { get; set; }
    }

    public class Target
    {
        public string total_debit { get; set; }
        public string total_credit { get; set; }
        public string total { get; set; }
        public string main_credit { get; set; }
        public string full_name { get; set; }
        public Extra_Debits[] extra_debits { get; set; }
        public string[] extra_credits { get; set; }
        public string entity { get; set; }
        public Bank_Account bank_account { get; set; }
    }

    public class Bank_Account
    {
        public string number { get; set; }
        public object description { get; set; }
        public int currency_id { get; set; }
        public int bank_account_type_id { get; set; }
    }
    public class Source
    {
        public string total_debit { get; set; }
        public string total_credit { get; set; }
        public string total { get; set; }
        public string main_debit { get; set; }
        public string full_name { get; set; }
        public Extra_Debits[]? extra_debits { get; set; }
        public object[] extra_credits { get; set; }
        public string entity { get; set; }
        public Bank_Account1 bank_account { get; set; }
    }
    public class Bank_Account1
    {
        public string number { get; set; }
        public string description { get; set; }
        public int currency_id { get; set; }
        public int bank_account_type_id { get; set; }
    }
    public class Extra_Debits
    {
        public string name { get; set; }
        public string key { get; set; }
        public object description { get; set; }
        public string amount { get; set; }
    }
    public class Entities
    {
        public Target1 target { get; set; }
        public Source1 source { get; set; }
    }
    public class Target1
    {
        public string name { get; set; }
    }
    public class Source1
    {
        public string name { get; set; }
    }
}
