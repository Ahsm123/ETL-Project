using ETL.Domain.Events;
using ExtractAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExtractAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExtractController : ControllerBase
{
    private readonly IExtractPipeline _extractPipeline;
    private readonly ILogger<ExtractController> _logger;

    public ExtractController(
        IExtractPipeline extractPipeline,
        ILogger<ExtractController> logger)
    {
        _extractPipeline = extractPipeline;
        _logger = logger;
    }

    [HttpPost("{configId}")]
    public async Task<ActionResult<ExtractResultEvent>> Trigger(string configId)
    {
        try
        {
            var result = await _extractPipeline.ExtractAsync(configId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl under extract for config med ID: {ConfigId}", configId);
            return BadRequest(new { error = ex.Message });
        }
    }

}
