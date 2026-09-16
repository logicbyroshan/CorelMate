namespace CorelMate.Curves;

public enum CurveSkipReason
{
    Locked,
    Hidden,
    Unsupported
}

public sealed class CurveConversionSummary
{
    public int FoundTextObjects { get; set; }
    public int ConvertibleTextObjects { get; set; }
    public int LockedTextObjects { get; set; }
    public int HiddenTextObjects { get; set; }
    public int UnsupportedTextObjects { get; set; }

    public int SkippedTextObjects => LockedTextObjects + HiddenTextObjects + UnsupportedTextObjects;
}