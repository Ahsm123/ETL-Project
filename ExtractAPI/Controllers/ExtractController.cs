using ETL.Domain.Events;
using ExtractAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtractAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExtractController : ControllerBase
{
    private readonly IDataExtractionService _dataExtractionService;
    private readonly ILogger<ExtractController> _logger;

    public ExtractController(
        IDataExtractionService dataExtractionService,
        ILogger<ExtractController> logger)
    {
        _dataExtractionService = dataExtractionService;
        _logger = logger;
    }

    [HttpPost("{configId}")]
    public async Task<ActionResult<ExtractResultEvent>> Trigger(string configId)
    {
        try
        {
            var result = await _dataExtractionService.ExtractAsync(configId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl under extract for config med ID: {ConfigId}", configId);
            return BadRequest(new { error = ex.Message });
        }
    }

}
