using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.Widget;
using System.Net;
using System.Collections.Specialized;

namespace RemoteNotificationsSelfBuilded
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        private static string tokenRegistration = "hi here is tokenRegistration";
        public string test = "Hello this is a test";
        static object locker = new object();

        public static string TokenRegistration
        {
            get
            {
                return tokenRegistration;
            }

            set
            {
                tokenRegistration = value;
            }
        }

        public RegistrationIntentService() : base("RegistrationIntentService") { }



        protected override void OnHandleIntent(Intent intent)
        {


            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        "659320137171", GoogleCloudMessaging.InstanceIdScope, null);
                    
                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    
                    
                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                    

                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }


        void SendRegistrationToAppServer(string token)
        {
            //           // Add custom implementation here as needed.
            TokenRegistration = token;
            WebClient client = new WebClient();
            Uri uri = new Uri("http://gcmwebservice.lenguyenwi.com/SendTokenToDB.php");
            //Uri uri = new Uri("http://192.168.127.10:8447/rest/Logging/RegisterPushId?v=1.2&key=0F5C05060E51D7595992761431C43ADD0171C18068A5ECD3C391816B84A1A63D");
            NameValueCollection parameters = new NameValueCollection();     
            parameters.Add("Name", "3");
            parameters.Add("Number", token);
            // client.UploadValuesCompleted += client_UploadValuesCompleted;
            client.UploadValuesAsync(uri, parameters);
            

        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}