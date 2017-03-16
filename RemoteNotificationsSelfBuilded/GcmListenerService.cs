using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using System;
using System.Net;
using System.Collections.Specialized;

namespace RemoteNotificationsSelfBuilded
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        public static string body = "";
        public static string Body
        {
            get
            {
                return body;
            }

            set
            {
                body = value;
            }
        }
        public override void OnMessageReceived(string from, Bundle data)
        {
            Log.Info("MyGcmListenerService:OnMessageReceived", data.ToString());
            var title = data.GetString("Title");
            var body = data.GetString("Body");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Title:    " + title);
            Log.Debug("MyGcmListenerService", "Body    " + body);
            Body = body;
            SendNotification(title);
            sendNotificationToServer(title, body);
        }

        private void sendNotificationToServer(string title, string body)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri("http://xamarin21php.lenguyenwi.com/setLog.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("Name", title);
            parameters.Add("Number", body);
            // client.UploadValuesCompleted += client_UploadValuesCompleted;
            client.UploadValuesAsync(uri, parameters);
        }

        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(GCMmainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("LWN")
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetDefaults(NotificationDefaults.Sound)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}