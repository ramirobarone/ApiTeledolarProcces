using Domain.Entities.Teledolar.CreateBatch;
using Domain.Entities.Teledolar.GetBatch;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITeledolarProxy
    {
        Task<BatchResponse> CreateBatch(BatchRequest request);
        Task<Response_Domiciliacion> GetBatchStatus(string batchId, int pageSize, int page);
        string GetConstSettings();
    }
}
