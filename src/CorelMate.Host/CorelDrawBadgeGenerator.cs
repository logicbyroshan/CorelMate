using System;
using System.Collections.Generic;
using Corel.Interop.VGCore;
using CorelMate.Badges;
using CorelApplication = Corel.Interop.CorelDRAW.Application;
using CorelDocument = Corel.Interop.VGCore.Document;
using CorelPage = Corel.Interop.VGCore.Page;
using CorelShape = Corel.Interop.VGCore.Shape;
using CorelShapeRange = Corel.Interop.VGCore.ShapeRange;

namespace CorelMate.Host;

public sealed class CorelBadgeMaster
{
    internal CorelBadgeMaster(CorelShapeRange selection, CorelPage page, double widthMillimeters, double heightMillimeters, double pageWidthMillimeters, double pageHeightMillimeters, IReadOnlyList<string> variables)
    {
        Selection = selection;
        Page = page;
        WidthMillimeters = widthMillimeters;
        HeightMillimeters = heightMillimeters;
        PageWidthMillimeters = pageWidthMillimeters;
        PageHeightMillimeters = pageHeightMillimeters;
        Variables = variables;
    }

    internal CorelShapeRange Selection { get; }
    internal CorelPage Page { get; }
    public double WidthMillimeters { get; }
    public double HeightMillimeters { get; }
    public double PageWidthMillimeters { get; }
    public double PageHeightMillimeters { get; }
    public IReadOnlyList<string> Variables { get; }
}

public sealed class CorelBadgeGenerationResult
{
    internal CorelBadgeGenerationResult(int totalBadges, int pagesCreated)
    {
        TotalBadges = totalBadges;
        PagesCreated = pagesCreated;
    }

    public int TotalBadges { get; }
    public int PagesCreated { get; }
}

public sealed class CorelDrawBadgeGenerator
{
    private readonly CorelDrawHost host;

    public CorelDrawBadgeGenerator(CorelDrawHost host) => this.host = host ?? throw new ArgumentNullException(nameof(host));

    public CorelBadgeMaster CaptureSelectedMaster()
    {
        var document = RequireDocument();
        var page = document.ActivePage ?? throw new InvalidOperationException("CorelDRAW does not have an active page.");
        var selection = host.Application.ActiveSelectionRange;
        if (selection == null || selection.Count == 0) throw new InvalidOperationException("Select the complete master artwork first.");

        var variables = new List<string>();
        foreach (var shape in EnumerateShapes(selection.Shapes)) CollectVariables(shape, variables);
        if (variables.Count == 0) throw new InvalidOperationException("The selected artwork contains no valid {{VARIABLE}} placeholders.");

        return new CorelBadgeMaster(selection, page, document.ToUnits(selection.SizeWidth, cdrUnit.cdrMillimeter), document.ToUnits(selection.SizeHeight, cdrUnit.cdrMillimeter), document.ToUnits(page.SizeWidth, cdrUnit.cdrMillimeter), document.ToUnits(page.SizeHeight, cdrUnit.cdrMillimeter), variables);
    }

    public CorelBadgeGenerationResult Generate(CorelBadgeMaster master, BadgeLayoutSettings settings, IReadOnlyList<BadgeDataRow> rows)
    {
        if (master == null) throw new ArgumentNullException(nameof(master));
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (rows == null) throw new ArgumentNullException(nameof(rows));
        var document = RequireDocument();
        var total = 0;
        foreach (var row in rows)
        {
            foreach (var variable in master.Variables)
            {
                if (!row.Values.ContainsKey(variable)) throw new InvalidOperationException("No value was supplied for " + variable + ".");
            }
            total += row.Quantity;
        }

        var plan = BadgeLayoutEngine.Plan(settings, total);
        var pagesToAdd = Math.Max(0, plan.Pages.Count - 1);
        var originalPageCount = document.Pages.Count;
        var createdRanges = new List<CorelShapeRange>();
        document.BeginCommandGroup("CorelMate Generate Badges");
        try
        {
            if (pagesToAdd > 0) document.AddPages(pagesToAdd);
            var itemIndex = 0;
            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                for (var quantityIndex = 0; quantityIndex < row.Quantity; quantityIndex++, itemIndex++)
                {
                    var pageIndex = itemIndex / plan.PerPage;
                    var positionIndex = itemIndex % plan.PerPage;
                    var page = pageIndex == 0 ? master.Page : document.Pages[originalPageCount + pageIndex];
                    var copy = master.Selection.Duplicate(0, 0);
                    ReplacePlaceholders(copy.Shapes, row.Values);
                    MoveToPosition(copy, page, settings, plan.Pages[pageIndex].Positions[positionIndex], document);
                    createdRanges.Add(copy);
                }
            }

            return new CorelBadgeGenerationResult(total, plan.Pages.Count);
        }
        catch
        {
            foreach (var createdRange in createdRanges) { try { createdRange.Delete(); } catch { } }
            if (pagesToAdd > 0) { try { document.DeletePages(originalPageCount + 1, pagesToAdd); } catch { } }
            throw;
        }
        finally
        {
            document.EndCommandGroup();
        }
    }

    private CorelDocument RequireDocument() => host.ActiveDocument ?? throw new InvalidOperationException("CorelDRAW does not have an active document.");

    private static IEnumerable<CorelShape> EnumerateShapes(Shapes shapes)
    {
        for (var index = 1; index <= shapes.Count; index++)
        {
            var shape = shapes[index];
            if (shape.Type == cdrShapeType.cdrGroupShape)
            {
                foreach (var child in EnumerateShapes(shape.Shapes)) yield return child;
            }
            else yield return shape;
        }
    }

    private static void CollectVariables(CorelShape shape, IList<string> variables)
    {
        if (shape.Type != cdrShapeType.cdrTextShape) return;
        foreach (var name in PlaceholderParser.GetNames(shape.Text.Story.Text))
        {
            if (!ContainsIgnoreCase(variables, name)) variables.Add(name);
        }
    }

    private static bool ContainsIgnoreCase(IList<string> values, string value)
    {
        foreach (var existing in values) if (string.Equals(existing, value, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    private static void ReplacePlaceholders(Shapes shapes, IReadOnlyDictionary<string, string> values)
    {
        foreach (var shape in EnumerateShapes(shapes))
        {
            if (shape.Type != cdrShapeType.cdrTextShape) continue;
            foreach (var placeholder in PlaceholderParser.Parse(shape.Text.Story.Text))
            {
                shape.Text.Replace(placeholder.OriginalText, values[placeholder.Name], false, 1, false, true, cdrTextIndexingType.cdrCharacterIndexing);
            }
        }
    }

    private static void MoveToPosition(CorelShapeRange copy, CorelPage page, BadgeLayoutSettings settings, BadgePosition position, CorelDocument document)
    {
        var targetLeft = page.LeftX + document.FromUnits(position.XFromLeftMillimeters, cdrUnit.cdrMillimeter);
        var targetTop = page.TopY - document.FromUnits(position.YFromTopMillimeters, cdrUnit.cdrMillimeter);
        copy.Move(targetLeft - copy.LeftX, targetTop - copy.TopY);
    }
}