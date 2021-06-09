using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenApp.Constants;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;

namespace GreenApp.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
    [MetaData("com.google.android.maps.v2.API_KEY",
        Value = AppConstants.GoogleMapsApiKey)]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) :
            base(handle, transfer)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Base)
            {
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
                FirebasePushNotificationManager.DefaultNotificationChannelImportance = NotificationImportance.Max;

            }
#if DEBUG
            FirebasePushNotificationManager.Initialize(this,true);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif




            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {

            };
        }
    }
}