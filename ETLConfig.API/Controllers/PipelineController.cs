using ETL.Domain.Config;
using ETL.Domain.Model;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using ETLConfig.API.Models.DTOs;
using ETLConfig.API.Services.Interfaces;
using ETLConfig.API.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ETLConfig.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PipelineController : ControllerBase
{
    private readonly IConfigProcessingService _processingService;
    private readonly IConnectionValidatorResolver _validatorResolver;

    public PipelineController(IConfigProcessingService processingService, IConnectionValidatorResolver validatorResolver)
    {
        _processingService = processingService;
        _validatorResolver = validatorResolver;
    }

    [HttpGet]
    public async Task<ActionResult<List<JsonElement>>> GetAllPipelines()
    {
        var configs = await _processingService.GetAllConfigsAsync();
        return Ok(configs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JsonElement>> GetPipelineById(string id)
    {
        var config = await _processingService.GetConfigByIdAsync(id);
        return config is null ? NotFound() : Ok(config);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePipeline([FromBody] JsonElement rawJson)
    {
        var config = await _processingService.ProcessSingleConfigAsync(rawJson);
        return CreatedAtAction(nameof(GetPipelineById), new { id = config.Id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePipeline(string id, [FromBody] JsonElement rawJson)
    {
        var jsonId = rawJson.GetProperty("Id").GetString();
        if (!string.Equals(id, jsonId, StringComparison.OrdinalIgnoreCase))
            return BadRequest("ID does not match ID in body.");

        if (await _processingService.GetConfigByIdAsync(id) is null)
            return NotFound();

        await _processingService.UpdateConfigAsync(id, rawJson);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePipeline(string id)
    {
        if (await _processingService.GetConfigByIdAsync(id) is null)
            return NotFound();

        await _processingService.DeleteConfigAsync(id);
        return NoContent();
    }

    // Validation & Metadata

    [HttpPost("metadata")]
    public async Task<IActionResult> GetMetadata([FromBody] ConnectionValidationRequest request)
    {
        var validator = ResolveOrThrow(request.Type);

        if (!await validator.IsValidAsync(request.ConnectionString))
            return BadRequest("Connection failed.");

        var metadata = await validator.GetMetadataAsync(request.ConnectionString);
        return Ok(metadata);
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateConnection([FromBody] ConnectionValidationRequest request)
    {
        var validator = ResolveOrThrow(request.Type);
        var isValid = await validator.IsValidAsync(request.ConnectionString);
        return Ok(new { isValid });
    }

    // Available Types

    [HttpGet("types")]
    public IActionResult GetAvailableTypes()
    {
        var sources = TypeDiscoveryHelper.GetJsonDerivedTypes(typeof(SourceInfoBase));
        var targets = TypeDiscoveryHelper.GetJsonDerivedTypes(typeof(TargetInfoBase));
        return Ok(new { sources, targets });
    }

    [HttpGet("example")]
    public IActionResult GetExamplePipeline()
    {
        var example = new ConfigFile
        {
            Id = "example-pipeline",
            Name = "ETL: Active Users Sync",
            Description = "Syncs active users from source database to target warehouse.",
            Version = "1.0",
            ExtractConfig = new ExtractConfig
            {
                SourceInfo = new MsSqlSourceInfo
                {
                    ConnectionString = "Server=localhost;Database=mydb;Trusted_Connection=True;",
                    TargetTable = "Users",
                    UseTrustedConnection = true
                },
                Fields = new() { "Id", "Name", "Email", "Spendings", "IsActive" },
                Filters = new() { new FilterRule("IsActive", "equals", "true") }
            },
            TransformConfig = new TransformConfig
            {
                Mappings = new()
                {
                    new FieldMapRule { SourceField = "Email", TargetField = "UserEmail" },
                    new FieldMapRule { SourceField = "Name", TargetField = "FullName" }
                },
                Filters = new()
                {
                    new FilterRule("Spendings", "greaterthan", "1000")
                }
            },
            LoadTargetConfig = new LoadConfig
            {
                TargetInfo = new MySqlTargetInfo
                {
                    ConnectionString = "Server=localhost;Database=targetdb;Trusted_Connection=True;",
                    LoadMode = "append"
                },
                Tables = new()
                {
                    new TargetTableConfig
                    {
                        TargetTable = "ActiveUsers",
                        Fields = new()
                        {
                            new FieldMapRule { SourceField = "UserEmail", TargetField = "Email" },
                            new FieldMapRule { SourceField = "FullName", TargetField = "Name" },
                            new FieldMapRule { SourceField = "Spendings", TargetField = "Spendings" }
                        }
                    },
                    new TargetTableConfig
                    {
                        TargetTable = "UserLogs",
                        Fields = new()
                        {
                            new FieldMapRule { SourceField = "UserEmail", TargetField = "UserEmail" },
                            new FieldMapRule { SourceField = "Spendings", TargetField = "AmountSpent" }
                        }
                    }
                }
            }
        };

        return Ok(example);
    }

    // Helpers

    private IConnectionValidator ResolveOrThrow(string type)
    {
        return _validatorResolver.Resolve(type)
            ?? throw new ArgumentException($"Unsupported type '{type}'");
    }
}
