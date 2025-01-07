using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace Eclair.Views;

public partial class MainWindow : Window
{
    internal static List<MainWindow> OtherWindows = [];
    bool isMainWindow;
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeBackground();
        SetTitle("Eclair");
        MinWidth = 790;
        MinHeight = 670;
        View.Content = new MainView();
        isMainWindow = true;
    }

    public MainWindow(UserControl view)
    {
        InitializeComponent();
        InitializeBackground();
        SetTitle(view.GetType().Name.Replace("View", null));
        View.Content = view;
        Width = 600;
        Height = 450;
        CanResize = false;

        OtherWindows.Add(this);
    }

    public void SetTitle(string title)
    {
        Title = title;
        WinTitle.Text = title;
    }

    private void InitializeBackground()
    {
        Background = new SolidColorBrush(Config.BackgroundColor);
        if (OperatingSystem.IsWindows() && !Config.DisableCustomBorder)
        {
            ExtendClientAreaToDecorationsHint = true;
        }
        else
        {
            WinTitle.IsVisible = false;
            MainGrid.RowDefinitions.RemoveAt(1);
            MainGrid.RowDefinitions[0].Height = GridLength.Star;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        if (isMainWindow)
        {
            Logger.Log("Closing other windows...");
            for (int i = 0; i < OtherWindows.Count; i++)
            {
                var win = OtherWindows[i];
                Logger.Log($"Closing window {win} with index {i}...");
                win.Close();
            }
        }
        else
            OtherWindows.Remove(this);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (!isMainWindow) return;

        if (e.Key == Key.Escape)
            (View.Content as MainView)?.BackToList(null, new());
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender as Button is not Button button) return;
        if ((string?)button.Content == "-")
            WindowState = WindowState.Minimized;
        else
            Close();
    }

    //                   DisableCustomBorder
    internal void Update_DCB()
    {
        if (Config.DisableCustomBorder)
        {
            ExtendClientAreaToDecorationsHint = false;
        }
        else
        {
            WinTitle.IsVisible = true;
            MainGrid.RowDefinitions.Add(new(GridLength.Star));
            MainGrid.RowDefinitions[0].Height = new(30);
        }

        InitializeBackground();
    }
}