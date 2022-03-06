using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class ResultadoDomiciliacionTeleDolarIndivdual
    {
        public int Id { get; set; }
        public string Timestamp { get; set; }
        public string Message { get; set; }
        public string ExternalMessage { get; set; }
        public int? Code { get; set; }
        public int? TransactionTypeId { get; set; }
        public int? TransactionStateId { get; set; }
        public decimal? TotalDebit { get; set; }
        public decimal? TotalCredit { get; set; }
        public decimal? Total { get; set; }
        public decimal? MainCredit { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string Number { get; set; }
        public int? BankAccountTypeId { get; set; }
        public string FullNameDesnity { get; set; }
        public string NumberDesnity { get; set; }
        public int? BankAccountTypeIdDesnity { get; set; }
        public string SinpeReference { get; set; }
        public string InsertedAt { get; set; }
        public string DescriptionDomi { get; set; }
        public int? ChannelId { get; set; }
        public string Identificacion { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public decimal? MontoDomiciliacion { get; set; }
        public int IdCredito { get; set; }
        public string Referencia { get; set; }
        public bool? Estado { get; set; }
        public string DeviceId { get; set; }
        public string Cuotas { get; set; }
    }
}
