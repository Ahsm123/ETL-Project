using ETL.Domain.NewFolder;
using ETL.Domain.Targets;
using ETL.Domain.Targets.FileTargets;
using OfficeOpenXml;

namespace Test.LoadTest.TargetWriterUnitTests
{
    public class ExcelTargetWriterUnitTests : IDisposable
    {
        private readonly string _testDirectory;

        public ExcelTargetWriterUnitTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"ExcelTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }


        [Fact]
        public async Task WriteAsync_WhenTargetInfoIsInvalid_ThenThrowsArgumentException()
        {
            var context = new LoadContext
            {
                TargetInfo = new FakeTargetInfo(),
                Data = new Dictionary<string, object> { ["A"] = 1 }
            };

            var writer = new ExcelTargetWriter();

            await Assert.ThrowsAsync<ArgumentException>(() => writer.WriteAsync(context));
        }
        [Fact]
        public async Task WriteAsync_WhenFileExists_ThenAppendsNewRowsBelowExistingData()
        {
            // Arrange
            var baseFilePath = Path.Combine(_testDirectory, "test.xlsx");
            var pipelineId = "fixed_test";
            var writer = new ExcelTargetWriter();

            var context1 = new LoadContext
            {
                TargetInfo = new ExcelTargetInfo
                {
                    FilePath = baseFilePath,
                    SheetName = "Sheet1",
                    IncludeHeaders = true
                },
                PipelineId = pipelineId,
                Data = new Dictionary<string, object> { ["Name"] = "Alice", ["Age"] = 30 }
            };

            var context2 = new LoadContext
            {
                TargetInfo = new ExcelTargetInfo
                {
                    FilePath = baseFilePath,
                    SheetName = "Sheet1",
                    IncludeHeaders = true
                },
                PipelineId = pipelineId,
                Data = new Dictionary<string, object> { ["Name"] = "Bob", ["Age"] = 25 }
            };

            // Act
            await writer.WriteAsync(context1);
            await writer.WriteAsync(context2);

            var expectedFile = Path.Combine(_testDirectory, $"{pipelineId}_{DateTime.Now:yyyyMMdd}.xlsx");

            // Assert
            using var workbook = new ClosedXML.Excel.XLWorkbook(expectedFile);
            var sheet = workbook.Worksheet("Sheet1");

            Assert.Equal("Alice", sheet.Cell(2, 1).GetString());
            Assert.Equal("30", sheet.Cell(2, 2).GetString());
            Assert.Equal("Bob", sheet.Cell(3, 1).GetString());
            Assert.Equal("25", sheet.Cell(3, 2).GetString());
        }



        public void Dispose()
        {
            try { Directory.Delete(_testDirectory, true); } catch { }
        }

        private class FakeTargetInfo : TargetInfoBase { }
    }
}
