using ClosedXML.Excel;
using ETL.Domain.Attributes;
using ETL.Domain.Model.SourceInfo;
using System.ComponentModel;
using System.Text.Json;

namespace ExtractAPI.DataSources;

[SourceProviderType("excel")]
public class ExcelDataSourceProvider : IDataSourceProvider
{
    public async Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo)
    {
        if (sourceInfo is not ExcelSourceInfo excelInfo)
            throw new ArgumentException("Invalid sourceInfo for excel source");

        if (!File.Exists(excelInfo.FilePath))
            throw new FileNotFoundException("Excel file not found", excelInfo.FilePath);

        using var workbook = new XLWorkbook(excelInfo.FilePath);
        var worksheet = workbook.Worksheet(excelInfo.SheetName ?? "Sheet1");

        var headers = worksheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();
        var rows = new List<Dictionary<string, object>>();

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var rowDict = new Dictionary<string, object>();
            for (int col = 1; col <= headers.Count; col++)
            {
                rowDict[headers[col - 1]] = row.Cell(col).GetValue<string>();
            }
            rows.Add(rowDict);
        }

        var json = JsonSerializer.Serialize(rows);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone(); 
    }

}
