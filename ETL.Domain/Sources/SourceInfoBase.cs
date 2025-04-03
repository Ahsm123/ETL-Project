using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.Sources;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RestApiSourceInfo), "api")]
[JsonDerivedType(typeof(ExcelSourceInfo), "excel")]
[JsonDerivedType(typeof(DbSourceBaseInfo), "db")]
public abstract class SourceInfoBase
{
}
