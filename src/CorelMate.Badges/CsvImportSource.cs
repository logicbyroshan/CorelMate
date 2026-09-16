using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CorelMate.Badges;

public static class CsvImportSource
{
    public static ImportedDataSet Read(string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        using (var reader = new StreamReader(path, new UTF8Encoding(false, true), true))
        {
            var records = ParseRecords(reader.ReadToEnd());
            if (records.Count == 0) throw new FormatException("The CSV file is empty.");
            var headers = records[0];
            if (headers.Count == 0 || AllBlank(headers)) throw new FormatException("The CSV file has no header row.");
            var rows = new List<IReadOnlyDictionary<string, string>>();
            for (var index = 1; index < records.Count; index++)
            {
                var record = records[index];
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var column = 0; column < headers.Count; column++) row[headers[column]] = column < record.Count ? record[column] : string.Empty;
                rows.Add(row);
            }

            return new ImportedDataSet(Path.GetFileName(path), string.Empty, headers, rows);
        }
    }

    private static List<List<string>> ParseRecords(string text)
    {
        var records = new List<List<string>>();
        var record = new List<string>();
        var field = new StringBuilder();
        var quoted = false;
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (quoted)
            {
                if (character == '"')
                {
                    if (index + 1 < text.Length && text[index + 1] == '"') { field.Append('"'); index++; }
                    else quoted = false;
                }
                else field.Append(character);
            }
            else if (character == '"' && field.Length == 0) quoted = true;
            else if (character == ',') { record.Add(field.ToString()); field.Clear(); }
            else if (character == '\r' || character == '\n')
            {
                if (character == '\r' && index + 1 < text.Length && text[index + 1] == '\n') index++;
                record.Add(field.ToString()); field.Clear();
                if (record.Count > 1 || record[0].Length > 0) records.Add(record);
                record = new List<string>();
            }
            else field.Append(character);
        }

        if (quoted) throw new FormatException("The CSV file contains an unterminated quoted field.");
        if (field.Length > 0 || record.Count > 0) { record.Add(field.ToString()); records.Add(record); }
        return records;
    }

    private static bool AllBlank(IReadOnlyList<string> values)
    {
        foreach (var value in values) if (!string.IsNullOrWhiteSpace(value)) return false;
        return true;
    }
}