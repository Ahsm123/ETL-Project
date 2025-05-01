using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.MetaDataModels
{
    public class DatabaseMetaData
    {
        [JsonPropertyName("tables")]
        public List<TableMetadata> Tables { get; set; }
    }
}
