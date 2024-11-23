using Android.Content;

namespace Eclair.Android;

[BroadcastReceiver]
public class NotificationReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        //string action = intent?.Action!;
        //if (action == "ACTION_TOGGLE")
        App.PManager.TogglePause?.Invoke();
    }
}