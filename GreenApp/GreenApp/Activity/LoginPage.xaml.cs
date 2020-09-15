using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Animation;
using GreenApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

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
            try
            {
                var stat = (await MobileService.GetTable<TBL_MarketStatus>().ToListAsync()).FirstOrDefault();
                if (stat != null) MarketStatus = stat.status;
                //await Navigation.PushModalAsync(new MenuPage(),true);
                bool isemailempty = string.IsNullOrEmpty(emailentry.Text);
                bool ispasswordempty = string.IsNullOrEmpty(passentry.Text);
                if (isemailempty || ispasswordempty)
                {
                    indicatorloader.IsVisible = false;
                    await DisplayAlert("Error", "Please enter your email and password!", "OK");
                }
                else
                {
                    indicatorloader.IsVisible = true;
                    var users = (await MobileService.GetTable<TBL_Users>().Where(mail => mail.emailadd == emailentry.Text).ToListAsync()).FirstOrDefault();
                    if (users != null)
                    {
                        if (users.password == passentry.Text)
                        {
                            if (MarketStatus == "1")
                            {
                                user_id = users.Id;
                                refresh = false;
                                indicatorloader.IsVisible = false;
                                //await DisplayAlert("Success", "Email or password is incorrect!", "OK");
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Application.Current.MainPage = new AppShell();
                                });
                                await Navigation.PushAsync(new MenuPage(), true);
                            }
                            else
                            {
                                indicatorloader.IsVisible = false;
                                await DisplayAlert("Close", "Green Market is currently closed. Please try again later ", "OK");
                            }
                        }
                        else
                        {
                            indicatorloader.IsVisible = false;
                            await DisplayAlert("Error", "Email or password is incorrect!", "OK");
                        }
                    }
                    else
                    {
                        indicatorloader.IsVisible = false;
                        await DisplayAlert("Error", "There was an error logging you in! Please check the information you're entering.", "OK");
                    }
                }
            }
            catch
            {
                indicatorloader.IsVisible = false;
                await DisplayAlert("Error", "There was an error logging you in! Please check your internet connection.", "OK");
            }
        }

        private async void Btnsignup_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignupPage(),true);
        }
    }
}