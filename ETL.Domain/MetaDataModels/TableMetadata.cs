using Mysqlx.Resultset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.MetaDataModels
{
    public class TableMetadata
    {
        [JsonPropertyName("tableName")]
        public string TableName { get; set; }
        [JsonPropertyName("columns")]
        public List<ColumnMetadata> Columns { get; set; } = new();
        [JsonPropertyName("primaryKeys")]
        public List<string> PrimaryKeys { get; set; } = new();
        [JsonPropertyName("foreignKeys")]
        public List<ForeignKeyMetadata> ForeignKeys { get; set; } = new();
    }
}
