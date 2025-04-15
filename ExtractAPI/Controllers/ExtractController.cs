using ETL.Domain.Events;
using ExtractAPI.Interfaces;
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

    [ProducesResponseType(typeof(ExtractResultEvent), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{configId}")]
    public async Task<ActionResult<ExtractResultEvent>> TriggerExtractionAsync(string configId)
    {
        if (string.IsNullOrWhiteSpace(configId))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "Config ID cannot be null or empty.",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { ["configId"] = configId }
            });
        }

        try
        {
            var result = await _extractPipeline.RunPipelineAsync(configId);

            if (result == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Extraction Failed",
                    Detail = $"No config found or extraction failed for config ID: {configId}",
                    Status = StatusCodes.Status404NotFound,
                    Extensions = { ["configId"] = configId }
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during extraction for config ID: {ConfigId}", configId);

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred during extraction.",
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { ["configId"] = configId }
            });
        }
    }

}
