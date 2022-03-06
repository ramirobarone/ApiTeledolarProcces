using System;

namespace Domain.Entities.Fenix
{
    public class CuentaBancariaVerificadaTeledolar
    {
        public int Id { get; set; }
        public string Iban { get; set; }
        public string Identificacion { get; set; }
        public DateTime FechaVerificacion { get; set; }
        public bool CuentaVerificadaOk { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string MensajeTeledolar { get; set; }
    }
}
