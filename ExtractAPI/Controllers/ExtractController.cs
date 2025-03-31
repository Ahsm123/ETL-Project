using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using ExtractAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtractAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExtractController : ControllerBase
{
    private readonly IDataExtractionService _extractService;
    private readonly ILogger<ExtractController> _logger;

    public ExtractController(
        IDataExtractionService extractService,
        ILogger<ExtractController> logger)
    {
        _extractService = extractService;
        _logger = logger;
    }

    [HttpPost("{configId}")]
    public async Task<ActionResult<ExtractResponseDto>> Trigger(string configId)
    {
        try
        {
            var result = await _extractService.ExtractAsync(configId);
            return Ok(result); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl under extract for config med ID: {ConfigId}", configId);
            return BadRequest(new { error = ex.Message });
        }
    }

}
