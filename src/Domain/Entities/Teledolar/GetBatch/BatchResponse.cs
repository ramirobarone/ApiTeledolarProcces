using System;
using System.Collections.Generic;

namespace Domain.Entities.Teledolar.GetBatch
{
    public class Execution
    {
        public int timestamp { get; set; }
        public List<string> message { get; set; }
        public List<string> external_message { get; set; }
        public int code { get; set; }
    }

    public class Target
    {
        public string client { get; set; }
    }

    public class Source
    {
        public int enterprise_service_id { get; set; }
        public string client { get; set; }
        public string ada_service { get; set; }
        public string enterprise_service_reference { get; set; }
    }

    public class Params
    {
        public int trigger_account_id { get; set; }
        public int transaction_type_id { get; set; }
        public string timestamp { get; set; }
        public Target target { get; set; }
        public Source source { get; set; }
        public string reference_number { get; set; }
        public string device_id { get; set; }
        public string description { get; set; }
        public int currency_id { get; set; }
        public int channel_id { get; set; }
        public string amount { get; set; }
    }

    public class Entry
    {
        public DateTime updated_at { get; set; }
        public int? transaction_id { get; set; }
        public Transaction transaction { get; set; }
        public string state { get; set; }
        public ResponseDOMI response { get; set; }
        public Params @params { get; set; }
        public DateTime inserted_at { get; set; }
        public int id { get; set; }
        public int batch_id { get; set; }
    }

    public class Batch
    {
        public string state { get; set; }
        public int id { get; set; }
    }

    public class Data
    {
        public int total_pages { get; set; }
        public int total_entries { get; set; }
        public int page_size { get; set; }
        public int page_number { get; set; }
        public List<Entry> entries { get; set; }
        public Batch batch { get; set; }
    }

    public class Result
    {
        public Execution execution { get; set; }
        public Data data { get; set; }
    }

    public class Response_Domiciliacion
    {
        public Result Result { get; set; }

    }

    public class Summary
    {
        public Target2 target { get; set; }
        public Source2 source { get; set; }
    }

    public class Transaction
    {
        public int transaction_type_id { get; set; }
        public int transaction_state_id { get; set; }
        public Summary summary { get; set; }
        public string sinpe_reference { get; set; }
        public DateTime inserted_at { get; set; }
        public int id { get; set; }
        public object fee_id { get; set; }
        public string exchange_rate { get; set; }
        public string description { get; set; }
        public int channel_id { get; set; }
    }

    public class Target2
    {
        public string total_debit { get; set; }
        public string total_credit { get; set; }
        public string total { get; set; }
        public string main_credit { get; set; }
        public string full_name { get; set; }
        public List<ExtraDebit> extra_debits { get; set; }
        public List<object> extra_credits { get; set; }
        public string entity { get; set; }
        public BankAccount bank_account { get; set; }
    }
    public class ExtraDebit
    {
        public string name { get; set; }
        public string key { get; set; }
        public object description { get; set; }
        public string amount { get; set; }
    }

    public class BankAccount
    {
        public string number { get; set; }
        public string description { get; set; }
        public int currency_id { get; set; }
        public int bank_account_type_id { get; set; }
    }

    public class Source2
    {
        public object total_debit { get; set; }
        public object total_credit { get; set; }
        public object total { get; set; }
        public string main_debit { get; set; }
        public string full_name { get; set; }
        public object extra_debits { get; set; }
        public object extra_credits { get; set; }
        public string entity { get; set; }
        public BankAccount2 bank_account { get; set; }
    }
    public class BankAccount2
    {
        public string number { get; set; }
        public object description { get; set; }
        public int currency_id { get; set; }
        public int bank_account_type_id { get; set; }
    }

    public class ResponseDOMI
    {
        public string key { get; set; }
        public string internal_message { get; set; }
        public object external_message { get; set; }
        public int code { get; set; }
        public int? timestamp { get; set; }
        public List<string> message { get; set; }
    }

    public class DatosDomiciliacionTeledolar
    {
        public int? transaction_type_id { set; get; }
        public int? transaction_state_id { set; get; }
        public string total_debit { set; get; }
        public string total_credit { set; get; }
        public string total { set; get; }
        public string main_credit { set; get; }
        public string full_name { set; get; }
        public string description { set; get; }
        public int? batch_id { set; get; }
        public string state { set; get; }
        public string clientCC { set; get; }
        public string origenCC { set; get; }
        public string ada_service { set; get; }

        public string sinpe_reference { set; get; }
        public DateTime? inserted_at { set; get; }
        public string description_domi { set; get; }
        public int? channel_id { set; get; }

        public int? code { set; get; }
        public int? timestamp { set; get; }
    }
}
