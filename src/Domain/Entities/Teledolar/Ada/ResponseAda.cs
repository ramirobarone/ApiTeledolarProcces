using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Teledolar.Ada
{
    public class ResponseAda
    {
        public Execution execution { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int time_to_activate_tft { get; set; }
        public int time_to_activate_dtr { get; set; }
        public Sinpe_Adas[] sinpe_adas { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string identification_type_key { get; set; }
        public string identification { get; set; }
        public int id { get; set; }
        public string entity { get; set; }
        public Current_Sinpe_Ada current_sinpe_ada { get; set; }
        public string currency { get; set; }
        public object confirmed_at_dtr { get; set; }
        public object confirmed_at { get; set; }
        public int client_account_type_id { get; set; }
        public Client_Account_Type client_account_type { get; set; }
        public int client_account_state_id { get; set; }
        public Client_Account_State client_account_state { get; set; }
        public int client_account_dtr_state_id { get; set; }
        public Client_Account_Dtr_State client_account_dtr_state { get; set; }
        public object awaiting_time_to_activate_dtr { get; set; }
        public object awaiting_time_to_activate { get; set; }
    }

    public class Current_Sinpe_Ada
    {
        public string state { get; set; }
        public string service { get; set; }
        public string name { get; set; }
        public string identification { get; set; }
        public int id { get; set; }
        public string end_date { get; set; }
        public string description { get; set; }
        public string currency { get; set; }
        public string client_account_number { get; set; }
        public string client_account_currency { get; set; }
        public string amount { get; set; }
        public int admin { get; set; }
    }

    public class Client_Account_Type
    {
        public string name { get; set; }
        public string key { get; set; }
        public int id { get; set; }
    }

    public class Client_Account_State
    {
        public string name { get; set; }
        public string key { get; set; }
        public int id { get; set; }
    }

    public class Client_Account_Dtr_State
    {
        public string name { get; set; }
        public string key { get; set; }
        public int id { get; set; }
    }

    public class Sinpe_Adas
    {
        public string state { get; set; }
        public object[] sinpe_ada_states { get; set; }
        public string service { get; set; }
        public string name { get; set; }
        public string identification { get; set; }
        public int id { get; set; }
        public string end_date { get; set; }
        public string description { get; set; }
        public string currency { get; set; }
        public string client_account_number { get; set; }
        public string client_account_currency { get; set; }
        public string amount { get; set; }
        public int admin { get; set; }
    }

}
