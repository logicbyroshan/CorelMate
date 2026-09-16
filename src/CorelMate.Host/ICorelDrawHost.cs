using System;
using CorelApplication = Corel.Interop.CorelDRAW.Application;
using CorelDocument = Corel.Interop.VGCore.Document;
using CorelLayer = Corel.Interop.VGCore.Layer;
using CorelPage = Corel.Interop.VGCore.Page;
using CorelShapeRange = Corel.Interop.VGCore.ShapeRange;

namespace CorelMate.Host;

public interface ICorelDrawHost
{
    CorelApplication Application { get; }
    CorelDocument? ActiveDocument { get; }
    CorelPage? ActivePage { get; }
    CorelLayer? ActiveLayer { get; }
    CorelShapeRange? ActiveSelection { get; }
}

public sealed class CorelDrawHost : ICorelDrawHost
{
    public CorelDrawHost(CorelApplication application) => Application = application ?? throw new ArgumentNullException(nameof(application));

    public CorelApplication Application { get; }
    public CorelDocument? ActiveDocument => Application.ActiveDocument;
    public CorelPage? ActivePage => Application.ActivePage;
    public CorelLayer? ActiveLayer => Application.ActiveLayer;
    public CorelShapeRange? ActiveSelection => Application.ActiveSelectionRange;
}