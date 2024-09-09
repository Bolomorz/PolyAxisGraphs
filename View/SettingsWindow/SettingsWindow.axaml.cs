using Avalonia.Controls;

namespace PolyAxisGraphs.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow(UserControl view)
    {
        Content = view;
        this.Width = view.Width;
        this.Height = view.Height;
        this.MaxHeight = view.MaxHeight;
        this.MaxWidth = view.MaxWidth;
        InitializeComponent();
    }
}