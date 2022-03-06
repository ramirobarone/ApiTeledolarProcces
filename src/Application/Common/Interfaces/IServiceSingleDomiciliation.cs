using Domain.Entities.Teledolar.SingleDomiciliation;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IServiceSingleDomiciliation
    {
        Task<ResultDomiciliation> ExecuteSingleDomiciliation(ExecuteTransaction executeTransaction);
    }
}
