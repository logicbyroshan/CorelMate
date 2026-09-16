using System;
using System.Runtime.InteropServices;
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

    public static CorelDrawHost ConnectToRunningInstance()
    {
        var application = (CorelApplication)Marshal.GetActiveObject("CorelDRAW.Application");
        return new CorelDrawHost(application);
    }

    public static CorelDrawHost ConnectToComObject(object comObject)
    {
        if (comObject == null) throw new ArgumentNullException(nameof(comObject));
        var unknown = Marshal.GetIUnknownForObject(comObject);
        try
        {
            var application = (CorelApplication)Marshal.GetTypedObjectForIUnknown(unknown, typeof(CorelApplication));
            return new CorelDrawHost(application);
        }
        finally
        {
            Marshal.Release(unknown);
        }
    }

    public CorelApplication Application { get; }
    public CorelDocument? ActiveDocument => Application.ActiveDocument;
    public CorelPage? ActivePage => Application.ActivePage;
    public CorelLayer? ActiveLayer => Application.ActiveLayer;
    public CorelShapeRange? ActiveSelection => Application.ActiveSelectionRange;
}