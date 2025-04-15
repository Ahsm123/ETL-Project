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

    //CRUD

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
        if (config == null) return NotFound();
        return Ok(config);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePipeline([FromBody] JsonElement rawJson)
    {
        try
        {
            if (rawJson.ValueKind == JsonValueKind.Array)
            {
                var result = await _processingService.ProcessMultipleConfigsAsync(rawJson);
                return Ok(result);
            }

            var config = await _processingService.ProcessSingleConfigAsync(rawJson);
            return CreatedAtAction(nameof(GetPipelineById), new { id = config.Id }, null);
        }
        catch (JsonException ex)
        {
            return BadRequest($"Invalid JSON structure: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePipeline(string id, [FromBody] JsonElement rawJson)
    {
        var jsonId = rawJson.GetProperty("Id").GetString();
        if (!string.Equals(id, jsonId, StringComparison.OrdinalIgnoreCase))
            return BadRequest("Id in URL does not match Id in body.");

        var existing = await _processingService.GetConfigByIdAsync(id);
        if (existing == null) return NotFound();

        await _processingService.UpdateConfigAsync(id, rawJson);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePipeline(string id)
    {
        var config = await _processingService.GetConfigByIdAsync(id);
        if (config == null) return NotFound();

        await _processingService.DeleteConfigAsync(id);
        return NoContent();
    }

    //MetaData and Validation

    [HttpPost("metadata")]
    public async Task<IActionResult> GetMetadata([FromBody] ConnectionValidationRequest request)
    {
        var validator = GetValidatorOrBadRequest(request.Type, out var badRequest);
        if (badRequest != null) return badRequest;

        if (!await validator!.IsValidAsync(request.ConnectionString))
            return BadRequest("Connection failed.");

        try
        {
            var metadata = await validator.GetMetadataAsync(request.ConnectionString);
            return Ok(metadata);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving metadata.",
                error = ex.Message
            });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateConnection([FromBody] ConnectionValidationRequest request)
    {
        var validator = GetValidatorOrBadRequest(request.Type, out var badRequest);
        if (badRequest != null) return badRequest;

        var isValid = await validator!.IsValidAsync(request.ConnectionString);
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

            ExtractConfig = new ExtractConfig
            {
                SourceInfo = new MsSqlSourceInfo
                {
                    ConnectionString = "Server=localhost;Database=mydb;Trusted_Connection=True;",
                    TargetTable = "Users",
                    UseTrustedConnection = true
                },
                Fields = new List<string> { "Id", "Name", "Email", "Spendings", "IsActive"},
                Filters = new List<FilterRule>
            {
                new() { Field = "IsActive", Operator = "equals", Value = "true" }
            }
            },

            TransformConfig = new TransformConfig
            {
                Filters = new List<FilterRule>
            {
                new() { Field = "Spendings", Operator = "greather_than", Value = "1000" }
            },
                Mappings = new List<FieldMapRule>
            {
                new() { SourceField = "Email", TargetField = "UserEmail" },
                new() { SourceField = "Name", TargetField = "FullName" }
            }
            },

            LoadTargetConfig = new LoadTargetConfig
            {
                TargetInfo = new MsSqlTargetInfo
                {
                    ConnectionString = "Server=localhost;Database=targetdb;Trusted_Connection=True;",
                    TargetTable = "ActiveUsers",
                    UseBulkInsert = true
                }
            }
        };

        return Ok(example);
    }




    //Helpers

    private IConnectionValidator? GetValidatorOrBadRequest(string type, out IActionResult? badRequest)
    {
        var validator = _validatorResolver.Resolve(type);
        if (validator == null)
        {
            badRequest = BadRequest($"Unsupported type '{type}'");
            return null;
        }

        badRequest = null;
        return validator;
    }

}
