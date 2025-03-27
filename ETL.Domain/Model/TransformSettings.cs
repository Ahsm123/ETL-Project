using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Model
{
    public class TransformSettings
    {
        public string StrategyType { get; set; }
        public List<FieldMapping> Mappings {  get; set; }
        public List<FilterCondition> Filters { get; set; }

    }
}
