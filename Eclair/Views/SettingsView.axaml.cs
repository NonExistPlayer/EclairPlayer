using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace Eclair.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        if (OperatingSystem.IsAndroid())
        {
            Sections.IsVisible = false;
            MainGrid.ColumnDefinitions.RemoveAt(0);
        }

        CB_UseCircleIconAnimation.IsChecked = Config.UseCircleIconAnimation;
        CB_DisableCustomBorder.IsChecked = Config.DisableCustomBorder;
        CB_DisableCustomBorder.IsEnabled = OperatingSystem.IsWindows();
        CB_DisableEffects.IsChecked = Config.DisableEffects;
    }

    private void GotoBack(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Content = MainView.prevcontent;

    private void CheckBoxClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not CheckBox cb) return;
        string cfgparam = cb.Name![3..];

        var pInfo = typeof(ConfigJson).GetField(cfgparam);

        if (pInfo == null)
        {
            Logger.Error($"pInfo was null. cfgparam: {cfgparam}");
            return;
        }
        pInfo.SetValue(Config, cb.IsChecked);

        Config.Save();

        MainView? view;
        MainWindow? window = ((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .MainWindow as MainWindow);

        if (OperatingSystem.IsAndroid())
            view = (Application.Current?.ApplicationLifetime as ISingleViewApplicationLifetime)?.MainView as MainView;
        else
            view = window?.View.Content as MainView;

        if (view is null) return;

        switch (cfgparam)
        {
            case "UseCircleIconAnimation": view.Update_UCIA(); break;
            case "DisableCustomBorder":
                if (window is null) return;
                window.Update_DCB();
                foreach (var win in MainWindow.OtherWindows)
                    win.Update_DCB();
                break;
            case "DisableEffects": view.Update_DEff(); break;
        }
    }
}