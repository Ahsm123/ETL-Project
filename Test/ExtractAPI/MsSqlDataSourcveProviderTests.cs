using Dapper;
using ETL.Domain.Model;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using Moq;
using System.Text.Json;

namespace Test.ExtractAPI;

public class MsSqlDataSourceProviderTests
{
    [Fact]
    public async Task GetDataAsync_Returns_ValidJson()
    {
        // Arrange
        var mockExecutor = new Mock<IMsSqlExecutor>();
        var mockQueryBuilder = new Mock<IMsSqlQueryBuilder>();

        var expectedData = new List<IDictionary<string, object>>
    {
        new Dictionary<string, object> { { "Id", 1 }, { "Name", "Test User" } }
    };

        mockQueryBuilder.Setup(b => b.GenerateSelectQuery(
        It.IsAny<DbSourceBaseInfo>(),
        It.IsAny<List<string>>(),
        It.IsAny<List<FilterRule>>()))
        .Returns(("SELECT * FROM Users", new DynamicParameters()));


        mockExecutor
            .Setup(e => e.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(expectedData);

        var provider = new MsSqlDataSourceProvider(mockExecutor.Object, mockQueryBuilder.Object);

        var config = new ExtractConfig
        {
            SourceInfo = new MsSqlSourceInfo
            {
                ConnectionString = "Server=test;Database=db;",
                TargetTable = "Users"
            },
            Fields = new List<string> { "Id", "Name" },
            Filters = new List<FilterRule>()
        };

        // Act
        var result = await provider.GetDataAsync(config);

        // Assert
        Assert.Equal(JsonValueKind.Array, result.ValueKind);
        Assert.Equal("Test User", result[0].GetProperty("Name").GetString());
    }

}