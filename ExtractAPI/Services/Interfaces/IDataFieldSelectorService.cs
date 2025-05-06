using ETL.Domain.Events;
using System.Text.Json;

namespace ExtractAPI.Services.Interfaces;

public interface IDataFieldSelectorService
{
    IEnumerable<RawRecord> SelectRecords(JsonElement data, List<string>? fields);
}
