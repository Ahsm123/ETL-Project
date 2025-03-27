using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Strategy
{
    public class TransformationContext<T>
    {
        public ITransformationStrategy<T> _strategy;

        public TransformationContext(ITransformationStrategy<T> strategy)
        {
            _strategy = strategy;
        }
        public void SetStrategy(ITransformationStrategy<T> strategy)
        {
            _strategy = strategy;
        }

        public T Transform(IEnumerable<Dictionary<string, object>> data)
        {
            return _strategy.Transform(data);
        }
    }
} 
