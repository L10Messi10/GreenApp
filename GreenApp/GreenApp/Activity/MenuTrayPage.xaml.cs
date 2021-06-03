using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using GreenApp.Utils;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuTrayPage : ContentPage
    {
        public MenuTrayPage()
        {
            InitializeComponent();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.None)
            {
                this.DisplayToastAsync("You have no internet connection.");
            }
            else if (e.NetworkAccess == NetworkAccess.Internet)
            {
                this.DisplayToastAsync("You internet connection was restored.");
            }
        }

        protected override void OnAppearing()
        {
            GetLoginSetting();
        }

        private void GetLoginSetting()
        {
            if (SignedIn)
            {
                profpic.IsVisible = true;
                userfullname.IsVisible = true;
                profpic.Source = propic;
                userfullname.Text = fullname;
                accessLayout.IsVisible = false;
                menuAddresses.IsVisible = true;
                menuLogout.IsVisible = true;
                menuProfile.IsVisible = true;
                menuOrders.IsVisible = true;
                menuOrdersunpaid.IsVisible = true;
            }
            else
            {
                userfullname.IsVisible = false;
                accessLayout.IsVisible = true;
                profpic.IsVisible = false;
                menuAddresses.IsVisible = false;
                menuLogout.IsVisible = false;
                menuProfile.IsVisible = false;
                menuOrders.IsVisible = false;
                menuOrdersunpaid.IsVisible = false;
            }

        }

        private async void Btnlogin_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(), false);
        }

        private async void Bbtnsignin_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignupPage(), true);
        }

        private async void Profile_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(), true);
        }

        private async void Addresses_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddressesPage(), true);
        }

        private async void Orders_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrderHistoryPage(), true);
        }

        private async void Feedback_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FeedBackPage(), true);
        }

        private async void About_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage(), true);
        }

        private async void Logout_OnTapped(object sender, EventArgs e)
        {
            bool ans;
            ans = await DisplayAlert("Logout", "Are you sure to to Logout?", "Yes", "No");
            if (!ans) return;
            SignedIn = false;
            CurrentOrderId = "";
            Settings.LastUsedEmail = "";
            user_id = "";
            fullname = "";
            mobilenum = "";
            emailadd = "";
            password = "";
            propic = null;
            picstr = null;
            CurrentOrderId = null;
            refresh = false;
            AddressesPage._selectedAddressId = "";
            GetLoginSetting();
            //throw new NotImplementedException();
        }

        private async void UnpaidOrders_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UnpaidOrdersPage(), true);
        }

        private async void Home_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}