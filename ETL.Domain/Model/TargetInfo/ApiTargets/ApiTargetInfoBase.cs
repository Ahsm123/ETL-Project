using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.TargetInfo.ApiTargets;

public class ApiTargetInfoBase : TargetInfoBase
{
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get; set; }

    protected ApiTargetInfoBase()
    {
    }
}
