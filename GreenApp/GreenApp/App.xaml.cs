using System;
using GreenApp.Activity;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenApp
{
    public partial class App : Application
    {
        public static readonly MobileServiceClient MobileService = new MobileServiceClient("https://imarketresource.azurewebsites.net");
        public static string Selected_ProdId;
        public static string Selected_CatID;
        public static string user_id;
        public static string CurrentOrderId;
        public static bool refresh;
        public static string MarketStatus;
        public static bool checkout;
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
