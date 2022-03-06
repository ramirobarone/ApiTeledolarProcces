#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class TabConfigSy
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public double? ConfiguracionInt { get; set; }
        public string ConfiguracionChar { get; set; }
        public bool? Activo { get; set; }
        public string LlaveConfig1 { get; set; }
        public string LlaveConfig2 { get; set; }
        public string LlaveConfig3 { get; set; }
        public string LlaveConfig4 { get; set; }
        public string LlaveConfig5 { get; set; }
        public int? DatoInt1 { get; set; }
        public int? DatoInt2 { get; set; }
        public int? DatoInt3 { get; set; }
        public string DatoChar1 { get; set; }
        public string DatoChar2 { get; set; }
        public string DatoChar3 { get; set; }
        public string CodigoPais { get; set; }
    }
}
