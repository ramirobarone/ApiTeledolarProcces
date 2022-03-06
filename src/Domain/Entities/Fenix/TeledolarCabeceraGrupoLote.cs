using System;
using System.Collections.Generic;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class TeledolarCabeceraGrupoLote
    {
        public TeledolarCabeceraGrupoLote()
        {
            ResultadoDomiciliacionTeleDolars = new HashSet<ResultadoDomiciliacionTeleDolar>();
        }

        public int Id { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public string Lotes { get; set; }
        public decimal? TotalDomiciliado { get; set; }

        public virtual ICollection<ResultadoDomiciliacionTeleDolar> ResultadoDomiciliacionTeleDolars { get; set; }
    }
}
