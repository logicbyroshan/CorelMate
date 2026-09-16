using System;
using System.Collections.Generic;
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
Console.WriteLine("CorelMate.Tests passed.");