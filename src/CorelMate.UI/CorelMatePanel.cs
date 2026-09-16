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
    private sealed class RowEditor
    {
        public RowEditor(StackPanel container, List<TextBox> valueBoxes, TextBox quantityBox)
        {
            Container = container;
            ValueBoxes = valueBoxes;
            QuantityBox = quantityBox;
        }

        public StackPanel Container { get; }
        public List<TextBox> ValueBoxes { get; }
        public TextBox QuantityBox { get; }
    }

    private readonly CorelDrawBadgeGenerator? generator;
    private readonly CorelTextToCurvesConverter? curvesConverter;
    private readonly List<RowEditor> rowEditors = new List<RowEditor>();
    private CorelBadgeMaster? master;
    private bool isGenerating;

    public CorelMatePanel() : this("2026 / v27")
    {
    }

    public CorelMatePanel(string targetVersion)
    {
        InitializeComponent();
        try
        {
            var corelHost = CorelDrawHost.ConnectToRunningInstance();
            generator = new CorelDrawBadgeGenerator(corelHost);
            curvesConverter = new CorelTextToCurvesConverter(corelHost);
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
            RowsPanel.Children.Clear();
            rowEditors.Clear();
            AddRowHeader();
            AddRow();
            SetMasterControlsEnabled(true);
            StatusText.Text = "Master artwork captured. Enter one data row per line.";
            RefreshPreview();
        }
        catch (Exception exception)
        {
            ShowFriendlyError(exception);
            ClearMasterState();
        }
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (generator == null || master == null) throw new InvalidOperationException("Capture master artwork first.");
            if (isGenerating) return;
            isGenerating = true;
            SetBusyState(true);
            var rows = ParseRows();
            var settings = CreateLayoutSettings();
            var result = generator.Generate(master, settings, rows);
            ResultText.Text = "Generated " + result.TotalBadges + " badges across " + result.PagesCreated + " page(s). The master was kept.";
            StatusText.Text = "Generation complete.";
        }
        catch (Exception exception)
        {
            ShowFriendlyError(exception);
        }
        finally
        {
            isGenerating = false;
            SetBusyState(false);
        }
    }

    private void AddRowButton_Click(object sender, RoutedEventArgs e) => AddRow();

    private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
    {
        if (rowEditors.Count == 0) return;
        RowsPanel.Children.Remove(rowEditors[rowEditors.Count - 1].Container);
        rowEditors.RemoveAt(rowEditors.Count - 1);
        RefreshPreview();
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ParseRows();
            var plan = BadgeLayoutEngine.Plan(CreateLayoutSettings(), TotalQuantity());
            PreviewText.Text = "Total badges: " + TotalQuantity() + "\r\nColumns: " + plan.Columns + "\r\nRows: " + plan.RowsPerPage + "\r\nPer page: " + plan.PerPage + "\r\nPages required: " + plan.Pages.Count;
            ResultText.Text = string.Empty;
        }
        catch (Exception exception)
        {
            ShowFriendlyError(exception);
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        master = null;
        rowEditors.Clear();
        RowsPanel.Children.Clear();
        VariablesText.Text = "None detected";
        BadgeWidthText.Text = string.Empty;
        BadgeHeightText.Text = string.Empty;
        PreviewText.Text = "Capture a master to preview the layout.";
        ResultText.Text = string.Empty;
        StatusText.Text = "Select the complete badge artwork in CorelDRAW first.";
        SetMasterControlsEnabled(false);
    }

    private void ConvertCurvesButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (curvesConverter == null) throw new InvalidOperationException("CorelDRAW is not connected.");
            var preflight = curvesConverter.PreflightSelection();
            if (preflight.Summary.ConvertibleTextObjects == 0)
            {
                CurvesResultText.Text = "No convertible text was found in the selected artwork.";
                if (preflight.Summary.SkippedTextObjects > 0) CurvesResultText.Text += " " + preflight.Summary.SkippedTextObjects + " text object(s) were skipped.";
                return;
            }

            var confirmation = MessageBox.Show(
                "Convert " + preflight.Summary.ConvertibleTextObjects + " text object(s) to curves?\r\n\r\nThis makes those text objects no longer editable as text.",
                "Convert Text to Curves",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);
            if (confirmation != MessageBoxResult.OK) return;

            var result = curvesConverter.Convert(preflight);
            CurvesResultText.Text = result.ConvertedTextObjects + " text object(s) converted to curves.";
            if (result.Summary.SkippedTextObjects > 0) CurvesResultText.Text += " " + result.Summary.SkippedTextObjects + " skipped.";
        }
        catch (Exception exception)
        {
            CurvesResultText.Text = exception is InvalidOperationException ? exception.Message : "CorelDRAW could not complete the text conversion. Verify the selected artwork is still available.";
        }
    }

    private List<BadgeDataRow> ParseRows()
    {
        var rows = new List<BadgeDataRow>();
        if (master == null) throw new InvalidOperationException("Capture the master artwork first.");
        foreach (var editor in rowEditors)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < master.Variables.Count; index++)
            {
                var value = editor.ValueBoxes[index].Text.Trim();
                if (value.Length == 0) throw new FormatException("Enter a value for " + master.Variables[index] + " in every row.");
                values[master.Variables[index]] = value;
            }

            if (!int.TryParse(editor.QuantityBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) || quantity < 0) throw new FormatException("Quantity must be a nonnegative integer.");
            rows.Add(new BadgeDataRow(values, quantity));
        }

        if (rows.Count == 0) throw new FormatException("Add at least one data row.");
        return rows;
    }

    private void AddRow()
    {
        if (master == null) return;
        var container = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };
        var valueBoxes = new List<TextBox>();
        foreach (var variable in master.Variables)
        {
            var box = new TextBox { Width = 92, Margin = new Thickness(0, 0, 4, 0), ToolTip = variable };
            box.TextChanged += RowInputChanged;
            valueBoxes.Add(box);
            container.Children.Add(box);
        }

        var quantityBox = new TextBox { Width = 48, Text = "1", ToolTip = "Quantity" };
        quantityBox.TextChanged += RowInputChanged;
        container.Children.Add(quantityBox);
        rowEditors.Add(new RowEditor(container, valueBoxes, quantityBox));
        RowsPanel.Children.Add(container);
        DeleteRowButton.IsEnabled = true;
    }

    private void AddRowHeader()
    {
        if (master == null) return;
        var header = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
        foreach (var variable in master.Variables) header.Children.Add(new TextBlock { Text = variable, Width = 92, Margin = new Thickness(0, 0, 4, 0), FontWeight = FontWeights.Bold });
        header.Children.Add(new TextBlock { Text = "QTY", Width = 48, FontWeight = FontWeights.Bold });
        RowsPanel.Children.Add(header);
    }

    private void RowInputChanged(object sender, TextChangedEventArgs e)
    {
        if (master != null) RefreshPreview();
    }

    private void LayoutTextChanged(object sender, TextChangedEventArgs e)
    {
        if (master != null) RefreshPreview();
    }

    private BadgeLayoutSettings CreateLayoutSettings()
    {
        if (master == null) throw new InvalidOperationException("Capture the master artwork first.");
        return new BadgeLayoutSettings
        {
            PageWidthMillimeters = master.PageWidthMillimeters,
            PageHeightMillimeters = master.PageHeightMillimeters,
            BadgeWidthMillimeters = ParseNumber(BadgeWidthText.Text, "badge width"),
            BadgeHeightMillimeters = ParseNumber(BadgeHeightText.Text, "badge height"),
            HorizontalGapMillimeters = ParseNumber(HorizontalGapText.Text, "horizontal gap"),
            VerticalGapMillimeters = ParseNumber(VerticalGapText.Text, "vertical gap"),
            LeftMarginMillimeters = ParseNumber(HorizontalMarginText.Text, "left/right margin"),
            RightMarginMillimeters = ParseNumber(HorizontalMarginText.Text, "left/right margin"),
            TopMarginMillimeters = ParseNumber(VerticalMarginText.Text, "top/bottom margin"),
            BottomMarginMillimeters = ParseNumber(VerticalMarginText.Text, "top/bottom margin")
        };
    }

    private int TotalQuantity()
    {
        var total = 0;
        foreach (var editor in rowEditors)
        {
            if (int.TryParse(editor.QuantityBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) && quantity >= 0) total += quantity;
        }

        return total;
    }

    private void RefreshPreview()
    {
        if (master == null) return;
        try
        {
            var settings = CreateLayoutSettings();
            var total = TotalQuantity();
            var plan = BadgeLayoutEngine.Plan(settings, total);
            PreviewText.Text = "Total badges: " + total + "\r\nColumns: " + plan.Columns + "\r\nRows: " + plan.RowsPerPage + "\r\nPer page: " + plan.PerPage + "\r\nPages required: " + plan.Pages.Count;
        }
        catch
        {
            PreviewText.Text = "Preview unavailable until the layout and rows are valid.";
        }
    }

    private void SetMasterControlsEnabled(bool enabled)
    {
        AddRowButton.IsEnabled = enabled;
        DeleteRowButton.IsEnabled = enabled && rowEditors.Count > 0;
        PreviewButton.IsEnabled = enabled;
        GenerateButton.IsEnabled = enabled;
    }

    private void SetBusyState(bool busy)
    {
        UseSelectedButton.IsEnabled = !busy;
        AddRowButton.IsEnabled = !busy && master != null;
        DeleteRowButton.IsEnabled = !busy && rowEditors.Count > 0;
        PreviewButton.IsEnabled = !busy && master != null;
        GenerateButton.IsEnabled = !busy && master != null;
        ResetButton.IsEnabled = !busy;
    }

    private void ClearMasterState()
    {
        master = null;
        SetMasterControlsEnabled(false);
        VariablesText.Text = "None detected";
        PreviewText.Text = "Capture a master to preview the layout.";
    }

    private void ShowFriendlyError(Exception exception)
    {
        var message = exception is FormatException || exception is InvalidOperationException ? exception.Message : "CorelDRAW could not complete the operation. Verify that the document and selected artwork are still available.";
        StatusText.Text = message;
        ResultText.Text = message;
    }

    private static double ParseNumber(string text, string label)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) throw new FormatException("Enter a valid number for " + label + ".");
        return value;
    }
}