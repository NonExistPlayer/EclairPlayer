using Avalonia.Controls;
using System;

namespace Eclair.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        if (OperatingSystem.IsAndroid())
            Sections.IsVisible = false;

        CB_UseCircleIconAnimation.IsChecked = Config.UseCircleIconAnimation;
        CB_DisableCustomBorder.IsChecked = Config.DisableCustomBorder;
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
    }
}