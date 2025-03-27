using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Strategy
{
    public interface ITransformationStrategy<T>
    {
        T Transform(IEnumerable<Dictionary<string, object>> data);
    }
}
