using Domain.Entities.Teledolar.SingleDomiciliation;

namespace Domain.Entities.CommodEntities
{
    public class CreateTransaction : ExecuteTransaction
    {
        public string identification { get; set; }
        public int IdSolicitud { get; set; }
        public int IdCredito { get; set; }
    }
}
