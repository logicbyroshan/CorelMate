using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using CorelMate.Badges;
using CorelMate.Curves;

var grid = BadgeGrid.ForQuantity(5, 2);
if (grid.Rows != 3 || grid.Capacity != 6) throw new InvalidOperationException("Badge grid calculation failed.");

var placeholders = PlaceholderParser.Parse("{{title}} - {{STANDARD}} - {{TITLE}}");
if (placeholders.Count != 3 || placeholders[0].Name != "TITLE") throw new InvalidOperationException("Placeholder parsing failed.");
var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["TITLE"] = "MONITOR", ["STANDARD"] = "NURSERY" };
if (PlaceholderParser.Replace("{{TITLE}} / {{STANDARD}}", values) != "MONITOR / NURSERY") throw new InvalidOperationException("Placeholder replacement failed.");

var settings = new BadgeLayoutSettings
{
	PageWidthMillimeters = 210,
	PageHeightMillimeters = 297,
	BadgeWidthMillimeters = 80,
	BadgeHeightMillimeters = 25,
	HorizontalGapMillimeters = 3,
	VerticalGapMillimeters = 3,
	LeftMarginMillimeters = 5,
	RightMarginMillimeters = 5,
	TopMarginMillimeters = 5,
	BottomMarginMillimeters = 5
};
var layout = BadgeLayoutEngine.Plan(settings, 18);
if (layout.Columns != 2 || layout.RowsPerPage != 10 || layout.Pages.Count != 1 || layout.Pages[0].Positions.Count != 18) throw new InvalidOperationException("Badge layout calculation failed.");

var multiPage = BadgeLayoutEngine.Plan(settings, 21);
if (multiPage.Pages.Count != 2 || multiPage.Pages[1].Positions.Count != 1) throw new InvalidOperationException("Multi-page layout calculation failed.");

try { PlaceholderParser.Parse("{{student name}}"); throw new InvalidOperationException("Invalid placeholder was accepted."); } catch (FormatException) { }
try { new BadgeDataRow(values, -1); throw new InvalidOperationException("Negative quantity was accepted."); } catch (ArgumentOutOfRangeException) { }

var curveSummary = new CurveConversionSummary { FoundTextObjects = 5, ConvertibleTextObjects = 3, LockedTextObjects = 1, HiddenTextObjects = 1 };
if (curveSummary.SkippedTextObjects != 2) throw new InvalidOperationException("Curve conversion summary failed.");

var tempRoot = Path.Combine(Path.GetTempPath(), "CorelMate-import-tests");
Directory.CreateDirectory(tempRoot);
try
{
	var csvPath = Path.Combine(tempRoot, "badges.csv");
	File.WriteAllText(csvPath, "TITLE,STANDARD,QUANTITY,EXTRA\r\n\"Senior, Monitor\",\"वरिष्ठ मॉनिटर\",2,ignored\r\n\"Monitor\",Monitor,8,ignored\r\n\r\n\"Say \"\"Hi\"\"\",Test,1,ignored\r\n", new UTF8Encoding(false));
	var csv = CsvImportSource.Read(csvPath);
	var csvValidation = BadgeImportMapper.Validate(csv, new[] { "TITLE", "STANDARD" });
	if (!csvValidation.IsValid || csvValidation.Rows.Count != 3 || csvValidation.Rows[0].GetValue("TITLE") != "Senior, Monitor" || csvValidation.Rows[0].GetValue("STANDARD") != "वरिष्ठ मॉनिटर" || csvValidation.UnusedColumns.Count != 1) throw new InvalidOperationException("CSV import failed.");

	var duplicatePath = Path.Combine(tempRoot, "duplicate.csv");
	File.WriteAllText(duplicatePath, "TITLE,TITLE,QUANTITY\r\nA,B,1\r\n", new UTF8Encoding(false));
	var duplicateValidation = BadgeImportMapper.Validate(CsvImportSource.Read(duplicatePath), new[] { "TITLE" });
	if (duplicateValidation.IsValid || duplicateValidation.Errors.Count == 0) throw new InvalidOperationException("Duplicate CSV header validation failed.");

	var invalidPath = Path.Combine(tempRoot, "invalid.csv");
	File.WriteAllText(invalidPath, "TITLE,STANDARD,QUANTITY\nA,,2.5\n", new UTF8Encoding(false));
	var invalidValidation = BadgeImportMapper.Validate(CsvImportSource.Read(invalidPath), new[] { "TITLE", "STANDARD" });
	if (invalidValidation.IsValid || invalidValidation.Errors.Count != 2) throw new InvalidOperationException("CSV row validation failed.");

	var xlsxPath = Path.Combine(tempRoot, "badges.xlsx");
	CreateSyntheticXlsx(xlsxPath);
	var xlsx = XlsxImportSource.Read(xlsxPath);
	var xlsxValidation = BadgeImportMapper.Validate(xlsx, new[] { "TITLE", "STANDARD" });
	if (!xlsxValidation.IsValid || xlsx.WorksheetName != "Badges" || xlsxValidation.Rows.Count != 2 || xlsxValidation.Rows[1].GetValue("TITLE") != "मॉनिटर") throw new InvalidOperationException("XLSX import failed.");
}
finally
{
	if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true);
}
Console.WriteLine("CorelMate.Tests passed.");

static void CreateSyntheticXlsx(string path)
{
	using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
	{
		AddEntry(archive, "xl/workbook.xml", "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Badges\" sheetId=\"1\" r:id=\"rId1\" /></sheets></workbook>");
		AddEntry(archive, "xl/_rels/workbook.xml.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Target=\"worksheets/sheet1.xml\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" /></Relationships>");
		AddEntry(archive, "xl/worksheets/sheet1.xml", "<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData><row r=\"1\"><c r=\"A1\" t=\"inlineStr\"><is><t>TITLE</t></is></c><c r=\"B1\" t=\"inlineStr\"><is><t>STANDARD</t></is></c><c r=\"C1\" t=\"inlineStr\"><is><t>QUANTITY</t></is></c></row><row r=\"2\"><c r=\"A2\" t=\"inlineStr\"><is><t>MONITOR</t></is></c><c r=\"B2\" t=\"inlineStr\"><is><t>NURSERY</t></is></c><c r=\"C2\"><v>2</v></c></row><row r=\"3\"><c r=\"A3\" t=\"inlineStr\"><is><t>मॉनिटर</t></is></c><c r=\"B3\" t=\"inlineStr\"><is><t>वरिष्ठ</t></is></c><c r=\"C3\"><v>1</v></c></row></sheetData></worksheet>");
	}
}

static void AddEntry(ZipArchive archive, string name, string content)
{
	using (var writer = new StreamWriter(archive.CreateEntry(name).Open(), new UTF8Encoding(false))) writer.Write(content);
}