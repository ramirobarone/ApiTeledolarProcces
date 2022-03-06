using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class FiltrosProcesoDomiciliacion
    {
        public int Id { get; set; }
        public int? MinDiasMora { get; set; }
        public int? MaxDiasMora { get; set; }
        public decimal? MontoMora { get; set; }
        public DateTime? FechaUltimoCobro { get; set; }
        public bool? PromesaPago { get; set; }
        public int? RenovadorNuevoAmbos { get; set; }
        public bool? RechazoCurso { get; set; }
        public bool? ProcesadoCurso { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool? Estado { get; set; }
    }
}
