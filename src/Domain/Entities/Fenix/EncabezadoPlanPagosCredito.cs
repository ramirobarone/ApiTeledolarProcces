using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class EncabezadoPlanPagosCredito
    {
        public long IdEncabezadoPlanPagosCredito { get; set; }
        public string UsuInclusion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int Estado { get; set; }
        public int? ConsecutivoCronograma { get; set; }
        public string Descripcion { get; set; }
        public long IdCredito { get; set; }
        public decimal? Cuota { get; set; }
        public int? IdProducto { get; set; }
        public int? IdPlanMedidaPlazo { get; set; }
        public DateTime? FechaCambio { get; set; }
    }
}
