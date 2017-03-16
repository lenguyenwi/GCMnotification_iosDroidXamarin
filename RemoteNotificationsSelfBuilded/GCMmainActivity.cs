using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;

namespace RemoteNotificationsSelfBuilded
{
    [Activity(Label = "lNW", MainLauncher = true, Icon = "@drawable/icon")]
    public class GCMmainActivity : Activity
    {
        public static Token TokenObject;
        TextView msgText;
        TextView tvBody;
 
        


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            msgText = FindViewById<TextView>(Resource.Id.msgText);
            tvBody = FindViewById<TextView>(Resource.Id.tvBody);


            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));               
                StartService(intent);
                // String token = RegistrationIntentService.TokenRegistration;
                String body = MyGcmListenerService.Body;
                tvBody.Text =body;
                
            }
            

        }


        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText.Text = "Sorry, this device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                //msgText.Text = "Google Play Services is available.";
                return true;
            }
        }

    }
}

