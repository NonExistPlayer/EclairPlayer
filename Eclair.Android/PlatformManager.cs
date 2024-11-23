using System;
using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace Eclair.Android;

internal class PlatformManager : IPlatformManager
{
    public Action? TogglePause { get; set; }

    public const int NOTIFICATION_ID = 1001;

    NotificationManager? nManager;

    public void ShowPlayerNotification(string currentTrackTitle, bool isPlaying)
    {
        const string CHANNEL_ID = "audio_player_channel";

        if (nManager is null)
        {
            nManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService)!;

            var channel = new NotificationChannel(
                CHANNEL_ID,
                resources.android_notifications_mplayer,
                NotificationImportance.Min);

            nManager.CreateNotificationChannel(channel);
        }
        
        var builder = new NotificationCompat.Builder(Application.Context, CHANNEL_ID)
            .SetContentTitle("Eclair")
            .SetContentText(resources.mplayer_nowplaying + currentTrackTitle)
            .SetPriority(NotificationCompat.PriorityMin)
            .SetSmallIcon(Resource.Drawable.Icon)
            .SetVisibility(NotificationCompat.VisibilitySecret)
            .SetOngoing(true);

        var intent = new Intent(Application.Context, typeof(NotificationReceiver));
        intent.SetAction("ACTION_TOGGL");
        var action_toggle = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;

        if (isPlaying)
            builder.AddAction(Resource.Drawable.ic_pause, resources.mplayer_pause, action_toggle);
        else
            builder.AddAction(Resource.Drawable.ic_play, resources.mplayer_play, action_toggle);

        nManager.Notify(NOTIFICATION_ID, builder.Build());
    }
    public void HidePlayerNotification() => nManager?.Cancel(NOTIFICATION_ID);
}