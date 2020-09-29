using System;
using GreenApp.Activity;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenApp
{
    public partial class App
    {
        public static readonly MobileServiceClient MobileService = new MobileServiceClient("https://imarketresource.azurewebsites.net");
        public static string Selected_ProdId;
        public static string Selected_CatID;
        public static string user_id;
        public static string CurrentOrderId;
        public static bool refresh;
        public static string MarketStatus;
        public static bool checkout;
        public static bool _conn;
        public static string Selected_orderID;
        public static string fullname, address, mobilenum, emailadd, password, propic;
        public static DateTime datereg;
        public App()
        {
            InitializeComponent();
            Device.SetFlags(new []{"Brush_Experimental"});
            MainPage = new NavigationPage(new LoginPage());
        }
        
        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
