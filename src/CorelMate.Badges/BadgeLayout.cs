using System;
using System.Collections.Generic;

namespace CorelMate.Badges;

public sealed class BadgeLayoutSettings
{
    public double PageWidthMillimeters { get; set; }
    public double PageHeightMillimeters { get; set; }
    public double BadgeWidthMillimeters { get; set; }
    public double BadgeHeightMillimeters { get; set; }
    public double HorizontalGapMillimeters { get; set; }
    public double VerticalGapMillimeters { get; set; }
    public double LeftMarginMillimeters { get; set; }
    public double RightMarginMillimeters { get; set; }
    public double TopMarginMillimeters { get; set; }
    public double BottomMarginMillimeters { get; set; }
}

public readonly struct BadgePosition
{
    public BadgePosition(double xFromLeftMillimeters, double yFromTopMillimeters)
    {
        XFromLeftMillimeters = xFromLeftMillimeters;
        YFromTopMillimeters = yFromTopMillimeters;
    }

    public double XFromLeftMillimeters { get; }
    public double YFromTopMillimeters { get; }
}

public sealed class BadgePageLayout
{
    public BadgePageLayout(IReadOnlyList<BadgePosition> positions)
    {
        Positions = positions ?? throw new ArgumentNullException(nameof(positions));
    }

    public IReadOnlyList<BadgePosition> Positions { get; }
}

public sealed class BadgeLayoutPlan
{
    public BadgeLayoutPlan(int columns, int rowsPerPage, IReadOnlyList<BadgePageLayout> pages)
    {
        Columns = columns;
        RowsPerPage = rowsPerPage;
        Pages = pages ?? throw new ArgumentNullException(nameof(pages));
    }

    public int Columns { get; }
    public int RowsPerPage { get; }
    public IReadOnlyList<BadgePageLayout> Pages { get; }
    public int PerPage => Columns * RowsPerPage;
}

public static class BadgeLayoutEngine
{
    public static BadgeLayoutPlan Plan(BadgeLayoutSettings settings, int quantity)
    {
        Validate(settings);
        if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var columns = Fit(settings.PageWidthMillimeters - settings.LeftMarginMillimeters - settings.RightMarginMillimeters,
            settings.BadgeWidthMillimeters, settings.HorizontalGapMillimeters);
        var rows = Fit(settings.PageHeightMillimeters - settings.TopMarginMillimeters - settings.BottomMarginMillimeters,
            settings.BadgeHeightMillimeters, settings.VerticalGapMillimeters);
        if (columns < 1 || rows < 1) throw new InvalidOperationException("The badge does not fit within the page margins.");

        var pages = new List<BadgePageLayout>();
        for (var pageStart = 0; pageStart < quantity; pageStart += columns * rows)
        {
            var positions = new List<BadgePosition>();
            var pageCount = Math.Min(columns * rows, quantity - pageStart);
            for (var index = 0; index < pageCount; index++)
            {
                var column = index % columns;
                var row = index / columns;
                positions.Add(new BadgePosition(
                    settings.LeftMarginMillimeters + column * (settings.BadgeWidthMillimeters + settings.HorizontalGapMillimeters),
                    settings.TopMarginMillimeters + row * (settings.BadgeHeightMillimeters + settings.VerticalGapMillimeters)));
            }

            pages.Add(new BadgePageLayout(positions));
        }

        return new BadgeLayoutPlan(columns, rows, pages);
    }

    private static int Fit(double available, double size, double gap)
    {
        return (int)Math.Floor((available + gap) / (size + gap));
    }

    private static void Validate(BadgeLayoutSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (settings.PageWidthMillimeters <= 0 || settings.PageHeightMillimeters <= 0) throw new ArgumentOutOfRangeException(nameof(settings));
        if (settings.BadgeWidthMillimeters <= 0 || settings.BadgeHeightMillimeters <= 0) throw new ArgumentOutOfRangeException(nameof(settings));
        if (settings.HorizontalGapMillimeters < 0 || settings.VerticalGapMillimeters < 0) throw new ArgumentOutOfRangeException(nameof(settings));
        if (settings.LeftMarginMillimeters < 0 || settings.RightMarginMillimeters < 0 || settings.TopMarginMillimeters < 0 || settings.BottomMarginMillimeters < 0) throw new ArgumentOutOfRangeException(nameof(settings));
    }
}