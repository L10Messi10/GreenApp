using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuTrayPage : ContentPage
    {
        public MenuTrayPage()
        {
            InitializeComponent();
            getLoginSetting();
        }

        private void getLoginSetting()
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
            }
        }

        private async void Btnlogin_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(), false);
        }

        private async void Bbtnsignin_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignupPage(), false);
        }

        private async void Profile_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(), false);
        }

        private async void Addresses_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddressesPage(), false);
        }

        private async void Orders_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrderHistoryPage(), false);
        }

        private async void Feedback_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FeedBackPage(), false);
        }

        private async void About_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage(), false);
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
            propic = "";
            picstr = "";
            CurrentOrderId = null;
            refresh = false;
            AddressesPage._selectedAddressId = "";
            getLoginSetting();
            //throw new NotImplementedException();
        }
    }
}