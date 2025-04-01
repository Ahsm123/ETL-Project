using ETL.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.SourceInfo;

[SourceType("api")]
public class RestApiSourceInfo : SourceInfoBase
{
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get; set; }

}
