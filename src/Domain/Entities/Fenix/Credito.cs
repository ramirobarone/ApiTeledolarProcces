using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class Credito
    {
        public Credito()
        {
            GestionCobros = new HashSet<GestionCobro>();
        }

        public int Id { get; set; }
        public int IdPersona { get; set; }
        public int IdSolicitud { get; set; }
        public int IdTipo { get; set; }
        public string Comentario { get; set; }
        public DateTime? FechaCredito { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsrModifica { get; set; }
        public DateTime? FechaTransferencia { get; set; }
        public DateTime? FechaFirma { get; set; }
        public double? InteresCorriente { get; set; }
        public double? InteresMora { get; set; }
        public decimal? MontoCuota { get; set; }
        public decimal? CapitalPendiente { get; set; }
        public string Identificacion { get; set; }
        public int? IdProducto { get; set; }
        public int? NumCredito { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal? SaldoDia { get; set; }
        public decimal? SaldoTotal { get; set; }
        public decimal? Matriz { get; set; }
        public DateTime? FechaUltimoCargoMora { get; set; }
        public DateTime? FechaUltimoPago { get; set; }
        public string NotaPermanente { get; set; }
        public int? EstadoCredito { get; set; }
        public int? SubEstadoCredito { get; set; }
        public decimal? MontoFinanciado { get; set; }
        public decimal? MontoOriginacion { get; set; }
        public long? IdLineaCredito { get; set; }
        public int? IdCicloPago { get; set; }
        public string FactorPaga { get; set; }
        public string CondicionFinanciera { get; set; }
        public long? IdPlanMedidaPlazo { get; set; }
        public int? Plazo { get; set; }
        public decimal? TasaInteresNormal { get; set; }
        public long? IdMoneda { get; set; }
        public DateTime? FechaPrimerPago { get; set; }
        public int? FkProcedencia { get; set; }
        public bool ActualizaFechaPago { get; set; }
        public int? IdCandado { get; set; }

        public virtual ICollection<GestionCobro> GestionCobros { get; set; }
    }
}
