using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Extract.DataSources
{
    public interface IDataSourceProvider
    {
        Task<JsonElement> GetDataAsync(string sourceInfo);
    }
}
