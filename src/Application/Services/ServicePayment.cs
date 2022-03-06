using Application.Common.Interfaces;
using Domain.Entities.CommodEntities;
using Domain.Entities.Teledolar.SingleDomiciliation;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServicePayment : IServicePayment
    {
        private IServiceSingleDomiciliation ProxySingleDomiciliation { get; }

        public ServicePayment(IServiceSingleDomiciliation proxySingleDomiciliation)
        {
            ProxySingleDomiciliation = proxySingleDomiciliation;
        }

        public Task<ResultDomiciliation> ExecuteDomiciliation(CreateTransaction execute)
        {

            return ProxySingleDomiciliation.ExecuteSingleDomiciliation(execute);
        }
    }
}
