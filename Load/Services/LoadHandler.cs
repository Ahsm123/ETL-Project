using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using ETL.Domain.NewFolder;
using ETL.Domain.Targets.DbTargets;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using ETL.Domain.MetaDataModels;
using Load.Messaging.Interfaces;
using Load.Services.Interfaces;

namespace Load.Services;

public class LoadHandler : ILoadHandler
{
    private readonly ITargetWriterResolver _targetWriterResolver;
    private readonly IServiceProvider _serviceProvider;
    private readonly IJsonService _jsonService;
    private readonly ILogger<LoadHandler> _logger;
    private readonly IMessagePublisher _messagePublisher;
    private readonly string _deadLetterTopic = "dead-letter"; 

    public LoadHandler(
        ITargetWriterResolver targetWriterResolver,
        IServiceProvider serviceProvider,
        IJsonService jsonService,
        ILogger<LoadHandler> logger,
        IMessagePublisher messagePublisher)
    {
        _targetWriterResolver = targetWriterResolver;
        _serviceProvider = serviceProvider;
        _jsonService = jsonService;
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    public async Task HandleAsync(string json)
    {
        try
        {
            _logger.LogWarning("📥 Incoming raw JSON:\n{Json}", json);

            if (string.IsNullOrWhiteSpace(json) || json == "{}")
                throw new InvalidOperationException("Received empty or invalid payload.");

            var payload = _jsonService.Deserialize<TransformedEvent>(json)
                ?? throw new InvalidOperationException("Failed to deserialize payload.");

            _logger.LogInformation("✅ Deserialized table count: {Count}", payload.LoadTargetConfig?.Tables?.Count ?? -1);

            if (payload.LoadTargetConfig?.TargetInfo == null)
                throw new InvalidOperationException("TargetInfo is missing from payload.");

            if (payload.LoadTargetConfig.Tables == null || payload.LoadTargetConfig.Tables.Count == 0)
                throw new InvalidOperationException("Tables are missing or empty in LoadTargetConfig.");

            var writer = _targetWriterResolver.Resolve(payload.LoadTargetConfig.TargetInfo.GetType(), _serviceProvider)
                ?? throw new InvalidOperationException($"No writer found for type '{payload.LoadTargetConfig.TargetInfo.GetType()}'");

            _logger.LogInformation("📦 Using writer: {Writer}", writer.GetType().Name);

            var context = new LoadContext
            {
                TargetInfo = payload.LoadTargetConfig.TargetInfo,
                Data = payload.Record.GetNormalized(),
                PipelineId = payload.PipelineId,
                Tables = payload.LoadTargetConfig.Tables
            };

            await writer.WriteAsync(context);
            _logger.LogInformation("✅ Load completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ LoadHandler failed: {Message}", ex.Message);
            await SendToDeadLetter(json, ex.Message);
            throw;
        }
    }

    private async Task SendToDeadLetter(string originalMessage, string reason)
    {
        try
        {
            var envelope = new
            {
                Error = reason,
                Timestamp = DateTime.UtcNow,
                OriginalPayload = originalMessage
            };

            var payload = JsonSerializer.Serialize(envelope);
            await _messagePublisher.PublishAsync(Guid.NewGuid().ToString(), payload);
            _logger.LogWarning("📨 Message sent to dead-letter topic: {Topic}", _deadLetterTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Failed to publish to dead-letter topic.");
        }
    }

}

