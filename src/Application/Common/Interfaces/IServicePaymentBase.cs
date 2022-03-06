using Domain.Entities.CommodEntities;
using Domain.Entities.Teledolar.SingleDomiciliation;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IServicePaymentBase
    {
        Task<ResultDomiciliation> ExecuteDomiciliation(CreateTransaction execute);
    }
}
