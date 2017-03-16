using Foundation;
using UIKit;
using Google.InstanceID;
using System;
using Google.GoogleCloudMessaging;
using System.Net;
using System.Collections.Specialized;
using CloudKit;


namespace InstanceIDSample
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IInstanceIdDelegate, IReceiverDelegate
    {

        public override UIWindow Window
        {
            get;
            set;
        }

        private static string tokenRegistration = "hi here is tokenRegistration";
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

        public Google.Core.Configuration Configuration { get; set; }

        NSData DeviceToken { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Log("Finished Launching");

            Log("Starting GCM");

            // Configure our core Google 
            NSError err;
            Google.Core.Context.SharedInstance.Configure(out err);
            if (err != null)
                Console.WriteLine("Failed to configure Google: {0}", err.LocalizedDescription);
            Configuration = Google.Core.Context.SharedInstance.Configuration;

            // Configure and Start GCM
            var gcmConfig = Google.GoogleCloudMessaging.Config.DefaultConfig;
            gcmConfig.ReceiverDelegate = this;

            Service.SharedInstance.Start(gcmConfig);

            Log("Started GCM");

            Log("Registering for Remote Notifications");

            //// Register for remote notifications
            //var notTypes = UIUserNotificationType.Sound | UIUserNotificationType.Alert | UIUserNotificationType.Badge;
            //var settings = UIUserNotificationSettings.GetSettingsForTypes(notTypes, null);
            //UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            //UIApplication.SharedApplication.RegisterForRemoteNotifications();



            // registers for push
            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();



            //// check for a notification
            if (launchOptions != null)
            {

                // check for a local notification
                if (launchOptions.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {

                    UILocalNotification localnotification = launchOptions[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                    if (localnotification != null)
                    {

                        //new UIAlertView(localnotification.AlertAction, localnotification.AlertBody, null, "ok", null).Show();
                        new UIAlertView("lauchOptions localnotification.AlertAction", "lauchOptions localnotification.AlertBody", null, "ok", null).Show();
                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
                // check for a remote notification               
                if (launchOptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
                {
                     NSDictionary remoteNotification = launchOptions[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                     if (remoteNotification != null)
                    //if (launchOptions != null)
                    {
                        alertNotification(application, remoteNotification);
                        //new UIAlertView("basdasdqrwqr", "adasdasd", null, "OK", null).Show();
                        //new UIAlertView("basdasdqrwqr", "adasdasd", null, "OK", null).Show();//remoteNotification["Body"].ToString()
                        //new UIAlertView(remoteNotification.AlertAction, remoteNotification.AlertBody, null, "OK", null).Show();
                    }
                }
            }


            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                                               UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                                           );

                application.RegisterUserNotificationSettings(notificationSettings);
                application.RegisterForRemoteNotifications();
            }
            else
            {
                //==== register for remote notifications and get the device token
                // set what kind of notification types we want
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge;
                // register for remote notifications
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
            //end notifi

            return true;
        }


        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Log("Failed to register for remote notifications {0}", error.LocalizedDescription);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Log("Registered for Remote Notifications: {0}", deviceToken.Description);

            // Save our token in memory for future calls to GCM
            DeviceToken = deviceToken;

            Log("Starting Instance ID");

            // Configure and start Instance ID
            var config = Google.InstanceID.Config.DefaultConfig;
            config.Delegate = this;
            InstanceId.SharedInstance.Start(config);

            Log("Started Instance ID");

            // Get a GCM token
            GetToken();
        }

        [Export("onTokenRefresh")]
        public void OnTokenRefresh()
        {
            Log("Token Refreshed on Server");

            // Token was refreshed, get it again
            GetToken();
        }

        void GetToken()
        {
            // Register APNS Token to GCM
            var options = new NSMutableDictionary();
            options.SetValueForKey(DeviceToken, Constants.RegisterAPNSOption);
            options.SetValueForKey(new NSNumber(true), Constants.APNSServerTypeSandboxOption);

            Log("Get Token");

            // Get our token
            InstanceId.SharedInstance.Token(
                "659320137171",
                Constants.ScopeGCM,
                options,
                (token, error) => //Log("GCM Registration ID: " + token)
                SendRegistrationToAppServer(token)

                );
        }
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            //// Handle the notification here
            //Log("Received Remote Notification");
            //Log("UserInfo: ", userInfo.ToString());
            //// Notify GCM we received the message
            //Service.SharedInstance.AppDidReceiveMessage(userInfo);                       
            alertNotification(application, userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
            Console.WriteLine("have received new Notification in background");
            Console.WriteLine(userInfo.Description);
            

        }
        /*
         //This is the ReceivedNotification
         //
         //      
       */
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {

            if (application.ApplicationState == UIApplicationState.Active)
            {
                alertNotification(application, userInfo);
            }
            else if (application.ApplicationState == UIApplicationState.Background)
            {
                alertNotification(application, userInfo);
            }
            else if (application.ApplicationState == UIApplicationState.Inactive)
            {
                alertNotification(application, userInfo);
            }
        }
        /*
         //This is the arlert notification
         //
         //      
       */
        public void alertNotification(UIApplication application, NSDictionary userInfo)
        {
            Console.WriteLine("Application" + application.ToString());
            //Console.WriteLine("received userInfo.Decription :"+ userInfo.Description);
            Console.WriteLine("received userInfo.ToString received remote : " + userInfo.ToString());
            var title = userInfo["Title"];
            var body = userInfo["Body"];
            Console.WriteLine("title : " + title);
            Console.WriteLine("body : " + body);

            
            //notification to Device
            UILocalNotification notification = new UILocalNotification();
            var fireDate = DateTime.Now.AddSeconds(3);
            notification.FireDate = (NSDate)fireDate;

            //---- configure the alert stuff
            notification.AlertAction = body.ToString();
            notification.AlertBody = title.ToString();

            //---- modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            //---- set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            //				notification.UserInfo = new NSDictionary();
            //				notification.UserInfo[new NSString("Message")] = new NSString("Your 1 minute notification has fired!");

            //---- schedule it
            new UIAlertView("hi this is title", "hi this is message", null, "OK", null).Show();
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        //public override void WillEnterForeground(UIApplication application)
        //{
        //    Console.WriteLine("App will enter foreground");

        //}

        //public override void OnResignActivation(UIApplication application)
        //{
        //    Console.WriteLine("OnResignActivation called, App moving to inactive state.");
        //}

        //public override void DidEnterBackground(UIApplication application)
        //{
        //    Console.WriteLine("App entering background state.");
        //}

        //localnotification

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            new UIAlertView(notification.AlertAction, notification.AlertBody, null, "ok", null).Show();

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            // Connect to the GCM server to receive non-APNS notifications
            Service.SharedInstance.Connect(error => {
                if (error != null)
                {
                    Log("Could not connect to GCM: {0}", error.LocalizedDescription);
                }
                else
                {
                    Log("Connected to GCM");
                }
            });
        }

        //public override void DidEnterBackground(UIApplication application)
        //{
        //    // Disconnect from GCM when we go to background so we'll 
        //    // begin receiving APNS messages again
        //    Service.SharedInstance.Disconnect();
        //    Log("Disconnected why ?");
        //}

        // We can use this to delete our regsitration with GCM
        public void DeleteToken()
        {
            Log("Deleting Token");

            InstanceId.SharedInstance.DeleteToken(
                "659320137171",
                Constants.ScopeGCM,
                error => {
                    // Callback, non-null error if there was a problem
                    if (error != null)
                        Log("Deleted Token");
                    else
                        Log("Error deleting token");
                });
        }

        int messageId = 1;

        // We can send upstream messages back to GCM
        public void SendUpstreamMessage()
        {
            var msg = new NSDictionary();
            msg.SetValueForKey(new NSString("1234"), new NSString("userId"));
            msg.SetValueForKey(new NSString("hello world"), new NSString("msg"));

            var to = "659320137171" + "@gcm.googleapis.com";

            Log("Sending Message to: {0}", to);
            Service.SharedInstance.SendMessage(msg, to, (messageId++).ToString());
        }

        // Implement some messages to receive notifications about upstream message callbacks
        [Export("didDeleteMessagesOnServer")]
        public void DidDeleteMessagesOnServer()
        {
            Log("Did Delete Messages on Server");
        }

        [Export("didSendDataMessageWithID:")]
        public void DidSendDataMessage(string messageID)
        {
            Log("Did Send Message: {0}", messageId);
        }

        [Export("willSendDataMessageWithID:error:")]
        public void WillSendDataMessage(string messageID, NSError error)
        {
            Log("Will Send Message: {0}", messageId);
        }

        public static event Action<string> LoggedMessage;

        static void Log(string messageFormat, params object[] args)
        {
            var msg = string.Format(messageFormat, args);

            Console.WriteLine(msg);

            var evt = LoggedMessage;
            if (evt != null)
                evt(msg);
        }
        void SendRegistrationToAppServer(string token)
        {
            // Add custom implementation here as needed.
            TokenRegistration = token;
            Console.WriteLine(TokenRegistration);
            WebClient client = new WebClient();
            //Uri uri = new Uri("http://xamarin21php.lenguyenwi.com/sendTokenToCCServer.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("CompanyId", "3");
            parameters.Add("PushId", token);
            // client.UploadValuesCompleted += client_UploadValuesCompleted;
            //// client.UploadValuesAsync(uri, parameters);
        }


        }
}


