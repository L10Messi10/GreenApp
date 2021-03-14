using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Android.Locations;
using GreenApp.Activity;
using GreenApp.Droid;
using GreenApp.Utils;
using static GreenApp.App;
using Xamarin.Forms.Xaml;

[assembly: Xamarin.Forms.Dependency(typeof(LocationZ))]
namespace GreenApp.Droid
{
    public class LocationZ : ILocSettings
    {
        public void OpenSettings()
        {
            LocationManager lm = Application.Context.GetSystemService(Context.LocationService) as LocationManager;

            if (lm != null && lm.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                //Show_Dialog msg1 = new Show_Dialog(this);
                //AlertDialog.Builder alertDialogue = new AlertDialog.Builder();
                //location_service = false;
                Toast.MakeText(Application.Context, "Please TURN ON location service to detect your location.", ToastLength.Long)?.Show();
                Context ctx = Application.Context;
                ctx.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            }
            else
            {
                Toast.MakeText(Application.Context, "Location service is active", ToastLength.Long)?.Show();
                //this is handled in the PCL
            }

        }
    }
}