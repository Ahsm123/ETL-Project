using ETL.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transform.Strategy;

namespace Transform.Services
{
    public class TransformService : ITransformService
    {
        public Task TransformDataAsync(string configId)
        {
            throw new NotImplementedException();
        }




        //private ITransformationStrategy<Dictionary<string, object>> GetTransformationStrategy(TransformSettings transformSettings)
        //{
        //    switch (transformSettings.StrategyType)
        //    {
        //        case "Filter":
        //            // Create and return FilterStrategy based on filter conditions
        //            return new FilterStrategy(transformSettings.FilterConditions);

        //        case "Map":
        //            // Create and return MapStrategy based on map conditions
        //            return new MapStrategy(transformSettings.MapConditions);

        //        default:
        //            throw new InvalidOperationException($"Unknown strategy type: {transformSettings.StrategyType}");
        //    }
        //}
    }
}
