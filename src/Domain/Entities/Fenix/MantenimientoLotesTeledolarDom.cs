using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class MantenimientoLotesTeledolarDom
    {
        public int Id { get; set; }
        public int? Paso { get; set; }
        public string Descripcion { get; set; }
        public decimal? MontoDomiciliacion { get; set; }
        public decimal? PorcentajeMontoMora { get; set; }
        public bool? DebitoAutomatico { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool? Estado { get; set; }
    }
}
