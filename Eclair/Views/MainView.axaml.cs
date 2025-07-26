using Eclair.Core;
using Avalonia.Controls;
using Eclair.ViewModels;

namespace Eclair.Views;

public partial class MainView : UserControl
{
    MainViewModel ViewModel => (MainViewModel)DataContext!;
    public AudioPlayer Player { get; }

    public MainView()
    {
        InitializeComponent();
        Player = new(true);
        Logger.Log("Player initialized.");

        Player.AudioPlay +=
            (sender, e) =>
                PlayButton.Content = ViewModel.PauseIcon;

        Player.AudioPaused +=
            (sender, e) =>
                PlayButton.Content = ViewModel.PlayIcon;

        Player.PluginLoaded +=
            (sender, e) =>
                Logger.Log($"Successfully loaded bass lib: {e.Library}, hnd: {e.Handle}.");

        Player.PluginLoadFailed +=
            (sender, e) =>
                Logger.Error($"Failed to load bass lib: {e.Library}.");

        Player.PluginLoad("aac" );
        Player.PluginLoad("alac");
        Player.PluginLoad("dsd" );
        Player.PluginLoad("flac");
        Player.PluginLoad("midi");
        Player.PluginLoad("opus");
        Player.PluginLoad("webm");
        Player.PluginLoad("wv"  );
    }
}