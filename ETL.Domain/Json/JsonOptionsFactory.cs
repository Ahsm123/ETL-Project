using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETL.Domain.Json;

public static class JsonOptionsFactory
{
    public static JsonSerializerOptions Default => new()
    {
        PropertyNameCaseInsensitive = true
    };
}
