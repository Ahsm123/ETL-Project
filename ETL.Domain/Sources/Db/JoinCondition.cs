using System.Text.Json.Serialization;

namespace ETL.Domain.Sources.Db;

public class JoinCondition
{
    [JsonPropertyName("Left")]
    public string Left { get; set; }  // ex., "Users.Id"

    [JsonPropertyName("Right")]
    public string Right { get; set; } // ex, "Orders.UserId"
}
