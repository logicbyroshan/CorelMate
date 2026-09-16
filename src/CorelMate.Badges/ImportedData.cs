using System;
using System.Collections.Generic;

namespace CorelMate.Badges;

public sealed class ImportedDataSet
{
    public ImportedDataSet(string sourceName, string worksheetName, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyDictionary<string, string>> rows)
    {
        SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
        WorksheetName = worksheetName ?? string.Empty;
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        Rows = rows ?? throw new ArgumentNullException(nameof(rows));
    }

    public string SourceName { get; }
    public string WorksheetName { get; }
    public IReadOnlyList<string> Headers { get; }
    public IReadOnlyList<IReadOnlyDictionary<string, string>> Rows { get; }
}

public sealed class BadgeImportValidation
{
    public BadgeImportValidation(
        IReadOnlyList<BadgeDataRow> rows,
        IReadOnlyList<string> errors,
        IReadOnlyList<string> usedColumns,
        IReadOnlyList<string> unusedColumns)
    {
        Rows = rows;
        Errors = errors;
        UsedColumns = usedColumns;
        UnusedColumns = unusedColumns;
    }

    public IReadOnlyList<BadgeDataRow> Rows { get; }
    public IReadOnlyList<string> Errors { get; }
    public IReadOnlyList<string> UsedColumns { get; }
    public IReadOnlyList<string> UnusedColumns { get; }
    public bool IsValid => Errors.Count == 0 && Rows.Count > 0;
}

public static class BadgeImportMapper
{
    public static BadgeImportValidation Validate(ImportedDataSet dataSet, IReadOnlyList<string> variables)
    {
        if (dataSet == null) throw new ArgumentNullException(nameof(dataSet));
        if (variables == null) throw new ArgumentNullException(nameof(variables));

        var errors = new List<string>();
        var normalizedHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var duplicateHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in dataSet.Headers)
        {
            var normalized = Normalize(header);
            if (normalized.Length == 0) continue;
            if (normalizedHeaders.ContainsKey(normalized)) duplicateHeaders.Add(normalized);
            else normalizedHeaders[normalized] = header.Trim();
        }

        foreach (var duplicate in duplicateHeaders) errors.Add("Duplicate column header: " + duplicate + ".");
        var quantityHeader = FindHeader(normalizedHeaders, "QUANTITY", "QTY");
        if (quantityHeader == null) errors.Add("Missing required column: QUANTITY.");

        var used = new List<string>();
        foreach (var variable in variables)
        {
            var header = FindHeader(normalizedHeaders, variable);
            if (header == null) errors.Add("Missing required column: " + variable + ".");
            else used.Add(header);
        }

        if (quantityHeader != null) used.Add(quantityHeader);
        var unused = new List<string>();
        foreach (var header in dataSet.Headers)
        {
            if (!ContainsIgnoreCase(used, header.Trim()) && header.Trim().Length > 0) unused.Add(header.Trim());
        }

        var rows = new List<BadgeDataRow>();
        if (errors.Count > 0) return new BadgeImportValidation(rows, errors, used, unused);

        var rowNumber = 1;
        foreach (var sourceRow in dataSet.Rows)
        {
            rowNumber++;
            if (IsBlank(sourceRow)) continue;
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var variable in variables)
            {
                var header = FindHeader(normalizedHeaders, variable)!;
                var value = GetValue(sourceRow, header);
                if (value.Length == 0) errors.Add("Row " + rowNumber + " — " + variable + " is empty.");
                values[variable] = value;
            }

            var quantityText = GetValue(sourceRow, quantityHeader!);
            if (!int.TryParse(quantityText, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var quantity) || quantity < 0)
            {
                errors.Add("Row " + rowNumber + " — invalid quantity \"" + quantityText + "\".");
                continue;
            }

            rows.Add(new BadgeDataRow(values, quantity));
        }

        if (rows.Count == 0 && errors.Count == 0) errors.Add("The imported data contains no non-empty data rows.");
        return new BadgeImportValidation(rows, errors, used, unused);
    }

    public static string Normalize(string value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant();
    }

    private static string? FindHeader(IReadOnlyDictionary<string, string> headers, params string[] names)
    {
        foreach (var name in names) if (headers.TryGetValue(Normalize(name), out var header)) return header;
        return null;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> row, string header)
    {
        return row.TryGetValue(header, out var value) ? value.Trim() : string.Empty;
    }

    private static bool IsBlank(IReadOnlyDictionary<string, string> row)
    {
        foreach (var value in row.Values) if (!string.IsNullOrWhiteSpace(value)) return false;
        return true;
    }

    private static bool ContainsIgnoreCase(IEnumerable<string> values, string value)
    {
        foreach (var item in values) if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}