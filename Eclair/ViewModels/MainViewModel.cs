using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Projektanker.Icons.Avalonia;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Eclair.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public const int FontSize = 36;
#pragma warning disable CA1822
    public Icon PlayIcon => new() { Value = "fa-play", FontSize = FontSize };
    public Icon PauseIcon => new() { Value = "fa-pause", FontSize = FontSize };
    public Icon NextIcon => new() { Value = "fa-forward-step", FontSize = FontSize };
#pragma warning restore CA1822

    [ObservableProperty]
    string title = "-";
    [ObservableProperty]
    IImage? imageSource;
}
