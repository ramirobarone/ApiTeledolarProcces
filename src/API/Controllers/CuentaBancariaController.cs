using Application.Common.Interfaces.BankAccount;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaBancariaController : ControllerBase
    {
        private readonly IServiceBankAccount serviceBankAccount;

        public CuentaBancariaController(IServiceBankAccount serviceBankAccount)
        {
            this.serviceBankAccount = serviceBankAccount;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string iban, string identificacion, string tipoIdentificacion)
        {

            return Ok(await serviceBankAccount.GetBankAccountAsync(iban, identificacion, tipoIdentificacion));
        }
    }
}
