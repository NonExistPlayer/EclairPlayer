using System;
using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace Eclair.Android;

internal class PlatformManager : IPlatformManager
{
    public Action? TogglePause { get; set; }
    public Action? Stop { get; set; }

    public const int NOTIFICATION_ID = 1001;

    NotificationManager? nManager;

    public void ShowPlayerNotification(string currentTrackTitle, bool isPlaying)
    {
        const string CHANNEL_ID = "audio_player_channel";

        static PendingIntent GetIntent(string action)
        {
            var intent = new Intent(Application.Context, typeof(NotificationReceiver));
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
        }

        if (nManager is null)
        {
            nManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService)!;

            var channel = new NotificationChannel(
                CHANNEL_ID,
                resources.android_notifications_mplayer,
                NotificationImportance.Low);

            nManager.CreateNotificationChannel(channel);
        }
        
        var builder = new NotificationCompat.Builder(Application.Context, CHANNEL_ID)
            .SetContentTitle("Eclair")
            .SetContentText(resources.mplayer_nowplaying + currentTrackTitle)
            .SetPriority(NotificationCompat.PriorityMin)
            .SetSmallIcon(isPlaying ? Resource.Drawable.ic_play : Resource.Drawable.ic_pause)
            .SetVisibility(NotificationCompat.VisibilitySecret)
            .SetOngoing(true);

        var action_toggle = GetIntent("ACTION_TOGGLE");

        if (isPlaying)
            builder.AddAction(Resource.Drawable.ic_pause, resources.mplayer_pause, action_toggle);
        else
            builder.AddAction(Resource.Drawable.ic_play, resources.mplayer_play, action_toggle);

        builder.AddAction(Resource.Drawable.ic_stop, resources.mplayer_stop, GetIntent("ACTION_STOP"));

        nManager.Notify(NOTIFICATION_ID, builder.Build());
    }
    public void HidePlayerNotification() => nManager?.Cancel(NOTIFICATION_ID);
}