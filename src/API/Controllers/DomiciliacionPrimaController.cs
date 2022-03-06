using Application.Common.Interfaces;
using Domain.Entities.CommodEntities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomiciliacionPrimaController : ControllerBase
    {
        private IServicePaymentBase _servicePayment { get; }
        public DomiciliacionPrimaController(IServicePaymentExtra servicePayment)
        {
            _servicePayment = servicePayment;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateTransaction executeTransaction)
        {
            var result = await _servicePayment.ExecuteDomiciliation(executeTransaction);

            return Ok(result);
        }
    }
}
