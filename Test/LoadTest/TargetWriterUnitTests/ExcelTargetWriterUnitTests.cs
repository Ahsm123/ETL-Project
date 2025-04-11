using ClosedXML.Excel;
using ETL.Domain.Targets.FileTargets;
using ETL.Domain.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.LoadTest.TargetWriterUnitTests;

public class ExcelTargetWriterUnitTests : IDisposable
{
    private readonly string _testDirectory;

    public ExcelTargetWriterUnitTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"ExcelTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task WriteAsync_WhenIncludeHeadersIsTrueAndDataIsValid_ThenFileIsCreatedWithHeadersAndValues()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "output.xlsx");
        var targetInfo = new ExcelTargetInfo
        {
            FilePath = filePath,
            SheetName = "Data",
            IncludeHeaders = true
        };

        var data = new Dictionary<string, object>
        {
            ["Name"] = "TestDude",
            ["Age"] = 30
        };

        var writer = new ExcelTargetWriter();

        // Act
        await writer.WriteAsync(targetInfo, data);

        // Assert
        Assert.True(File.Exists(GetExpectedFilePath(filePath)));

        using var workbook = new XLWorkbook(GetExpectedFilePath(filePath));
        var worksheet = workbook.Worksheet("Data");

        Assert.Equal("Name", worksheet.Cell(1, 1).GetValue<string>());
        Assert.Equal("Age", worksheet.Cell(1, 2).GetValue<string>());
        Assert.Equal("TestDude", worksheet.Cell(2, 1).GetValue<string>());
        Assert.Equal("30", worksheet.Cell(2, 2).GetValue<string>());
    }

    [Fact]
    public async Task WriteAsync_WhenFileExists_ThenAppendsNewRowsBelowExistingData()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "append_test.xlsx");
        var targetInfo = new ExcelTargetInfo
        {
            FilePath = filePath,
            SheetName = "Sheet1",
            IncludeHeaders = true
        };

        var writer = new ExcelTargetWriter();

        var data1 = new Dictionary<string, object> { ["Item"] = "RedBull", ["Qty"] = 5 };
        var data2 = new Dictionary<string, object> { ["Item"] = "Booster", ["Qty"] = 10 };

        // Act
        await writer.WriteAsync(targetInfo, data1);
        await writer.WriteAsync(targetInfo, data2);

        // Assert
        using var workbook = new XLWorkbook(GetExpectedFilePath(filePath));
        var ws = workbook.Worksheet("Sheet1");

        Assert.Equal("RedBull", ws.Cell(2, 1).GetValue<string>());
        Assert.Equal("5", ws.Cell(2, 2).GetValue<string>());
        Assert.Equal("Booster", ws.Cell(3, 1).GetValue<string>());
        Assert.Equal("10", ws.Cell(3, 2).GetValue<string>());
    }

    [Fact]
    public async Task WriteAsync_WhenTargetInfoIsInvalid_ThenThrowsArgumentException()
    {
        // Arrange
        var invalidTarget = new FakeTargetInfo(); 
        var writer = new ExcelTargetWriter();

        var data = new Dictionary<string, object> { ["A"] = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            writer.WriteAsync(invalidTarget, data));
    }

    private string GetExpectedFilePath(string original)
    {
        var baseName = Path.GetFileNameWithoutExtension(original);
        var dir = Path.GetDirectoryName(original)!;
        var date = DateTime.Now.ToString("yyyyMMdd");
        return Path.Combine(dir, $"{baseName}_{date}.xlsx");
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_testDirectory, true);
        }
        catch {  }
    }

    private class FakeTargetInfo : TargetInfoBase { }
}
