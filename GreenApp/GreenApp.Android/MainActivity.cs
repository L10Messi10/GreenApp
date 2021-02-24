using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Gms.Common.Apis;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Google.Android.Material.Snackbar;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using TouchEffect.Android;
using Xamarin.Forms;
using static Plugin.CurrentActivity.CrossCurrentActivity;

namespace GreenApp.Droid
{
    [Activity(Label = "GreenApp", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.SetFlags("SwipeView_Experimental"); // Add here
            Current.Init(this, savedInstanceState);
            //Initialize(ApplicationContext, "ca-app-pub-7879306170422036/6877254608");
            Forms.SetFlags("UseLegacyRenderers");
            Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this,savedInstanceState);
            TouchEffectPreserver.Preserve();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Essentials.Platform.Init(Application);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            RequestPermissions(permissionGroup,0);
            MobileAds.Initialize(ApplicationContext);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //if (requestCode == REQUEST_LOCATION)
            //{
            //    // Received permission result for camera permission.
            //    Log.Info("", "Received response for Location permission request.");

            //    // Check if the only required permission has been granted
            //    if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
            //    {
            //        // Location permission has been granted, okay to retrieve the location of the device.
            //        Log.Info("", "Location permission has now been granted.");
            //        //Snackbar.Make(layout, Resource.String.permission_available_camera, Snackbar.LengthShort).Show();
            //    }
            //    else
            //    {
            //        Log.Info("", "Location permission was NOT granted.");
            //        //Snackbar.Make("", Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
            //    }
            //}
            //else
            //{
            //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //}
        }
        
    }
}