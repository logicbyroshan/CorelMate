using System;
using System.Collections.Generic;

namespace CorelMate.Badges;

public sealed class BadgeDataRow
{
    public BadgeDataRow(IReadOnlyDictionary<string, string> values, int quantity)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));
        var copiedValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in values) copiedValues[pair.Key] = pair.Value;
        Values = copiedValues;
        if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Quantity = quantity;
    }

    public IReadOnlyDictionary<string, string> Values { get; }
    public int Quantity { get; }

    public string GetValue(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        return Values.TryGetValue(name, out var value) ? value : string.Empty;
    }
}