using System.Collections.Generic;
using Corel.Interop.VGCore;
using CorelShape = Corel.Interop.VGCore.Shape;

namespace CorelMate.Host;

public sealed class CorelShapeNode
{
    internal CorelShapeNode(CorelShape shape, bool hiddenByAncestor, bool lockedByAncestor)
    {
        Shape = shape;
        HiddenByAncestor = hiddenByAncestor;
        LockedByAncestor = lockedByAncestor;
    }

    public CorelShape Shape { get; }
    public bool HiddenByAncestor { get; }
    public bool LockedByAncestor { get; }
    public bool IsHidden => HiddenByAncestor || !Shape.Visible;
    public bool IsLocked => LockedByAncestor || Shape.Locked;
}

public static class CorelShapeTraversal
{
    public static IReadOnlyList<CorelShapeNode> Flatten(ShapeRange selection)
    {
        var nodes = new List<CorelShapeNode>();
        for (var index = 1; index <= selection.Count; index++) AddShape(selection[index], false, false, nodes);
        return nodes;
    }

    private static void AddShape(CorelShape shape, bool hiddenByAncestor, bool lockedByAncestor, ICollection<CorelShapeNode> nodes)
    {
        var hidden = hiddenByAncestor || !shape.Visible;
        var locked = lockedByAncestor || shape.Locked;
        nodes.Add(new CorelShapeNode(shape, hiddenByAncestor, lockedByAncestor));
        if (shape.Type != cdrShapeType.cdrGroupShape) return;
        for (var index = 1; index <= shape.Shapes.Count; index++) AddShape(shape.Shapes[index], hidden, locked, nodes);
    }
}