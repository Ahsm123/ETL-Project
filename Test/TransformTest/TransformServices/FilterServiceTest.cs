using ETL.Domain.Events;
using ETL.Domain.Rules;
using Transform.Services;

namespace Test.TransformTest.TransformServices;

public class FilterServiceTest
{
    [Fact]
    public void ShouldInclude_ReturnsTrue_WhenAllFiltersPass()
    {
        var filterservice = new FilterService();
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", 1500 },
            { "status", "Accepted" }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "greaterthan", "1000"),
            new("status", "equals", "Accepted")
        };

        var result = filterservice.ShouldInclude(item, filters);

        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_ReturnsFalse_WhenFilterFails()
    {
        var filterservice = new FilterService();
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", 1500 }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "greaterthan", "2000")
        };

        var result = filterservice.ShouldInclude(item, filters);

        Assert.False(result);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, false)]
    [InlineData(1001, true)]
    [InlineData(2000, true)]
    public void ShouldInclude_GreaterThan_BoundaryValueTesting(int cost, bool expectedResult)
    {
        var filterService = new FilterService();
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", cost }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "greaterthan", "1000")
        };

        var result = filterService.ShouldInclude(item, filters);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(500, true)]
    [InlineData(999, true)]
    [InlineData(1000, false)]
    [InlineData(1001, false)]
    public void ShouldInclude_LessThan_BoundaryValueTesting(int cost, bool expectedResult)
    {
        var filterService = new FilterService();
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", cost }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "lessthan", "1000")
        };

        var result = filterService.ShouldInclude(item, filters);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, true)]
    [InlineData(1001, false)]
    public void ShouldInclude_Equals_BoundaryValueTesting(int cost, bool expectedResult)
    {
        var filterService = new FilterService();
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", cost }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "equals", "1000")
        };

        var result = filterService.ShouldInclude(item, filters);

        Assert.Equal(expectedResult, result);
    }
}
