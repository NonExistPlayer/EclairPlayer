using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Eclair.Controls;

public sealed class RefTextButton : Button, INotifyPropertyChanged
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<RefTextButton, string>(nameof(Text), "");

    public RefTextButton()
    {
        Background = Brushes.Transparent;
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public string? Link { get; set; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            Content = new TextBlock
            {
                Text = Text,
                Foreground = Brushes.Blue,
                TextDecorations = TextDecorations.Underline
            };
        }
    }

    protected async override void OnClick()
    {
        var launcher = TopLevel.GetTopLevel(this)?.Launcher;

        if (launcher is null)
        {
            Logger.Log("launcher is null.", Notice);
            return;
        }

        bool success = await launcher.LaunchUriAsync(new(Link ?? ""));

        if (!success)
            Logger.Error("LaunchUriAsync() failed.");

        base.OnClick();
    }
}