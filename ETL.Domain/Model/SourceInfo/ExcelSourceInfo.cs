using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.SourceInfo;

public class ExcelSourceInfo : FileSourceBaseInfo
{
    public string SheetName { get; set; } = "TestSheet";
}
