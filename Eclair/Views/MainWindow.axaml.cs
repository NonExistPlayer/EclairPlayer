using Avalonia.Controls;
using Avalonia.Media;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Background = new SolidColorBrush(new Color(125, 0, 0, 0));
    }

    public MainWindow(UserControl view)
    {
        InitializeComponent();

        Content = view;
        Width = 600;
        Height = 450;
        MaxWidth = 600;
        MaxHeight = 450;
        CanResize = false;
    }
}