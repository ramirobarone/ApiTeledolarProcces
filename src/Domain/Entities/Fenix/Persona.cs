using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class Persona
    {
        public int Id { get; set; }
        public int? IdTipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public DateTime? VencimientoIdentificacion { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public int? TelefonoCel { get; set; }
        public int? TelefonoFijo { get; set; }
        public int? TelefonoLaboral { get; set; }
        public string Correo { get; set; }
        public string CorreoOpcional { get; set; }
        public int? EstadoCivil { get; set; }
        public int? Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Provincia { get; set; }
        public int? Canton { get; set; }
        public int? Distrito { get; set; }
        public string DetalleDireccion { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsrModifica { get; set; }
        public int? IdStatusOrden { get; set; }
        public int? IdTAgenciasExternas { get; set; }
        public DateTime? FechaIngresoAgenciaExterna { get; set; }
        public string CorreoDomiciliacion { get; set; }
        public string CorreoDomiciliacionAlternativo { get; set; }
        public int? CorreoValido { get; set; }
    }
}
