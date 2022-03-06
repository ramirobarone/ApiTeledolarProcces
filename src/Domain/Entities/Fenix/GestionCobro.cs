using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class GestionCobro
    {
        public int IdGestionCobro { get; set; }
        public int? IdCredito { get; set; }
        public DateTime? FechaGestion { get; set; }
        public string Detalle { get; set; }
        public string FechaPromesaPago { get; set; }
        public decimal? MontoPromesaPago { get; set; }
        public int? Tipollamada { get; set; }
        public string UsrModifica { get; set; }
        public DateTime? FechaPromPago { get; set; }
        public bool? Realizada { get; set; }
        public bool? Activo { get; set; }
        public string Accion { get; set; }
        public string IdRespuestaGestion { get; set; }
        public bool? BndAvisoPromesa { get; set; }
        public bool? BndAvisoPromesaVencio { get; set; }
        public int? DiasMora { get; set; }
        public string IdMotivoMora { get; set; }

        public virtual Credito IdCreditoNavigation { get; set; }
    }
}
