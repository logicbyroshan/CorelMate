namespace CorelMate.Badges;

public readonly struct Placeholder
{
    public Placeholder(string name, string originalText, int startIndex)
    {
        Name = name;
        OriginalText = originalText;
        StartIndex = startIndex;
    }

    public string Name { get; }
    public string OriginalText { get; }
    public int StartIndex { get; }
}