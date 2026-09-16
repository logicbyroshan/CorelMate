namespace CorelMate.Core;

public sealed class CorelMateSettings
{
    public double HorizontalBadgeGapMillimeters { get; set; } = 3;
    public double VerticalBadgeGapMillimeters { get; set; } = 3;
    public double PageMarginMillimeters { get; set; } = 5;
    public string PreferredAiProvider { get; set; } = string.Empty;
    public string AiModel { get; set; } = string.Empty;
}