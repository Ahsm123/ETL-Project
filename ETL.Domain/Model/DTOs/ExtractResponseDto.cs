using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model.DTOs;

public class ExtractResponseDto
{
    public string PipelineId { get; set; }
    public int MessagesSent { get; set; }
}
