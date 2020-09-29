using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Toasts;
using Xamarin.Forms;

namespace GreenApp.Droid
{
    [Activity(Label = "GreenApp", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
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
            DependencyService.Register<ToastNotification>(); // Register your dependency
            // If you are using Android you must pass through the activity
            ToastNotification.Init(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.SetFlags("UseLegacyRenderers");
            Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this,savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            FormsControls.Droid.Main.Init(this);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            RequestPermissions(permissionGroup,0);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}