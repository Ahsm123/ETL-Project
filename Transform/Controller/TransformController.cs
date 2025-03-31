
using ETL.Domain.Model.DTOs;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transform.Kafka;
using Transform.Services;

namespace Transform.Controller
{
        public class TransformController : BackgroundService
        {
            private readonly IKafkaConsumer _kafkaConsumer;
            private readonly IKafkaProducer _kafkaProducer;
            private readonly ITransformService<string> _transformService;

            public TransformController(
                IKafkaConsumer kafkaConsumer,
                IKafkaProducer kafkaProducer,
                ITransformService<string> transformService)
            {
                _kafkaConsumer = kafkaConsumer;
                _kafkaProducer = kafkaProducer;
                _transformService = transformService;
            }


            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
            await _kafkaConsumer.ConsumeAsync(stoppingToken, async (string message) =>
                {
                    try
                    {
                        var payload = System.Text.Json.JsonSerializer.Deserialize<ExtractedPayload>(message);
                        var transformed = await _transformService.TransformDataAsync(payload);
                        await _kafkaProducer.ProduceAsync("processedData", Guid.NewGuid().ToString(), transformed);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error transforming message: {e.Message}");
                    }
                });
            }
        }
}






