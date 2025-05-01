using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Targets.FileTargets;

public abstract class FileTargetInfoBase : TargetInfoBase
{
    public required string FilePath { get; set; } = string.Empty;
}
