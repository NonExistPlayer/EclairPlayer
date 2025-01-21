using File = System.IO.File;
using TagFile = TagLib.File;
using TagLib;
using LibVLCSharp.Shared;
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;

namespace Eclair.Views;

partial class MainView
{
    internal void AddMusicItem(string path) => AddMusicItem(Path.GetFileName(path), File.OpenRead(path), path);
    private void AddMusicItem(string name, Stream stream, string? path = null)
    {
        int num = MusicPanel.Children.Count;

        Logger.Log($"AddMusicItem({(path ?? name)})");
        if (MusicPanel.Children.Count == 1 &&
            MusicPanel.Children[0] is TextBlock)
            MusicPanel.Children.Clear();
        var tag = TagFile.Create(new ReadOnlyFileImplementation(name, stream)).Tag;

        var border = new Border
        {
            CornerRadius = new CornerRadius(7),
            Margin = new Thickness(5),
            Background = Application.Current?.ActualThemeVariant == ThemeVariant.Light ?
                new SolidColorBrush(new Color(125, 199, 199, 199)) : new SolidColorBrush(new Color(125, 133, 133, 133))
        };

        var grid = new Grid { Height = 64 };
        ToolTip.SetTip(grid, path);

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var trackImage = new Image
        {
            Height = 56,
            Margin = new Thickness(6, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };

        IPicture? picture = tag.Pictures.Length > 0 ? tag.Pictures[0] : null;

        if (picture != null)
        {
            string fpath = TempPath + $"{tag.Title}-picture0";

            if (File.Exists(fpath))
                goto display;

            var outputstream = File.OpenWrite(fpath);
            outputstream.Write(picture.Data.Data, 0, picture.Data.Count);
            outputstream.Close();

        display:
            trackImage.Source = new Bitmap(fpath);
        }
        else trackImage.Source = Application.Current?.FindResource("unknowntrack") as Bitmap;

        var textBlock = new TextBlock
        {
            Text = $"{string.Join(", ", tag.Performers)} - {tag.Title}",
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };

        if (textBlock.Text == " - ")
            textBlock.Text = name;

        var playButtonImage = new Image
        {
            Source = (IImage?)Application.Current!.Resources["playbutton"],
            Height = 48
        };

        var button = new Button
        {
            Content = playButtonImage,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = Brushes.Transparent
        };

        button.Click += delegate
        {
            currenttrack = (ushort)num;
            LoadMusicFile(name, stream);
            PlayOrPause();
        };

        Grid.SetColumn(textBlock, 1);
        Grid.SetColumn(button, 2);

        grid.Children.Add(trackImage);
        grid.Children.Add(textBlock);
        grid.Children.Add(button);

        border.PointerReleased += (s, e) =>
        {
            if (e.InitialPressMouseButton == Avalonia.Input.MouseButton.Right /* <-- i have no idea why it works like this*/) return;
            currenttrack = (ushort)num;
            LoadMusicFile(name, stream);
        };

        border.Child = grid;

        if (SearchBox.Text == "")
            MusicPanel.Children.Add(border);
        else if (musicitems != null)
            musicitems = [.. musicitems, border];
    }
    
    internal void LoadMusicFile(string name, Stream stream)
    {
        MusDurationLabel.Content = "00:00";

        if (player.Media == null)
        {
            MusSlider.IsEnabled = true;
        }
        else
        {
            Stop();
            player.Media.Dispose();
        }

        var file = TagFile.Create(new ReadOnlyFileImplementation(name, stream));
        var tags = file.Tag;

        SetTitle($"{string.Join(", ", tags.Performers)} - {tags.Title}");

        if (TitleLabel.Content?.ToString() == " - ")
            SetTitle(name);

        SetTitle(TitleLabel.Content?.ToString());

        if (!OperatingSystem.IsAndroid())
            ((Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
                .MainWindow as MainWindow)!.SetTitle($"Eclair - {name}");

        IPicture? picture = tags.Pictures.Length > 0 ? tags.Pictures[0] : null;

        if (picture != null)
        {
            string fpath = TempPath + $"{tags.Title}-picture0";

            if (File.Exists(fpath))
                goto display;

            var outputstream = File.OpenWrite(fpath);
            outputstream.Write(picture.Data.Data, 0, picture.Data.Count);
            outputstream.Close();

        display:
            SetImage(new Bitmap(fpath));
        }
        else SetImage(Application.Current?.FindResource("unknowntrack") as Bitmap);

        player.Media = new(vlc, new StreamMediaInput(stream));
    }
}