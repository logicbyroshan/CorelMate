using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CorelMate.Badges;

public static class PlaceholderParser
{
    private static readonly Regex TokenRegex = new Regex(@"\{\{([^{}]*)\}\}", RegexOptions.Compiled);
    private static readonly Regex NameRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

    public static IReadOnlyList<Placeholder> Parse(string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));
        var placeholders = new List<Placeholder>();
        foreach (Match match in TokenRegex.Matches(text))
        {
            var name = match.Groups[1].Value;
            if (!NameRegex.IsMatch(name)) throw new FormatException("Invalid placeholder: " + match.Value);
            placeholders.Add(new Placeholder(name.ToUpperInvariant(), match.Value, match.Index));
        }

        return placeholders;
    }

    public static IReadOnlyCollection<string> GetNames(string text)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var placeholder in Parse(text)) names.Add(placeholder.Name);
        return names;
    }

    public static string Replace(string text, IReadOnlyDictionary<string, string> values)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));
        if (values == null) throw new ArgumentNullException(nameof(values));
        Parse(text);
        return TokenRegex.Replace(text, match =>
        {
            var name = match.Groups[1].Value;
            return values.TryGetValue(name, out var value) ? value : match.Value;
        });
    }
}