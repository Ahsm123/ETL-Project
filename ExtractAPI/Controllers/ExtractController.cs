using ETL.Domain.Model;
using ExtractAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtractAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ExtractController : ControllerBase
    {
        private readonly IDataExtractionService _extractService;

        public ExtractController(IDataExtractionService extractService)
        {
            _extractService = extractService;
        }

        [HttpPost("{configId}")]
        public async Task<ActionResult<ConfigFile>> Trigger(string configId)
        {
            try
            {
                ConfigFile configWithData = await _extractService.ExtractAndDispatchAsync(configId);
                return Ok(configWithData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }


    // parse json så det kan læse
    // extract fields, find source og targetfield
    // map det til c# objekter for at arbejde med det
    // ændre fields så source property bliver target property.
    // erstatte det gamle json med det nye json
    // serialize til json
    // returnere det nye json og fjerne transform dele i config

}
