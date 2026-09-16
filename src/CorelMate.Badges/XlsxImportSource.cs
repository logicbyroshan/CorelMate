using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace CorelMate.Badges;

public static class XlsxImportSource
{
    private static readonly XNamespace Spreadsheet = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace Relationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    public static ImportedDataSet Read(string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        using (var archive = ZipFile.OpenRead(path))
        {
            var sharedStrings = ReadSharedStrings(archive);
            var workbook = ReadXml(archive, "xl/workbook.xml");
            var firstSheet = workbook.Descendants(Spreadsheet + "sheet").FirstOrDefault() ?? throw new FormatException("The XLSX workbook contains no worksheets.");
            var relationshipId = (string?)firstSheet.Attribute(Relationships + "id");
            var relationships = ReadXml(archive, "xl/_rels/workbook.xml.rels");
            var target = relationships.Descendants().FirstOrDefault(item => (string?)item.Attribute("Id") == relationshipId)?.Attribute("Target")?.Value;
            if (string.IsNullOrWhiteSpace(target)) throw new FormatException("The first XLSX worksheet could not be located.");
            var worksheetTarget = target!;
            var worksheetPath = worksheetTarget.StartsWith("/") ? worksheetTarget.TrimStart('/') : "xl/" + worksheetTarget.TrimStart('/');
            var worksheet = ReadXml(archive, worksheetPath);
            var records = worksheet.Descendants(Spreadsheet + "row").Select(row => ReadRow(row, sharedStrings)).ToList();
            while (records.Count > 0 && IsBlank(records[records.Count - 1])) records.RemoveAt(records.Count - 1);
            if (records.Count == 0 || IsBlank(records[0])) throw new FormatException("The first XLSX worksheet has no header row.");

            var headers = records[0];
            var rows = new List<IReadOnlyDictionary<string, string>>();
            for (var index = 1; index < records.Count; index++)
            {
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var column = 0; column < headers.Count; column++) row[headers[column]] = column < records[index].Count ? records[index][column] : string.Empty;
                rows.Add(row);
            }

            return new ImportedDataSet(Path.GetFileName(path), firstSheet.Attribute("name")?.Value ?? "Sheet1", headers, rows);
        }
    }

    private static List<string> ReadSharedStrings(ZipArchive archive)
    {
        var entry = archive.GetEntry("xl/sharedStrings.xml");
        if (entry == null) return new List<string>();
        var document = XDocument.Load(entry.Open());
        return document.Descendants(Spreadsheet + "si").Select(item => string.Concat(item.Descendants(Spreadsheet + "t").Select(text => text.Value))).ToList();
    }

    private static List<string> ReadRow(XElement row, IReadOnlyList<string> sharedStrings)
    {
        var values = new List<string>();
        foreach (var cell in row.Elements(Spreadsheet + "c"))
        {
            var reference = cell.Attribute("r")?.Value ?? string.Empty;
            var column = ColumnIndex(reference);
            while (values.Count <= column) values.Add(string.Empty);
            var value = cell.Element(Spreadsheet + "v")?.Value ?? cell.Element(Spreadsheet + "is")?.Value ?? string.Empty;
            if ((string?)cell.Attribute("t") == "s" && int.TryParse(value, out var sharedIndex) && sharedIndex >= 0 && sharedIndex < sharedStrings.Count) value = sharedStrings[sharedIndex];
            values[column] = value;
        }
        return values;
    }

    private static int ColumnIndex(string reference)
    {
        var result = 0;
        foreach (var character in reference.TakeWhile(char.IsLetter)) result = result * 26 + char.ToUpperInvariant(character) - 'A' + 1;
        return Math.Max(0, result - 1);
    }

    private static bool IsBlank(IReadOnlyList<string> values)
    {
        foreach (var value in values) if (!string.IsNullOrWhiteSpace(value)) return false;
        return true;
    }

    private static XDocument ReadXml(ZipArchive archive, string path)
    {
        var entry = archive.GetEntry(path) ?? throw new FormatException("The XLSX file is missing " + path + ".");
        return XDocument.Load(entry.Open());
    }
}