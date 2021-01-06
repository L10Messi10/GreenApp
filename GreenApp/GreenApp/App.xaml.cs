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
        public static readonly MobileServiceClient MobileService = new MobileServiceClient("https://greenmarketwebapp.azurewebsites.net");
        //Transaction info
        public static string Selected_ProdId;
        public static string Selected_CatID;
        public static string user_id;
        public static string CurrentOrderId;
        public static bool refresh;
        public static string MarketStatus;
        public static bool checkout;
        public static string Selected_orderID;
        //Customer info
        public static string fullname, address, mobilenum, emailadd, password, propic,picstr;
        public static DateTime datereg;
        public static bool historyloaded = false;
        //Order delivery info
        public static double order_lat;
        public static double order_long;
        public static string order_choice;
        public static string order_rcvr_name;
        public static string order_rcvr_add;
        public static string order_rcvr_num;

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
