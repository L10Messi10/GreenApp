﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GreenApp.Activity;
using GreenApp.Models;
using GreenApp.Utils;
using Xamarin.Forms;
using static GreenApp.App;

namespace GreenApp.Droid
{
    [Activity(Theme = "@style/Theme.Splash",
        MainLauncher = true,
        NoHistory = true,
        Icon = "@drawable/logo",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class SplashActivity : Android.App.Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Threading.Thread.Sleep(500);
            StartActivity(typeof(MainActivity));
            // Create your application here
        }
        
    }
}