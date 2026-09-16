using System.Windows;
using System.Windows.Controls;

namespace CorelMate.UI;

public sealed partial class CorelMatePanel : UserControl
{
    public CorelMatePanel() : this("2026 / v27")
    {
    }

    public CorelMatePanel(string targetVersion)
    {
        InitializeComponent();
        TargetVersion.Text = targetVersion;
    }
}