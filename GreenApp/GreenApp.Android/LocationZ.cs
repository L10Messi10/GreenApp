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
using Android.Locations;
using GreenApp.Droid;
using GreenApp.Utils;

[assembly: Xamarin.Forms.Dependency(typeof(LocationZ))]
namespace GreenApp.Droid
{
    public class LocationZ : ILocSettings
    {
        public void OpenSettings()
        {
            LocationManager LM = (LocationManager)Application.Context.GetSystemService(Context.LocationService);


            if (LM.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {


                Context ctx = Application.Context;
                ctx.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));


            }
            else
            {
                //this is handled in the PCL
            }
        }
    }
}