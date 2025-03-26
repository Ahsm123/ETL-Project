using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.SourceInfo;

public class FileSourceBaseInfo : SourceInfoBase
{
    public string FilePath { get; set; }
    public string FileType { get; set; }
}
