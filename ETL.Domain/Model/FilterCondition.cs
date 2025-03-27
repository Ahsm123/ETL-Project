using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model;

public class FilterCondition
{
    public string Field { get; set; }
    public string Operator { get; set; }
    public string Value { get; set; }
}
