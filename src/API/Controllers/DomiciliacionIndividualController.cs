using Application.Common.Interfaces;
using Domain.Entities.CommodEntities;
using Domain.Entities.Teledolar.SingleDomiciliation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomiciliacionIndividualController : ControllerBase
    {
        private readonly IServicePayment servicePayment;

        public DomiciliacionIndividualController(IServicePayment servicePayment)
        {
            this.servicePayment = servicePayment;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateTransaction createTransaction)
        {
            try
            {
                var resultSingleDomiciliation = await servicePayment.ExecuteDomiciliation(createTransaction);

                return Ok(resultSingleDomiciliation);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
    }
}