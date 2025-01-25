using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Svg.Skia;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace Eclair.Controls;

public class HeaderPanel : StackPanel
{
    public static readonly StyledProperty<string> TitleProperty = 
        AvaloniaProperty.Register<HeaderPanel, string>(nameof(Title), "empty");
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public event EventHandler<Avalonia.Interactivity.RoutedEventArgs>? GotoBack
    {
        add => (Children[0] as Button)!.Click += value;
        remove => (Children[0] as Button)!.Click -= value;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TitleProperty)
            (Children[1] as TextBlock)!.Text = change.NewValue as string;
    }

    public HeaderPanel()
    {
        this[!BackgroundProperty] = new DynamicResourceExtension("SystemAccentColor");
        // ^^^^^ color setting
        
        Orientation = Orientation.Horizontal;
        Spacing = 12;
        Height = 64;

        var backButton = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(8),
            Width = 48,
            Height = 48,
            Background = Brushes.Transparent
        };

        var image = new Image
        {
            Source = new SvgImage { Source = SvgSource.Load<SvgSource>("/Assets/themes/backarrow.svg", new("avares://Eclair/Assets/themes/backarrow.svg")) }
        };

        backButton.Content = image;

        var headerTitle = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 24,
            Foreground = Brushes.White
        };

        Children.Add(backButton);
        Children.Add(headerTitle);
    }
}