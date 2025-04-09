using ClosedXML.Excel;
using ETL.Domain.Targets.FileTargets;
using ETL.Domain.Targets;
using Load.Interfaces;

public class ExcelTargetWriter : ITargetWriter
{
    public Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null)
    {
        if (targetInfo is not ExcelTargetInfo excelInfo)
            throw new ArgumentException("Invalid targetInfo for Excel target");

        // Construct the filename with pipelineId and date
        var currentDate = DateTime.Now.ToString("yyyyMMdd");
        var basePath = Path.GetDirectoryName(excelInfo.FilePath) ?? "Exports";
        Directory.CreateDirectory(basePath); // ensure directory exists

        var fileBaseName = Path.GetFileNameWithoutExtension(excelInfo.FilePath);
        var fileName = $"{(pipelineId ?? fileBaseName)}_{currentDate}.xlsx";
        var fullPath = Path.Combine(basePath, fileName);

        XLWorkbook workbook;
        IXLWorksheet worksheet;

        if (File.Exists(fullPath))
        {
            workbook = new XLWorkbook(fullPath);
            worksheet = workbook.Worksheet(excelInfo.SheetName ?? "Sheet1");
        }
        else
        {
            workbook = new XLWorkbook();
            worksheet = workbook.Worksheets.Add(excelInfo.SheetName ?? "Sheet1");

            if (excelInfo.IncludeHeaders)
            {
                int headerCol = 1;
                foreach (var key in data.Keys)
                {
                    worksheet.Cell(1, headerCol++).Value = key;
                }
            }
        }

        // Find next empty row
        int row = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;
        if (row == 1 && excelInfo.IncludeHeaders)
            row = 2;

        int col = 1;
        foreach (var key in data.Keys)
        {
            worksheet.Cell(row, col++).Value = data[key]?.ToString();
        }

        workbook.SaveAs(fullPath);
        return Task.CompletedTask;
    }

    public bool CanHandle(Type targetInfoType)
    {
        return targetInfoType == typeof(ExcelTargetInfo);
    }
}
