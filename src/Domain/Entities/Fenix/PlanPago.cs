using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class PlanPago
    {
        public int Id { get; set; }
        public int IdCredito { get; set; }
        public int Cuota { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal? MontoCapital { get; set; }
        public decimal? MontoInteres { get; set; }
        public decimal? MontoOriginacion { get; set; }
        public decimal? MontoIntMora { get; set; }
        public decimal? MontoCobrado { get; set; }
        public string Usuario { get; set; }
        public decimal? CapitalOriginal { get; set; }
        public decimal? InteresOriginal { get; set; }
        public decimal? OriginacionOriginal { get; set; }
        public decimal? InteresMoraOriginal { get; set; }
        public decimal? CapitalCobrado { get; set; }
        public decimal? InteresCobrado { get; set; }
        public decimal? OriginacionCobrado { get; set; }
        public decimal? InteresMoraCobrado { get; set; }
        public decimal? AdelantoCapital { get; set; }
        public decimal? MontoCargoMora { get; set; }
        public decimal? CargoMoraOriginal { get; set; }
        public decimal? CargoMoraCobrado { get; set; }
        public int? Dias { get; set; }
        public double? TasaDiaria { get; set; }
        public double? TasaAnual { get; set; }
        public DateTime? FechaCorte { get; set; }
        public bool? CuotaCondonada { get; set; }
        public double? PorcentajeCondonacion { get; set; }
        public long? FkIdEncabezadoPlanPagosCredito { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal? InteresesDelMes { get; set; }
        public decimal? InteresPendiente { get; set; }
        public int? Estado { get; set; }
    }
}
