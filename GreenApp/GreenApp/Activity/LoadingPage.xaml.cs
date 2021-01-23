using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using GreenApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();

            CheckForAutoLogin();
        }
        private async void CheckForAutoLogin()
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
                    //indicatorloader.IsVisible = false;
                    //Settings.LastUsedEmail = chkremember.IsChecked ? emailentry.Text : "";
                    //await DisplayAlert("Success", "Email or password is incorrect!", "OK");
                    Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = new AppShell(); });
                    await Navigation.PushAsync(new MenuPage(), true);
                    //var page = MenuPage as NavigationPage;
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = new LoginPage(); });
                await Navigation.PushAsync(new LoginPage(), true);
            }
        }
    }
}