using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Targets.FileTargets;

public class ExcelTargetInfo : FileTargetInfoBase
{
    public string SheetName { get; set; } = "Sheet1";
    public bool IncludeHeaders { get; set; } = true;
}