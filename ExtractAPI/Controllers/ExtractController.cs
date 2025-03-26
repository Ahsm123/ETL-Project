using ETL.Domain.Model;
using ExtractAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExtractAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ExtractController : ControllerBase
    {
        private readonly IExtractService _extractService;

        public ExtractController(IExtractService extractService)
        {
            _extractService = extractService;
        }

        [HttpPost("{configId}")]
        public async Task<ActionResult<ConfigFile>> Trigger(string configId)
        {
            try
            {
                var configWithData = await _extractService.ExtractAsync(configId);
                return Ok(configWithData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message});
            }
        }
    }

}
