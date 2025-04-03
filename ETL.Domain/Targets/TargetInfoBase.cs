using ETL.Domain.Targets.ApiTargets;
using ETL.Domain.Targets.DbTargets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MsSqlTargetInfo), "mssql")]
[JsonDerivedType(typeof(RestApiTargetInfo), "restapi")]
public abstract class TargetInfoBase
{
}
