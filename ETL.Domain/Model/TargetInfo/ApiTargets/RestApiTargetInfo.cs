using ETL.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.TargetInfo.ApiTargets;

[TargetType("restapi")]
public class RestApiTargetInfo
{
    public string Method { get; set; } = "POST";

    public RestApiTargetInfo()
    {
    }
}
