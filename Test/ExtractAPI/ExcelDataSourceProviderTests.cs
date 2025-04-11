using ETL.Domain.Model;
using ETL.Domain.Sources;
using ExtractAPI.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ExtractAPI;

public class ExcelDataSourceProviderTests
{
    private readonly ExcelDataSourceProvider _excelDataSourceProvider;

    public ExcelDataSourceProviderTests()
    {
        _excelDataSourceProvider = new ExcelDataSourceProvider();
    }

    [Fact]
    public void CanHandle_ShouldReturnTrue_ForExcelSourceInfo()
    {
        // Arrange
        var sourceInfoType = typeof(ExcelSourceInfo);

        // Act
        var result = _excelDataSourceProvider.CanHandle(sourceInfoType);

        // Assert 
        Assert.True(result);
    }

    [Fact]
    public void CanHandle_ShouldReturnFalse_ForNonExcelSourceInfo()
    {
        // Arrange
        var sourceInfoType = typeof(MsSqlSourceInfo);

        // Act
        var result = _excelDataSourceProvider.CanHandle(sourceInfoType);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetDataAsync_ShouldThrowArgumentException_WhenSourceInfoIsInvalid()
    {
        // Arrange
        var extractConfig = new ExtractConfig
        {
            SourceInfo = new MsSqlSourceInfo()
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _excelDataSourceProvider.GetDataAsync(extractConfig));
        Assert.Equal("Invalid sourceInfo for excel source", exception.Message);

    }
}
