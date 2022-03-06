using System;

namespace Domain.Entities.Fenix
{
    public class CobrosDePrima
    {
        public int id { get; set; }
        public int idSolicitud { get; set; }
        public DateTime fechaCobro { get; set; }
        public decimal montoPrima { get; set; }
        public string identificacionCliente { get; set; }
        public string timestampTeledolar { get; set; }
        public int transactionId { get; set; }
        public decimal mainCredit { get; set; }
        public string Sinpe_reference { get; set; }
        public string DescriptionTeledolar { get; set; }
        public DateTime inserted_at { get; set; }
        public int idTeledolar { get; set; }
        public string message { get; set; }
        public string entity { get; set; }
        public string bank_account { get; set; }
    }
}
