using System;
using System.Linq;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSecurityPage : ContentPage
    {
        public EditSecurityPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var getprofile = (await MobileService.GetTable<TBL_Users>().Where(profile => profile.Id == user_id).ToListAsync()).FirstOrDefault();
            profilelayout.BindingContext = getprofile;
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            bool oldpass = string.IsNullOrEmpty(entreyoldpass.Text);
            bool newpass = string.IsNullOrEmpty(entrynewpass.Text);
            bool newpassconfirm = string.IsNullOrEmpty(entrynewpassconfirm.Text);
            if (oldpass || newpass || newpassconfirm)
            {
                await DisplayAlert("Error", "Please Enter your old password and confirm your new password!", "OK");
                return;
            }
            if (entrynewpass.Text != entrynewpassconfirm.Text)
            {
                await DisplayAlert("Error confirming password", "Password did not match!", "OK");
                return;
            }
            var users = (await MobileService.GetTable<TBL_Users>().Where(mail => mail.emailadd == lblemail.Text).ToListAsync()).FirstOrDefault();
            if (users != null)
            {
                if (users.password == entreyoldpass.Text)
                {
                    var newusersecurity = new TBL_Users
                    {
                        Id = user_id,
                        full_name = fullname,
                        address = address,
                        mobile_num = mobilenum,
                        emailadd = emailadd,
                        password = entrynewpassconfirm.Text,
                        datereg = datereg,
                        propic = propic,
                        picstr = picstr
                    };
                    await TBL_Users.Update(newusersecurity);
                    await DisplayAlert("Info", "Security setting saved!", "OK");
                    await Navigation.PopAsync(true);
                }
                else
                {
                    //indicatorloader.IsVisible = false;
                    await DisplayAlert("Error", "Old password is incorrect!", "OK");
                }
            }
        }
    }
}