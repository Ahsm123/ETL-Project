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
    [HttpPost("{pipelineId}")]
    public async Task<ActionResult<ExtractResultEvent>> TriggerExtractionAsync(string pipelineId)
    {
        if (string.IsNullOrWhiteSpace(pipelineId))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "Config ID cannot be null or empty.",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { ["pipelineId"] = pipelineId }
            });
        }

        try
        {
            var result = await _extractPipeline.RunPipelineAsync(pipelineId);

            if (result == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Extraction Failed",
                    Detail = $"No config found or extraction failed for config ID: {pipelineId}",
                    Status = StatusCodes.Status404NotFound,
                    Extensions = { ["pipelineId"] = pipelineId }
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during extraction for config ID: {pipelineId}", pipelineId);

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred during extraction.",
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { ["pipelineId"] = pipelineId }
            });
        }
    }

}
