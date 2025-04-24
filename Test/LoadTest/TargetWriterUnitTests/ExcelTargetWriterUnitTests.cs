using ClosedXML.Excel;
using ETL.Domain.Targets.FileTargets;
using ETL.Domain.Targets;
using Load;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using ETL.Domain.NewFolder;

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
        public async Task WriteAsync_WhenIncludeHeadersIsTrueAndDataIsValid_ThenFileIsCreatedWithHeadersAndValues()
        {
            var filePath = Path.Combine(_testDirectory, "output.xlsx");
            var targetInfo = new ExcelTargetInfo
            {
                FilePath = filePath,
                SheetName = "Data",
                IncludeHeaders = true
            };

            var context = new LoadContext
            {
                TargetInfo = targetInfo,
                Data = new Dictionary<string, object>
                {
                    ["Name"] = "TestDude",
                    ["Age"] = 30
                }
            };

            var writer = new ExcelTargetWriter();

            await writer.WriteAsync(context);

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
            var filePath = Path.Combine(_testDirectory, "append_test.xlsx");
            var targetInfo = new ExcelTargetInfo
            {
                FilePath = filePath,
                SheetName = "Sheet1",
                IncludeHeaders = true
            };

            var writer = new ExcelTargetWriter();

            var context1 = new LoadContext
            {
                TargetInfo = targetInfo,
                Data = new Dictionary<string, object> { ["Item"] = "RedBull", ["Qty"] = 5 }
            };

            var context2 = new LoadContext
            {
                TargetInfo = targetInfo,
                Data = new Dictionary<string, object> { ["Item"] = "Booster", ["Qty"] = 10 }
            };

            await writer.WriteAsync(context1);
            await writer.WriteAsync(context2);

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
            var context = new LoadContext
            {
                TargetInfo = new FakeTargetInfo(),
                Data = new Dictionary<string, object> { ["A"] = 1 }
            };

            var writer = new ExcelTargetWriter();

            await Assert.ThrowsAsync<ArgumentException>(() => writer.WriteAsync(context));
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
            try { Directory.Delete(_testDirectory, true); } catch { }
        }

        private class FakeTargetInfo : TargetInfoBase { }
    }
}
