using System;
using System.Collections.Generic;
using Corel.Interop.VGCore;
using CorelMate.Curves;
using CorelApplication = Corel.Interop.CorelDRAW.Application;
using CorelDocument = Corel.Interop.VGCore.Document;
using CorelShape = Corel.Interop.VGCore.Shape;
using CorelShapeRange = Corel.Interop.VGCore.ShapeRange;

namespace CorelMate.Host;

public sealed class CorelCurvePreflight
{
    internal CorelCurvePreflight(CorelDocument document, IReadOnlyList<CorelShape> targets, CurveConversionSummary summary)
    {
        Document = document;
        Targets = targets;
        Summary = summary;
    }

    internal CorelDocument Document { get; }
    internal IReadOnlyList<CorelShape> Targets { get; }
    public CurveConversionSummary Summary { get; }
}

public sealed class CorelCurveConversionResult
{
    internal CorelCurveConversionResult(int converted, CurveConversionSummary summary)
    {
        ConvertedTextObjects = converted;
        Summary = summary;
    }

    public int ConvertedTextObjects { get; }
    public CurveConversionSummary Summary { get; }
}

public sealed class CorelTextToCurvesConverter
{
    private readonly CorelDrawHost host;

    public CorelTextToCurvesConverter(CorelDrawHost host) => this.host = host ?? throw new ArgumentNullException(nameof(host));

    public CorelCurvePreflight PreflightSelection()
    {
        var document = host.ActiveDocument ?? throw new InvalidOperationException("CorelDRAW does not have an active document.");
        var selection = host.Application.ActiveSelectionRange;
        if (selection == null || selection.Count == 0) throw new InvalidOperationException("Select the artwork you want to convert first.");

        var summary = new CurveConversionSummary();
        var targets = new List<CorelShape>();
        foreach (var node in CorelShapeTraversal.Flatten(selection))
        {
            if (node.Shape.Type != cdrShapeType.cdrTextShape) continue;
            summary.FoundTextObjects++;
            if (node.IsLocked) summary.LockedTextObjects++;
            else if (node.IsHidden) summary.HiddenTextObjects++;
            else
            {
                summary.ConvertibleTextObjects++;
                targets.Add(node.Shape);
            }
        }

        return new CorelCurvePreflight(document, targets, summary);
    }

    public CorelCurveConversionResult Convert(CorelCurvePreflight preflight)
    {
        if (preflight == null) throw new ArgumentNullException(nameof(preflight));
        var activeDocument = host.ActiveDocument ?? throw new InvalidOperationException("CorelDRAW does not have an active document.");
        if (!ReferenceEquals(activeDocument, preflight.Document)) throw new InvalidOperationException("The selected document changed. Select the artwork again.");

        activeDocument.BeginCommandGroup("CorelMate Convert Text to Curves");
        var converted = 0;
        try
        {
            foreach (var shape in preflight.Targets)
            {
                shape.ConvertToCurves();
                converted++;
            }

            return new CorelCurveConversionResult(converted, preflight.Summary);
        }
        finally
        {
            activeDocument.EndCommandGroup();
        }
    }
}