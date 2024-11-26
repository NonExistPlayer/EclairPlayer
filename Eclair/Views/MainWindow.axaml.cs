using Avalonia.Controls;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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