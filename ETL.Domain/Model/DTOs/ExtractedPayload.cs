using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETL.Domain.Model.DTOs;

public class ExtractedPayload
{
    public string Id { get; set; }
    public string SourceType { get; set; }
    public TransformSettings Transform { get; set; }
    public LoadSettings Load { get; set; }

    public Dictionary<string, object> Data { get; set; }
}
