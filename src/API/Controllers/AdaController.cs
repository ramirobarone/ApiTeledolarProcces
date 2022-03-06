using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities.Teledolar.Ada;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdaController : ControllerBase
    {
        private readonly IAdaService _adaService;
        public AdaController(IAdaService adaService)
        {
            _adaService = adaService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(string Identification)
        {
            return Ok(_adaService.StatusAda(Identification));
        }

        [HttpPost]
        public async Task<IActionResult> Post(RequestAda ada)
        {

            try
            {
                ResponseAda AdaResult = await _adaService.RegisterAda(ada);

                if (AdaResult != null)
                {
                    return Ok(AdaResult);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (AdaException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
