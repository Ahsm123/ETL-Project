using ETL.Domain.Events;
using ETL.Domain.Rules;
using Transform.Services;

namespace Test.TransformTest.TransformServices;

public class FilterServiceTest
{
    private readonly FilterService _filterService = new();

    [Fact]
    public void ShouldInclude_WhenAllFiltersPass_ReturnsTrue()
    {
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

        var result = _filterService.ShouldInclude(item, filters);

        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_WhenFilterFails_ReturnsFalse()
    {
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", 1500 }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "greaterthan", "2000")
        };

        var result = _filterService.ShouldInclude(item, filters);

        Assert.False(result);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, false)]
    [InlineData(1001, true)]
    [InlineData(2000, true)]
    public void ShouldInclude_WhenGreaterThan1000_OnlyReturnAbove1000(int cost, bool expectedResult)
    {
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", cost }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "greaterthan", "1000")
        };

        var result = _filterService.ShouldInclude(item, filters);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(500, true)]
    [InlineData(999, true)]
    [InlineData(1000, false)]
    [InlineData(1001, false)]
    public void ShouldInclude_WhenLessThan1000_OnlyReturnUnder1000(int cost, bool expectedResult)
    {
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", cost }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "lessthan", "1000")
        };

        var result = _filterService.ShouldInclude(item, filters);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(999, false)]
    [InlineData(1000, true)]
    [InlineData(1001, false)]
    public void ShouldInclude_WhenEquals1000_OnlyReturnCost1000(int cost, bool expectedResult)
    {
        var item = new RawRecord(new Dictionary<string, object>
        {
            { "cost", cost }
        });

        var filters = new List<FilterRule>
        {
            new("cost", "equals", "1000")
        };

        var result = _filterService.ShouldInclude(item, filters);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void ShouldInclude_WhenFieldIsMissing_ReturnsFalse()
    {
        var record = new RawRecord(new Dictionary<string, object>
    {
        { "other_field", 123 }
    });

        var filters = new List<FilterRule>
    {
        new("nonexistent", "equals", "123")
    };

        var result = _filterService.ShouldInclude(record, filters);

        Assert.False(result);
    }

    [Fact]
    public void ShouldInclude_WhenNoFiltersProvided_ReturnsTrue()
    {
        var record = new RawRecord(new Dictionary<string, object>
    {
        { "field", "value" }
    });

        var result = _filterService.ShouldInclude(record, new List<FilterRule>());

        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_Contains_MatchCaseInsensitive()
    {
        var record = new RawRecord(new Dictionary<string, object>
    {
        { "status", "Accepted" }
    });

        var filters = new List<FilterRule>
    {
        new("status", "contains", "accepted") // lower-case to test insensitivity
    };

        var result = _filterService.ShouldInclude(record, filters);

        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_NotEquals_ReturnsFalseOnMatch()
    {
        var record = new RawRecord(new Dictionary<string, object>
    {
        { "type", "Premium" }
    });

        var filters = new List<FilterRule>
    {
        new("type", "notequals", "Premium")
    };

        var result = _filterService.ShouldInclude(record, filters);

        Assert.False(result);
    }

    [Fact]
    public void ShouldInclude_WhenOneOfMultipleFiltersFails_ReturnsFalse()
    {
        var record = new RawRecord(new Dictionary<string, object>
    {
        { "cost", 1500 },
        { "status", "Rejected" }
    });

        var filters = new List<FilterRule>
    {
        new("cost", "greaterthan", "1000"),
        new("status", "equals", "Accepted") 
    };

        var result = _filterService.ShouldInclude(record, filters);

        Assert.False(result);
    }




}
