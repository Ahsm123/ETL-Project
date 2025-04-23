using ClosedXML.Excel;
using ETL.Domain.Model;
using ETL.Domain.Sources;
using ExtractAPI.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace ExtractAPI.DataSources
{
    public class ExcelDataSourceProvider : IDataSourceProvider
    {
        public bool CanHandle(Type sourceInfoType)
            => sourceInfoType == typeof(ExcelSourceInfo);

        public async Task<JsonElement> GetDataAsync(ExtractConfig extractConfig)
        {
            var excelInfo = ValidateSourceInfo(extractConfig);
            var worksheet = LoadWorksheet(excelInfo.FilePath, excelInfo.SheetName);

            var headers = ExtractHeaders(worksheet);
            if (!headers.Any())
                throw new InvalidOperationException("No headers found in the Excel sheet.");

            var rows = ExtractRows(worksheet, headers);

            return ConvertToJson(rows);
        }

        private ExcelSourceInfo ValidateSourceInfo(ExtractConfig config)
        {
            if (config.SourceInfo is not ExcelSourceInfo excelInfo)
                throw new ArgumentException("Invalid sourceInfo for excel source");

            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), excelInfo.FilePath);

            if (!File.Exists(fullFilePath))
                throw new FileNotFoundException("Excel file not found", fullFilePath);

            return new ExcelSourceInfo
            {
                FilePath = fullFilePath,
                SheetName = excelInfo.SheetName
            };
        }

        private IXLWorksheet LoadWorksheet(string filePath, string? sheetName)
        {
            try
            {
                var workbook = new XLWorkbook(filePath);
                return workbook.Worksheet(sheetName ?? "Sheet1");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load worksheet '{sheetName}' from file '{filePath}'.", ex);
            }
        }

        private List<string> ExtractHeaders(IXLWorksheet worksheet)
        {
            return worksheet.Row(1).Cells().Select(c => c.GetString()?.Trim()).ToList();
        }

        private List<Dictionary<string, object>> ExtractRows(IXLWorksheet worksheet, List<string> headers)
        {
            var rows = worksheet.RowsUsed()
                .Skip(1)
                .Select(row => headers
                    .Select((header, i) => new { header, value = row.Cell(i + 1).GetValue<string>() })
                    .ToDictionary(x => x.header, x => (object)x.value))
                .ToList();

            return rows;
        }

        private JsonElement ConvertToJson(List<Dictionary<string, object>> rows)
        {
            var json = JsonSerializer.Serialize(rows);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
    }
}
