using ETL.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Transform.Strategy
{
    public class MappingStrategy : ITransformationStrategy<List<Dictionary<string, object>>>
    {
        private readonly List<FieldMapping> _mappings;

        public MappingStrategy(List<FieldMapping> mappings)
        {
            _mappings = mappings;
        }

        public List<Dictionary<string, object>> Transform(IEnumerable<Dictionary<string, object>> data)
        {
            return data
             .Select(row =>
             {
                 var transformedRow = new Dictionary<string, object>();

                 foreach (var mapping in _mappings)
                 {
                     if (row.ContainsKey(mapping.SourceField))
                     {
                         transformedRow[mapping.TargetField] = row[mapping.SourceField];
                     }
                 }

                 return transformedRow;
             })
             .ToList();
        }


    }

}
