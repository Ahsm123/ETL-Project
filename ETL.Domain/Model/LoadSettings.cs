using ETL.Domain.Model.TargetInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model;

public class LoadSettings
{
    public string TargetType { get; set; }
    public TargetInfoBase TargetInfo { get; set; }

}
