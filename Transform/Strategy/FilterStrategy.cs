using ETL.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Strategy
{
    public class FilterStrategy : ITransformationStrategy<List<Dictionary<string, object>>>
    {
        private readonly List<FilterCondition> _filters;
        private readonly List<string> _selectedFields;

        public FilterStrategy(List<FilterCondition> filters, List<string> selectedFields)
        {
            _filters = filters;
            _selectedFields = selectedFields;
        }

        public List<Dictionary<string, object>> Transform(IEnumerable<Dictionary<string, object>> data)
        {
            return data
             .Select(row => row
                 .Where(kvp => _selectedFields.Contains(kvp.Key))
                 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
             .ToList();
        }
    }
}
