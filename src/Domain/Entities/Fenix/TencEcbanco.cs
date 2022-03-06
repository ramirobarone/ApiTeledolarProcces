using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class TencEcbanco
    {
        public int IdEc { get; set; }
        public int? IdBanco { get; set; }
        public DateTime FecCar { get; set; }
        public string CtaBanco { get; set; }
        public decimal? SalIni { get; set; }
        public decimal? SalDis { get; set; }
    }
}
