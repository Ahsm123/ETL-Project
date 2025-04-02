using ETL.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.TargetInfo.DbTargets;

[TargetType("mssql")]
public class MsSqlTargetInfo : DbTargetInfoBase
{
    public bool UseBulkInsert { get; set; }
}
