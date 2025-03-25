using ExtractAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExtractAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtractController : ControllerBase
    {
        private readonly IExtractService _extractService;

        public ExtractController(IExtractService extractService)
        {
            _extractService = extractService;
        }

        [HttpPost("{configId}")]
        public async Task<IActionResult> Trigger(string configId)
        {
            await _extractService.ExtractAsync(configId);
            return Ok("Pipeline triggerede for configId" + configId);
        }
    }
}
