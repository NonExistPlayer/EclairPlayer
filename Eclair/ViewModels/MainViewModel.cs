using System;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eclair.Core;
using Projektanker.Icons.Avalonia;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Eclair.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Constants
    public const string PlayIconValue = "fa-play";
    public const string PauseIconValue = "fa-pause";
    public const string NextIconValue = "fa-forward-step";
    public const string PrevIconValue = "fa-backward-step";
    
    public const string CirclePlayIconValue = "fa-circle-play";
    public const string CirclePauseIconValue = "fa-circle-pause";
    
    public const double FontSize = 36;
    #endregion
    public static Icon CreateIcon(string icon, double fontsize = FontSize, IconAnimation? animation = null)
        => new() {Value = icon, FontSize = fontsize, Animation = animation ?? IconAnimation.None};
    #region Properties
    [ObservableProperty]
    Icon playIcon = CreateIcon(PlayIconValue);
    [ObservableProperty]
    string performers = "-";
    [ObservableProperty]
    string title = Localization.TrackNotLoaded;
    [ObservableProperty]
    string album = "-";
    
    [ObservableProperty]
    ObservablePlaylist playlist = [];
    
    [ObservableProperty]
    IImage? imageSource;
    #endregion
}
