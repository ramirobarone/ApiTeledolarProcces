using Domain.Entities.Teledolar.Ada;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAdaService
    {
        Task<ResponseAda> RegisterAda(RequestAda ada);

        Task<ResponseAda> StatusAda(string Identification);
    }
}
