using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService healthService;

        public HealthController(IHealthService healthService)
        {
            this.healthService = healthService;
        }
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(healthService.ProbarConexion());

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
