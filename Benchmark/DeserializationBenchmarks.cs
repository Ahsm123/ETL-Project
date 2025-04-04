using BenchmarkDotNet.Attributes;
using ETL.Domain.Config;
using ETL.Domain.Events;
using ETL.Domain.Json;
using ETL.Domain.Targets.DbTargets;
using System.Text.Json;

namespace Benchmark;

[MemoryDiagnoser]
public class DeserializationBenchmarks
{
    private string _jsonWithType;
    private JsonSerializerOptions _options;

    [GlobalSetup]
    public void Setup()
    {
        var payload = new TransformedEvent
        {
            PipelineId = "abc123",
            Data = new Dictionary<string, object>
        {
            { "Name", "Test" }
        },
            LoadTargetConfig = new LoadTargetConfig
            {
                TargetInfo = new MsSqlTargetInfo
                {
                    ConnectionString = "test",
                    TargetTable = "TestTable",
                    UseBulkInsert = true
                }
            }
        };

        _options = JsonOptionsFactory.Default;
        _jsonWithType = JsonSerializer.Serialize(payload, _options);
    }

    [Benchmark]
    public TransformedEvent DeserializeWithPolymorphism()
    {
        return JsonSerializer.Deserialize<TransformedEvent>(_jsonWithType, _options);
    }

    [Benchmark]
    public MsSqlTargetInfo ManualDeserializationSwitch()
    {
        var wrapper = JsonSerializer.Deserialize<TransformedEvent>(_jsonWithType, _options);
        var raw = JsonSerializer.Serialize(wrapper.LoadTargetConfig.TargetInfo);
        return JsonSerializer.Deserialize<MsSqlTargetInfo>(raw);
    }
}
