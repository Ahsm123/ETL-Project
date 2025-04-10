using ETL.Domain.Model;
using ETL.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Transform.Services;

namespace Test.TransformTest.TransformServices;
public class FilterServiceTest
{
    [Fact]
    public void ShouldInclude_ReturnsTrue_WhenAllFiltersPass()
    {
        //Arrange
        var filterservice = new FilterService();
        var item = new Dictionary<string, object>
        {
            { "cost", JsonDocument.Parse("1500").RootElement },
            { "status", JsonDocument.Parse("\"Accepted\"").RootElement }
        };
        var filters = new List<FilterRule>
        {
            new FilterRule
            {
                Field = "cost",
                Operator = "greaterthan",
                Value = "1000"
            },
            new FilterRule
            {
                Field = "status",
                Operator = "equals",
                Value = "Accepted"
            }
        };
        //Act
        var result = filterservice.ShouldInclude(item, filters);
        //Assert
        Assert.True(result);

    }
    [Fact]
    public void ShouldInclude_ReturnsFalse_WhenFilterFails()
    {
        //Arrange
        var filterservice = new FilterService();
        var item = new Dictionary<string, object>
        {
            { "cost", JsonDocument.Parse("1500").RootElement },
        };
        var filters = new List<FilterRule>
        {
            new FilterRule
            {
                Field = "cost",
                Operator = "greaterthan",
                Value = "2000"
            },

        };
        //Act
        var result = filterservice.ShouldInclude(item, filters);
        //Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, false)]
    [InlineData(1001, true)]
    [InlineData(2000, true)]
    public void ShouldInclude_GreaterThan_BoundaryValueTesting(int cost, bool expectedResult)
    {
        // Arrange
        var filterService = new FilterService();

        var item = new Dictionary<string, object>
    {
        { "cost", JsonDocument.Parse(cost.ToString()).RootElement } // simulate JSON number
    };

        var filters = new List<FilterRule>
    {
        new FilterRule
        {
            Field = "cost",
            Operator = "greaterthan",
            Value = "1000"
        }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.Equal(expectedResult, result);
    }
    [Theory]
    [InlineData(500, true)]
    [InlineData(999, true)]
    [InlineData(1000, false)]
    [InlineData(1001, false)]
    public void ShouldInclude_LessThan_BoundaryValueTesting(int cost, bool expectedResult)
    {
        // Arrange
        var filterService = new FilterService();

        var item = new Dictionary<string, object>
    {
        { "cost", JsonDocument.Parse(cost.ToString()).RootElement } // simulate JSON number
    };

        var filters = new List<FilterRule>
    {
        new FilterRule
        {
            Field = "cost",
            Operator = "lessthan",
            Value = "1000"
        }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, true)]
    [InlineData(1001, false)]
    [InlineData(2000, false)]
    public void ShouldInclude_Equals_BoundaryValueTesting(int cost, bool expectedResult)
    {
        // Arrange
        var filterService = new FilterService();

        var item = new Dictionary<string, object>
    {
        { "cost", JsonDocument.Parse(cost.ToString()).RootElement } // simulate JSON number
    };

        var filters = new List<FilterRule>
    {
        new FilterRule
        {
            Field = "cost",
            Operator = "equals",
            Value = "1000"
        }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.Equal(expectedResult, result);
    }


}
