using System;
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
using GreenApp.Models;
using System.Threading.Tasks;
using GreenApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.Activity.AddressesPage;
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
            System.Threading.Thread.Sleep(1000);
            StartActivity(typeof(MainActivity));
            // Create your application here
            CheckForAutoLogin();
        }
        private async void CheckForAutoLogin()
        {
            try
            {
                if (Settings.LastUsedEmail != string.Empty)
                {
                    var users = (await MobileService.GetTable<TBL_Users>().Where(mail => mail.emailadd == Settings.LastUsedEmail).ToListAsync()).FirstOrDefault();
                    if (users != null)
                    {
                        user_id = users.Id;
                        fullname = users.full_name;
                        mobilenum = users.mobile_num;
                        emailadd = users.emailadd;
                        password = users.password;
                        datereg = users.datereg;
                        propic = users.propic;
                        picstr = users.picstr;
                        //user_id = null;
                        CurrentOrderId = null;
                        refresh = false;
                        SignedIn = true;
                        hasnetwork = true;
                        var getAddresses = (await MobileService.GetTable<TBL_Addresses>().Where(p => p.user_id == user_id).ToListAsync()).FirstOrDefault();
                        if (getAddresses != null)
                        {
                            _selectedAddressId = getAddresses.id;
                            order_long = getAddresses.add_long;
                            order_lat = getAddresses.add_lat;
                            order_rcvr_add = getAddresses.Address;
                            build_name = getAddresses.building_name;
                            order_notes = getAddresses.Notes;
                        }
                    }
                }
                else
                {
                    propic = null;
                    picstr = null;
                    hasnetwork = true;
                    SignedIn = false;
                    //Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = new LoginPage(); });
                    //await Navigation.PushAsync(new LoginPage(), true);
                }
            }
            catch (Exception e)
            {
                propic = null;
                picstr = null;
                hasnetwork = false;
                SignedIn = false;
            }

        }

    }
}