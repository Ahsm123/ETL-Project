using ClosedXML.Excel;
using ETL.Domain.Targets.FileTargets;
using ETL.Domain.Targets;
using Load.Interfaces;

public class ExcelTargetWriter : ITargetWriter
{
    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null)
    {
        if (targetInfo is not ExcelTargetInfo excelInfo)
            throw new ArgumentException("Invalid targetInfo for Excel target");

        string fullPath = GenerateFilePath(excelInfo, pipelineId);
        EnsureDirectoryExists(fullPath);

        using var workbook = File.Exists(fullPath)
            ? new XLWorkbook(fullPath)
            : new XLWorkbook();

        var sheetName = excelInfo.SheetName ?? "Sheet1";
        var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName)
                     ?? workbook.Worksheets.Add(sheetName);

        if (IsNewSheet(worksheet) && excelInfo.IncludeHeaders)
        {
            WriteHeaders(worksheet, data.Keys);
        }

        WriteRow(worksheet, data, excelInfo.IncludeHeaders);
        workbook.SaveAs(fullPath);

        await Task.CompletedTask;
    }

    public bool CanHandle(Type targetInfoType) => targetInfoType == typeof(ExcelTargetInfo);

    private static string GenerateFilePath(ExcelTargetInfo info, string? pipelineId)
    {
        var currentDate = DateTime.Now.ToString("yyyyMMdd");
        var basePath = Path.GetDirectoryName(info.FilePath) ?? "Exports";
        var fileBaseName = Path.GetFileNameWithoutExtension(info.FilePath);
        var finalName = $"{(pipelineId ?? fileBaseName)}_{currentDate}.xlsx";
        return Path.Combine(basePath, finalName);
    }

    private static void EnsureDirectoryExists(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
    }

    private static bool IsNewSheet(IXLWorksheet worksheet)
        => worksheet.LastRowUsed() == null;

    private static void WriteHeaders(IXLWorksheet worksheet, IEnumerable<string> keys)
    {
        int col = 1;
        foreach (var key in keys)
        {
            worksheet.Cell(1, col++).Value = key;
        }
    }

    private static void WriteRow(IXLWorksheet worksheet, Dictionary<string, object> data, bool headersIncluded)
    {
        int row = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;
        if (row == 1 && headersIncluded)
            row = 2;

        int col = 1;
        foreach (var key in data.Keys)
        {
            worksheet.Cell(row, col++).Value = data[key]?.ToString();
        }
    }
}
