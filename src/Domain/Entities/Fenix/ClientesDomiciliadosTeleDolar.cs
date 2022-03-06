using System;

#nullable disable

namespace Domain.Entities.Fenix
{
    public partial class ClientesDomiciliadosTeleDolar
    {
        public int Id { get; set; }
        public string Identificacion { get; set; }
        public decimal? MontoDomiciliado { get; set; }
        public int? EstatusDomiciliacion { get; set; }
        public string Codigo { get; set; }
        public long? IdPersona { get; set; }
        public int? IdSolicitud { get; set; }
        public string Comentario { get; set; }
        public string UsuarioIngresa { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
        public DateTime? FechaIngresa { get; set; }
        public bool? DebitoAutomatico { get; set; }
        public DateTime? FechaDebitoAutomatico { get; set; }
        public string UsrDebitoAutomatico { get; set; }
        public long? FkClientesDomiciliados { get; set; }
        public int? Idcredito { get; set; }
        public string UltimaRespuestaDelBacRegistrada { get; set; }
        public int? DiasMora { get; set; }
        public int? EsFisico { get; set; }
        public bool? Generado { get; set; }
        public string Servicio { get; set; }
        public string TitularServicio { get; set; }
        public string IdClienteOrigen { get; set; }
        public string ClienteOrigen { get; set; }
        public string Ccorigen { get; set; }
        public string EmailClienteOrigen { get; set; }
        public string IdClienteDestino { get; set; }
        public string EmailClienteDestino { get; set; }
        public string Ccdestino { get; set; }
        public decimal? Monto { get; set; }
        public string CodMoneda { get; set; }
        public string Descripcion { get; set; }
        public string Canal { get; set; }
        public string RefInterna { get; set; }
        public string FecValor { get; set; }
        public string FecFinalVigencia { get; set; }
        public bool? ProcesadoXml { get; set; }
        public bool? RespuestaXml { get; set; }
        public string CobrarComision { get; set; }
        public int? Estado { get; set; }
        public int? EstadoExclusion { get; set; }
        public string AreaSolicitante { get; set; }
        public DateTime? FechaVigenciaExclu { get; set; }
        public DateTime? FechaModificaExclu { get; set; }
        public string UsrModifica { get; set; }
        public int? RegistroTeledolar { get; set; }
        public string DescripcionTeledolar { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
