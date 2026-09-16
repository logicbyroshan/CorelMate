using System;
using CorelMate.Badges;

var grid = BadgeGrid.ForQuantity(5, 2);
if (grid.Rows != 3 || grid.Capacity != 6) throw new InvalidOperationException("Badge grid calculation failed.");
Console.WriteLine("CorelMate.Tests passed.");