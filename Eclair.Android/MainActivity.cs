using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;

namespace Eclair.Android;

[Activity(
    Label = "Eclair",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
#pragma warning disable CA1416
    private readonly static string read_permission =
        Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu /* (Android 13) */ ?
        Manifest.Permission.ReadMediaAudio : Manifest.Permission.ReadExternalStorage;
#pragma warning restore CA1416

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        Main.PManager = new PlatformManager();

        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (ContextCompat.CheckSelfPermission(this, read_permission) != (int)Permission.Granted)
        {
            Main.Logger.Log($"Requesting {read_permission}...");
            ActivityCompat.RequestPermissions(this, [read_permission], 1);
        }

        base.OnCreate(savedInstanceState);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 1)
        {
            if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                Main.Logger.Log($"{read_permission} granted!");
            else
                Main.Logger.Log($"{read_permission} not granted.");
        }
    }

    protected override void Dispose(bool disposing)
    {
        Main.PManager.HidePlayerNotification();

        base.Dispose(disposing);
    }
}
