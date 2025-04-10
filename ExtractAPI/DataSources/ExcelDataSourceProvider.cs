using ClosedXML.Excel;
using ETL.Domain.Model;
using ETL.Domain.Sources;
using ExtractAPI.Interfaces;
using System.Text.Json;

namespace ExtractAPI.DataSources;

// File: ExcelDataSourceProvider.cs

public class ExcelDataSourceProvider : IDataSourceProvider
{
    public bool CanHandle(Type sourceInfoType)
        => sourceInfoType == typeof(ExcelSourceInfo);

    public async Task<JsonElement> GetDataAsync(ExtractConfig extractConfig)
    {
        var excelInfo = ValidateSourceInfo(extractConfig);
        var worksheet = LoadWorksheet(excelInfo.FilePath, excelInfo.SheetName);

        var headers = ExtractHeaders(worksheet);
        var rows = ExtractRows(worksheet, headers);

        return ConvertToJson(rows);
    }

    private ExcelSourceInfo ValidateSourceInfo(ExtractConfig config)
    {
        if (config.SourceInfo is not ExcelSourceInfo excelInfo)
            throw new ArgumentException("Invalid sourceInfo for excel source");

        if (!File.Exists(excelInfo.FilePath))
            throw new FileNotFoundException("Excel file not found", excelInfo.FilePath);

        return excelInfo;
    }

    private IXLWorksheet LoadWorksheet(string filePath, string? sheetName)
    {
        var workbook = new XLWorkbook(filePath);
        return workbook.Worksheet(sheetName ?? "Sheet1");
    }

    private List<string> ExtractHeaders(IXLWorksheet worksheet)
        => worksheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();

    private List<Dictionary<string, object>> ExtractRows(IXLWorksheet worksheet, List<string> headers)
    {
        return worksheet.RowsUsed()
            .Skip(1)
            .Select(row => headers
                .Select((header, i) => new { header, value = row.Cell(i + 1).GetValue<string>() })
                .ToDictionary(x => x.header, x => (object)x.value))
            .ToList();
    }

    private JsonElement ConvertToJson(List<Dictionary<string, object>> rows)
    {
        var json = JsonSerializer.Serialize(rows);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}

