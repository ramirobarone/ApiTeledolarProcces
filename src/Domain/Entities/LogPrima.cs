using System;

namespace Domain.Entities
{
    public class LogPrima
    {
        public int Id { get; set; }
        public string Enviado { get; set; }
        public string Recibido { get; set; }
        public DateTime FechaPrima { get; set; } = DateTime.Now;
    }
}
