using CommunityToolkit.Mvvm.ComponentModel;
using Projektanker.Icons.Avalonia;

namespace Eclair.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public const int FontSize = 36;
    public Icon PlayIcon { get; } = new()  { Value = "fa-play", FontSize = FontSize };
    public Icon PauseIcon { get; } = new() { Value = "fa-pause", FontSize = FontSize };
    public Icon NextIcon { get; } = new() { Value = "fa-forward-step", FontSize = FontSize };
}
