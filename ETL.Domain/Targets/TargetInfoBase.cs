using ETL.Domain.Targets.ApiTargets;
using ETL.Domain.Targets.DbTargets;
using ETL.Domain.Targets.FileTargets;
using System.Text.Json.Serialization;

namespace ETL.Domain.Targets;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MsSqlTargetInfo), "mssql")]
[JsonDerivedType(typeof(MySqlTargetInfo), "mysql")]
[JsonDerivedType(typeof(RestApiTargetInfo), "restapi")]
[JsonDerivedType(typeof(ExcelTargetInfo), "excel")]
public abstract class TargetInfoBase
{
}
