using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class TdetEcbanco
    {
        public int IdEc { get; set; }
        public DateTime FecTran { get; set; }
        public string DocRef { get; set; }
        public string TipTran { get; set; }
        public string DescripTran { get; set; }
        public decimal? DebTran { get; set; }
        public decimal? CredTran { get; set; }
        public int? IdCobro { get; set; }
        public int? IdTipDepBan { get; set; }
        public DateTime? FecNac { get; set; }
        public bool? Domiciliacion { get; set; }
        public bool? Estado { get; set; }
        public int Id { get; set; }
    }
}
