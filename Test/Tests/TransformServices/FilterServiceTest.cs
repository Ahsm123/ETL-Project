using ETL.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Transform.Services;

namespace Test.Tests.TransformServices
{
    public class FilterServiceTest
    {
        [Fact]
        public void ShouldInclude_ReturnsTrue_When_CostIsGreaterThan1000()
        {
            // Arrange
            var filterService = new FilterService();

            var item = new Dictionary<string, object>
        {
            { "cost", JsonDocument.Parse("2000").RootElement } // simulate JSON number
        };

            var filters = new List<FilterCondition>
        {
            new FilterCondition
            {
                Field = "cost",
                Operator = "greaterthan",
                Value = "1000"
            }
        };

            // Act
            var result = filterService.ShouldInclude(item, filters);

            // Assert
            Assert.True(result);
        }

    }
}
