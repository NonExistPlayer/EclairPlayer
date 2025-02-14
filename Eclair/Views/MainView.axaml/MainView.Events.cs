// Most of the events are stored in this file.
using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace Eclair.Views;

partial class MainView
{
    private async void SelectFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await GetMusicFile();
    private async void SelectDir(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this); if (toplevel is null) return;
        var dirs = await toplevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = resources.ui_selectdir
            }
        );
        if (dirs.Count <= 0) return;

        await foreach (var item in dirs[0].GetItemsAsync())
        {
            if (item is not IStorageFile file) return;

            if (HasSupportedFormat(file.Name))
                AddMusicItem(file.Name, await file.OpenReadAsync());
        }
    }
    private void GotoSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        prevcontent = Content;
        App.ChangeView(new SettingsView(), this);
    }
    private void GotoAbout(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        prevcontent = Content;
        App.ChangeView(new AboutView(), this);
    }
    internal void BackToList(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainGrid.RowDefinitions[1] = new(GridLength.Star);
        MainGrid.RowDefinitions[2] = new(new(100));
        MainGrid.RowDefinitions[3] = new(new(0));
        AudioPanel.IsVisible = true;
        SearchBox.IsEnabled = true;
        BackButton.IsVisible = false;
        BackButton.IsEnabled = false;
        if (snowfall != null)
            (snowfall.Effect as BlurEffect)!.Radius = SnowfallBlurRadius;
    }
    private void ShowPlayer(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right) return;
        ShowPlayer();
    }
    private void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (SearchBox.Text == "")
        {
            MusicPanel.Children.Clear();
            MusicPanel.Children.AddRange(musicitems ?? []);
            musicitems = null;
            return;
        }

        if (musicitems is null)
        {
            musicitems = new Control[MusicPanel.Children.Count];
            MusicPanel.Children.CopyTo(musicitems, 0);
        }

        MusicPanel.Children.Clear();

        if (MusicPanel.Children.Count > 0 &&
            MusicPanel.Children[0] is not TextBlock)
        {
            MusicPanel.Children.AddRange(
                musicitems.Where(m =>
                    ((TextBlock)
                        ((Grid)
                            ((Border)m).Child!)
                                .Children[1])
                                .Text!
                                .Contains(SearchBox.Text ?? "", StringComparison.CurrentCultureIgnoreCase)
                )
            );
        }
    }
    internal void ShowPlayer()
    {
        MainGrid.RowDefinitions[1] = new(new(0));
        MainGrid.RowDefinitions[2] = new(new(0));
        MainGrid.RowDefinitions[3] = new(GridLength.Star);
        AudioPanel.IsVisible = false;
        SearchBox.IsEnabled = false;
        BackButton.IsVisible = true;
        BackButton.IsEnabled = true;
        if (!Config.DisableEffects && snowfall != null)
            (snowfall?.Effect as BlurEffect)!.Radius = 10;
    }
}