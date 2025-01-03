using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;

namespace Eclair.Controls;

public sealed class RefTextButton : Button, INotifyPropertyChanged
{
    private string? _text;

    public RefTextButton()
    {
        Background = Brushes.Transparent;
        UpdateContent();
        Content = Text;
    }

    public string? Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
                UpdateContent();
            }
        }
    }
    public string? Link { get; set; }

    private void UpdateContent()
    {
        Content = new TextBlock
        {
            Text = _text,
            Foreground = Brushes.Blue,
            TextDecorations = TextDecorations.Underline
        };
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

    new public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}