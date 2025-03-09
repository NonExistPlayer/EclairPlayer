using File = System.IO.File;
using TagFile = TagLib.File;
using TagLib;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Eclair.Views;

partial class MainView
{
    private ushort AddMusicItem(Media media)
    {
        int num = MusicPanel.Children.Count;

        if (MusicPanel.Children.Count == 1 &&
            MusicPanel.Children[0] is TextBlock)
            MusicPanel.Children.Clear();

        var border = new Border
        {
            CornerRadius = new CornerRadius(7),
            Margin = new Thickness(5),
            Background = new SolidColorBrush(new Color(125, 199, 199, 199))
        };

        var grid = new Grid { Height = 64 };
        ToolTip.SetTip(grid, media.LocalPath);

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

        IPicture? picture = media.Tags.Pictures.Length > 0 ? media.Tags.Pictures[0] : null;

        if (picture != null)
        {
            string fpath = TempPath + $"{media.Tags.Title}-picture0";

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
            Text = media.FullTitle,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };

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

        ushort id = (ushort)playlist.Count;

        button.Click += delegate
        {
            currenttrack = (ushort)num;
            PlayTrack(id);
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
            LoadTrack(id);
        };

        border.Child = grid;

        if (string.IsNullOrEmpty(SearchBox.Text))
            MusicPanel.Children.Add(border);
        else if (musicitems != null)
            musicitems = [.. musicitems, border];

        playlist.Add(media);

        return id;
    }
}