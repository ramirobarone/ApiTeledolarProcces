using Domain.Entities.CommodEntities;
using Domain.Entities.Teledolar.SingleDomiciliation;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IServiceDomiciliationToDataBase
    {
        Task<int> SaveChanges(ResultDomiciliation resultDomiciliation);
    }
}
