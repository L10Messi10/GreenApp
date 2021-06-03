using System;
using System.Linq;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;
using Xamarin.Essentials;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSecurityPage : ContentPage
    {
        public EditSecurityPage()
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

        protected override async void OnAppearing()
        {
            try
            {
                progresssave.IsVisible = true;
                lblstat.Text = "Loading security page. . .";
                var getprofile = (await MobileService.GetTable<TBL_Users>().Where(profile => profile.Id == user_id).ToListAsync()).FirstOrDefault();
                profilelayout.BindingContext = getprofile;
                progresssave.IsVisible = false;
            }
            catch
            {
                progresssave.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
                
            }
            
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var oldpass = string.IsNullOrEmpty(entreyoldpass.Text);
                var newpass = string.IsNullOrEmpty(entrynewpass.Text);
                var newpassconfirm = string.IsNullOrEmpty(entrynewpassconfirm.Text);
                if (oldpass || newpass || newpassconfirm)
                {
                    await DisplayAlert("Error", "Please Enter your old password and confirm your new password!", "OK");
                    return;
                }
                if (entrynewpass.Text != entrynewpassconfirm.Text)
                {
                    await DisplayAlert("Confirm password", "New Password did not match!", "OK");
                    return;
                }

                progresssave.IsVisible = true;
                lblstat.Text = "Saving. . .";
                var users = (await MobileService.GetTable<TBL_Users>().Where(usr => usr.emailadd == lblemail.Text).ToListAsync()).FirstOrDefault();
                if (users == null) return;
                if (users.password == entreyoldpass.Text)
                {
                    var newusersecurity = new TBL_Users
                    {
                        Id = user_id,
                        full_name = fullname,
                        mobile_num = mobilenum,
                        emailadd = emailadd,
                        password = entrynewpassconfirm.Text,
                        datereg = datereg,
                        propic = propic,
                        picstr = picstr
                    };
                    await TBL_Users.Update(newusersecurity);
                    await this.DisplayToastAsync("Security setting saved!");
                    progresssave.IsVisible = false;
                    //await DisplayAlert("Info", "Security setting saved!", "OK");
                    await Navigation.PopAsync(true);
                }
                else
                {
                    
                    await this.DisplayToastAsync("Old password is incorrect!");
                    progresssave.IsVisible = false;
                    //await DisplayAlert("Error", "Old password is incorrect!", "OK");
                    entreyoldpass.Focus();
                }
            }
            catch
            {
                progresssave.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }
    }
}