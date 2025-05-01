using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.MetaDataModels
{
    public class ForeignKeyMetadata
    {
        [JsonPropertyName("column")]
        public string Column { get; set; }
        [JsonPropertyName("referencedTable")]
        public string ReferencedTable { get; set; }
        [JsonPropertyName("referencedColumn")]
        public string ReferencedColumn { get; set; }
    }
}
