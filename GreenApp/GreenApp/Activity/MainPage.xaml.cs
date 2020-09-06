using System;
using System.ComponentModel;
using System.Threading.Tasks;
using GreenApp.Animation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        public MainPage() => InitializeComponent();

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.None)
            {
                await DisplayAlert("Internet", "Please check your internet connectivity!", "OK");
            }
        }

        private async void Btnlogin_OnClicked(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.None)
            {
                await DisplayAlert("Internet", "Please check your internet connectivity!", "OK");
            }
            else
            {
                await Navigation.PushAsync(new LoginPage());
            }
            
        }

        private void Btnsignup_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignupPage());
        }
    }
}
