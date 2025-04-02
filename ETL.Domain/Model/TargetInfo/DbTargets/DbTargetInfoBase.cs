using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.TargetInfo.DbTargets;

public abstract class DbTargetInfoBase : TargetInfoBase
{
    public string ConnectionString { get; set; }
    public string TableName { get; set; }
}
