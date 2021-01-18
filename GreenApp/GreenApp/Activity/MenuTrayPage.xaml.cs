using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            await Navigation.PushAsync(new LoginPage());
        }

        private async void Bbtnsignin_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignupPage());
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
            await Navigation.PushAsync(new AddressesPage(), true);
        }

        private async void Feedback_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FeedBackPage(), true);
        }

        private async void About_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage(), true);
        }

        private void Logout_OnTapped(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}