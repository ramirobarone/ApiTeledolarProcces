using Domain.Entities.Teledolar.Account;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IServiceProxy : IServiceProxyGet
    {
    }
    public interface IServiceProxyGet
    {
        Task<ResponseAccountClient> Get(string iban, string identification, string typeIdentification);
    }
}
