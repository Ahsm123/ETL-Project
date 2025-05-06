using ETL.Domain.Model;
using ETL.Domain.Sources;
using ExtractAPI.DataSources;
using System.Text.Json;

namespace Test.ExtractAPITest.DataSourceProviderTest
{
    public class ExcelDataSourceProviderTests
    {
        private readonly ExcelDataSourceProvider _excelDataSourceProvider;

        public ExcelDataSourceProviderTests()
        {
            _excelDataSourceProvider = new ExcelDataSourceProvider();
        }

        [Fact]
        public void CanHandle_WhenExcelSourceInfo_ShouldReturnTrue()
        {
            // Arrange
            var sourceInfoType = typeof(ExcelSourceInfo);

            // Act
            var result = _excelDataSourceProvider.CanHandle(sourceInfoType);

            // Assert 
            Assert.True(result);
        }

        [Fact]
        public void CanHandle_ForNonExcelSourceInfo_ShouldReturnFalse()
        {
            // Arrange
            var sourceInfoType = typeof(MsSqlSourceInfo);

            // Act
            var result = _excelDataSourceProvider.CanHandle(sourceInfoType);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetDataAsync_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var extractConfig = new ExtractConfig
            {
                SourceInfo = new ExcelSourceInfo
                {
                    FilePath = "invalid-path.xlsx", // Invalid path
                    SheetName = "Sheet1"
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() => _excelDataSourceProvider.GetDataAsync(extractConfig));
            Assert.Equal("Excel file not found", exception.Message);
        }

        [Fact]
        public async Task GetDataAsync_WhenNoDataRowsExist_ShouldReturnEmptyJson()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "ExcelDataSourceProviderTests");
            Directory.CreateDirectory(tempDirectory);
            var filePath = Path.Combine(tempDirectory, "test.xlsx");

            var extractConfig = new ExtractConfig
            {
                SourceInfo = new ExcelSourceInfo
                {
                    FilePath = filePath,
                    SheetName = "Sheet1"
                }
            };

            var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.AddWorksheet("Sheet1");
            worksheet.Cell("A1").Value = "Test";
            worksheet.Cell("B1").Value = "Data";
            workbook.SaveAs(filePath);

            // Act
            var result = await _excelDataSourceProvider.GetDataAsync(extractConfig);

            // Assert
            Assert.IsType<JsonElement>(result);
            Assert.Equal("[]", result.GetRawText());

            File.Delete(filePath);
            await Task.Delay(1000);
            Directory.Delete(tempDirectory);
        }


        [Fact]
        public async Task GetDataAsync_WhenHeadersAreEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), "ExcelDataSourceProviderTests");
            Directory.CreateDirectory(tempDirectory);
            var filePath = Path.Combine(tempDirectory, "test_invalid_headers.xlsx");

            var extractConfig = new ExtractConfig
            {
                SourceInfo = new ExcelSourceInfo
                {
                    FilePath = filePath,
                    SheetName = "Sheet1"
                }
            };

            var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.AddWorksheet("Sheet1");
            worksheet.Cell("A1").Value = "";
            worksheet.Cell("B1").Value = "";
            workbook.SaveAs(filePath);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _excelDataSourceProvider.GetDataAsync(extractConfig));
            Assert.Equal("No headers found in the Excel sheet.", exception.Message);

            // Clean up
            File.Delete(filePath);
            Directory.Delete(tempDirectory);
        }
    }
}
