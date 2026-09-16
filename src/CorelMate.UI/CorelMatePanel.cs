using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CorelMate.Badges;
using CorelMate.Host;

namespace CorelMate.UI;

public sealed partial class CorelMatePanel : UserControl
{
    private readonly CorelDrawBadgeGenerator? generator;
    private CorelBadgeMaster? master;

    public CorelMatePanel() : this("2026 / v27")
    {
    }

    public CorelMatePanel(string targetVersion)
    {
        InitializeComponent();
        try
        {
            generator = new CorelDrawBadgeGenerator(CorelDrawHost.ConnectToRunningInstance());
            StatusText.Text = "CorelDRAW " + targetVersion + " connected. Select a master badge.";
        }
        catch (Exception exception)
        {
            StatusText.Text = "CorelDRAW connection unavailable: " + exception.Message;
        }
    }

    private void UseSelectedButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (generator == null) throw new InvalidOperationException("CorelDRAW is not connected.");
            master = generator.CaptureSelectedMaster();
            VariablesText.Text = string.Join(" | ", master.Variables);
            BadgeWidthText.Text = master.WidthMillimeters.ToString("0.###", CultureInfo.InvariantCulture);
            BadgeHeightText.Text = master.HeightMillimeters.ToString("0.###", CultureInfo.InvariantCulture);
            GenerateButton.IsEnabled = true;
            StatusText.Text = "Master artwork captured. Enter one data row per line.";
        }
        catch (Exception exception)
        {
            StatusText.Text = exception.Message;
            GenerateButton.IsEnabled = false;
        }
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (generator == null || master == null) throw new InvalidOperationException("Capture master artwork first.");
            var rows = ParseRows();
            var settings = new BadgeLayoutSettings
            {
                PageWidthMillimeters = master.PageWidthMillimeters,
                PageHeightMillimeters = master.PageHeightMillimeters,
                BadgeWidthMillimeters = ParseNumber(BadgeWidthText.Text, "badge width"),
                BadgeHeightMillimeters = ParseNumber(BadgeHeightText.Text, "badge height"),
                HorizontalGapMillimeters = ParseNumber(HorizontalGapText.Text, "horizontal gap"),
                VerticalGapMillimeters = ParseNumber(VerticalGapText.Text, "vertical gap"),
                LeftMarginMillimeters = ParseNumber(HorizontalMarginText.Text, "horizontal margin"),
                RightMarginMillimeters = ParseNumber(HorizontalMarginText.Text, "horizontal margin"),
                TopMarginMillimeters = ParseNumber(VerticalMarginText.Text, "vertical margin"),
                BottomMarginMillimeters = ParseNumber(VerticalMarginText.Text, "vertical margin")
            };
            var result = generator.Generate(master, settings, rows);
            ResultText.Text = "Generated " + result.TotalBadges + " badges across " + result.PagesCreated + " page(s). The master was kept.";
        }
        catch (Exception exception)
        {
            ResultText.Text = exception.Message;
        }
    }

    private List<BadgeDataRow> ParseRows()
    {
        var rows = new List<BadgeDataRow>();
        foreach (var line in RowsText.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('|');
            if (master == null || parts.Length != master.Variables.Count + 1) throw new FormatException("Each data row must contain one value per variable followed by quantity.");
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < master.Variables.Count; index++) values[master.Variables[index]] = parts[index].Trim();
            if (!int.TryParse(parts[parts.Length - 1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) || quantity < 0) throw new FormatException("Quantity must be a nonnegative integer.");
            rows.Add(new BadgeDataRow(values, quantity));
        }

        if (rows.Count == 0) throw new FormatException("Enter at least one data row.");
        return rows;
    }

    private static double ParseNumber(string text, string label)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) throw new FormatException("Enter a valid number for " + label + ".");
        return value;
    }
}