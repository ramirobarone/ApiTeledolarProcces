using Domain.Entities.Teledolar.Account;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.BankAccount
{
    public interface IServiceBankAccount
    {
        Task<ResponseAccountClient> GetBankAccountAsync(string iban, string identification, string typeIdentification);
    }
}
