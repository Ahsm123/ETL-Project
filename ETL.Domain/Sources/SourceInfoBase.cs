using System.Text.Json.Serialization;

namespace ETL.Domain.Sources;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RestApiSourceInfo), "restapi")]
[JsonDerivedType(typeof(ExcelSourceInfo), "excel")]
[JsonDerivedType(typeof(MySQLSourceInfo), "mysql")]
[JsonDerivedType(typeof(MsSqlSourceInfo), "mssql")]
public abstract class SourceInfoBase
{
}
