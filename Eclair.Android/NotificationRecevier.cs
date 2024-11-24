using Android.Content;

namespace Eclair.Android;

[BroadcastReceiver]
public class NotificationReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        string action = intent?.Action!;
        switch (action)
        {
            case "ACTION_TOGGLE":
                App.PManager.TogglePause?.Invoke();
                break;
            case "ACTION_STOP":
                App.PManager.Stop?.Invoke();
                break;
        }
    }
}