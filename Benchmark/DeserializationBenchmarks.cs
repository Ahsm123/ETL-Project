using BenchmarkDotNet.Attributes;
using ETL.Domain.Config;
using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using ETL.Domain.Rules;
using ETL.Domain.Targets.DbTargets;

namespace Benchmark;

[MemoryDiagnoser]
public class DeserializationBenchmarks
{
    private string _jsonWithType;
    private IJsonService _jsonService;

    [GlobalSetup]
    public void Setup()
    {
        _jsonService = new JsonService();

        var payload = new TransformedEvent
        {
            PipelineId = "abc123",
            Record = new RawRecord(new Dictionary<string, object>
        {
            { "Name", "Test" }
        }),
            LoadTargetConfig = new LoadTargetConfig
            {
                TargetInfo = new MsSqlTargetInfo
                {
                    ConnectionString = "test",
                    UseBulkInsert = true
                },
                Tables = new List<TargetTableConfig>
            {
                new TargetTableConfig
                {
                    TargetTable = "TestTable",
                    Fields = new List<LoadFieldMapRule>
                    {
                        new LoadFieldMapRule { SourceField = "Name", TargetField = "Name" }
                    }
                }
            }
            }
        };

        _jsonWithType = _jsonService.Serialize(payload);
    }

    [Benchmark]
    public TransformedEvent DeserializeWithPolymorphism()
    {
        return _jsonService.Deserialize<TransformedEvent>(_jsonWithType)!;
    }

    [Benchmark]
    public MsSqlTargetInfo ManualDeserializationSwitch()
    {
        var wrapper = _jsonService.Deserialize<TransformedEvent>(_jsonWithType)!;
        var raw = _jsonService.Serialize(wrapper.LoadTargetConfig.TargetInfo);
        return _jsonService.Deserialize<MsSqlTargetInfo>(raw)!;
    }
}
