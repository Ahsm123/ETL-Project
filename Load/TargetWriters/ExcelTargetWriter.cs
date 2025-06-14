﻿using ClosedXML.Excel;
using ETL.Domain.NewFolder;
using ETL.Domain.Targets.FileTargets;
using Load.TargetWriters.Interfaces;
using System.Diagnostics;

public class ExcelTargetWriter : ITargetWriter
{
    public bool CanHandle(Type targetInfoType)
        => targetInfoType == typeof(ExcelTargetInfo);

    public async Task WriteAsync(LoadContext context)
    {
        if (context.TargetInfo is not ExcelTargetInfo excelInfo)
            throw new ArgumentException("Invalid targetInfo for Excel target");

        var pipelineId = context.PipelineId;
        var data = context.Data;

        try
        {
            string fullPath = GenerateFilePath(excelInfo, pipelineId);
            EnsureDirectoryExists(fullPath);

            var workbook = LoadOrCreateWorkbook(fullPath);
            var worksheet = GetOrCreateWorksheet(workbook, excelInfo);

            bool isNew = IsNewSheet(worksheet);

            if (isNew && excelInfo.IncludeHeaders)
            {
                WriteHeaders(worksheet, data.Keys);
            }

            WriteRow(worksheet, data, excelInfo.IncludeHeaders);
            workbook.Save(); // <- Save changes without overwriting the file
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"[IO ERROR] Failed to write Excel file: {ex.Message}");
            throw new ApplicationException("An error occurred while writing the Excel file.", ex);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[GENERAL ERROR] {ex.Message}");
            throw;
        }

        await Task.CompletedTask;
    }

    private static XLWorkbook LoadOrCreateWorkbook(string fullPath)
    {
        try
        {
            return File.Exists(fullPath)
                ? new XLWorkbook(fullPath)
                : new XLWorkbook();
        }
        catch (Exception ex)
        {
            throw new IOException($"Unable to load or create Excel workbook at path: {fullPath}", ex);
        }
    }

    private static IXLWorksheet GetOrCreateWorksheet(XLWorkbook workbook, ExcelTargetInfo info)
    {
        var sheetName = info.SheetName ?? "Sheet1";

        try
        {
            return workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName)
                ?? workbook.Worksheets.Add(sheetName);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Unable to access or create worksheet: {sheetName}", ex);
        }
    }

    private static string GenerateFilePath(ExcelTargetInfo info, string? pipelineId)
    {
        var currentDate = DateTime.Now.ToString("yyyyMMdd");
        var basePath = Path.GetDirectoryName(info.FilePath) ?? "Exports";
        var fileBaseName = Path.GetFileNameWithoutExtension(info.FilePath);
        var finalName = $"{(pipelineId ?? fileBaseName)}_{currentDate}.xlsx";

        // Resolve to absolute path
        var rootPath = Path.GetFullPath(basePath);
        return Path.Combine(rootPath, finalName);
    }


    private static void EnsureDirectoryExists(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                throw new IOException($"Could not create directory: {directory}", ex);
            }
        }
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
        int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

        // Start on next row
        int row = lastRow + 1;

        // If headers are included and we're writing to row 1, skip it
        if (headersIncluded && row == 1)
            row = 2;

        int col = 1;
        foreach (var key in data.Keys)
        {
            worksheet.Cell(row, col++).Value = data[key]?.ToString();
        }
    }


}
