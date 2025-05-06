using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.MetaDataModels
{
    public class ColumnMetadata
    {
        [JsonPropertyName("columnName")]
        public string ColumnName { get; set; }

        [JsonPropertyName("dataType")]
        public string DataType { get; set; }

        [JsonPropertyName("isNullable")]
        public bool IsNullable { get; set; }

        [JsonPropertyName("maxLength")]
        public int? MaxLength { get; set; }

        [JsonPropertyName("isAutoIncrement")]
        public bool IsAutoIncrement { get; set; }

        [JsonPropertyName("isPrimaryKey")]
        public bool IsPrimaryKey { get; set; }
    }

}
