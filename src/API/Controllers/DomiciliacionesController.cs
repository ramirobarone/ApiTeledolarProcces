using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [AllowAnonymous] //[Authorize (AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class ClienteController : BaseController
    {

        private readonly ILogger<ClienteController> _logger;
        private readonly IDomiciliacionesService _domis;

        public ClienteController(ILogger<ClienteController> logger, IDomiciliacionesService domis)
        {
            _logger = logger;
            _domis = domis;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("DomiciliationPerBatches")]
        public async Task<ActionResult> DomiciliationPerBatches()
        {
            await _domis.DomiciliationPerBatches();
            return Ok();
        }
    }
}
