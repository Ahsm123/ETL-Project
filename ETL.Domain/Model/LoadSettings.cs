using ETL.Domain.Model.TargetInfo;
using ETL.Domain.Model.TargetInfo.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ETL.Domain.Model;

public class LoadSettings
{
    public string TargetType { get; set; }

    [JsonConverter(typeof(TargetInfoConverter))]
    public TargetInfoBase TargetInfo { get; set; }

    //[JsonPropertyName("TargetInfo")]
    //public object TargetInfoJson => TargetInfo; 
}

