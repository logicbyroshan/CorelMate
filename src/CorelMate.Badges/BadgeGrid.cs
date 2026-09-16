using System;

namespace CorelMate.Badges;

public readonly struct BadgeGrid
{
    public BadgeGrid(int columns, int rows)
    {
        if (columns < 1 || rows < 1) throw new ArgumentOutOfRangeException();
        Columns = columns;
        Rows = rows;
    }

    public int Columns { get; }
    public int Rows { get; }
    public int Capacity => Columns * Rows;

    public static BadgeGrid ForQuantity(int quantity, int columns)
    {
        if (quantity < 1) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (columns < 1) throw new ArgumentOutOfRangeException(nameof(columns));
        return new BadgeGrid(columns, (quantity + columns - 1) / columns);
    }
}