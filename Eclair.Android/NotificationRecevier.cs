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
                Main.PManager.TogglePause?.Invoke();
                break;
            case "ACTION_STOP":
                Main.PManager.Stop?.Invoke();
                break;
        }
    }
}