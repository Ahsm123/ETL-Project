using ETL.Domain.Model;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using ExtractAPI.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using ETL.Domain.Rules;
using Dapper;

namespace Test.ExtractAPI;

public class MySQLDataSourceProviderTests
{
    [Fact]
    public async Task GetDataAsync_Returns_ValidJson()
    {
        // Arrange
        var mockBuilder = new Mock<IMySqlQueryBuilder>();
        var mockExecutor = new Mock<IMySqlExecutor>();

        var expectedRows = new List<IDictionary<string, object>>
    {
        new Dictionary<string, object> { { "Id", 1 }, { "Name", "Test" } }
    };

        mockBuilder.Setup(b => b.GenerateSelectQuery(
             It.IsAny<DbSourceBaseInfo>(),
            It.IsAny<List<string>>(),
             It.IsAny<List<FilterRule>>()))
            .Returns(("SELECT * FROM Users", new DynamicParameters()));


        mockExecutor.Setup(e => e.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(expectedRows);

        var provider = new MySQLDataSourceProvider(mockBuilder.Object, mockExecutor.Object);

        var config = new ExtractConfig
        {
            SourceInfo = new MySQLSourceInfo
            {
                ConnectionString = "fake_conn",
                TargetTable = "Users"
            },
            Fields = new List<string> { "Id", "Name" },
            Filters = new()
        };

        // Act
        var result = await provider.GetDataAsync(config);

        // Assert
        Assert.Equal(JsonValueKind.Array, result.ValueKind);
        Assert.Equal("Test", result[0].GetProperty("Name").GetString());
    }
}
