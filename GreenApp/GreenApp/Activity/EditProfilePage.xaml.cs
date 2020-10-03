using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        public EditProfilePage()
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
            try
            {
                if (entryfullname.Text == "")
                {
                    await DisplayAlert("Error", "Please Enter your full name!", "Ok");
                    entryfullname.Focus();
                }
                else if (entrymobile.Text == "")
                {
                    await DisplayAlert("Error", "Please Enter your mobile number!", "Ok");
                    entrymobile.Focus();
                }
                else if (entryaddress.Text == "")
                {
                    await DisplayAlert("Error", "Please Enter your address!", "Ok");
                    entryaddress.Focus();
                }

                progresssave.IsVisible = true;
                var profile = new TBL_Users
                {
                    Id = user_id,
                    full_name = entryfullname.Text,
                    address = entryaddress.Text,
                    mobile_num = entrymobile.Text,
                    emailadd = emailadd,
                    password = password,
                    datereg = datereg,
                    propic = propic,
                    picstr = picstr
                };
                address = entryaddress.Text;
                await TBL_Users.Update(profile);
                progresssave.IsVisible = false;
                await DisplayAlert("Info", "Profile information updated!", "Ok");
                await Navigation.PopAsync(true);
            }
            catch
            {
                progresssave.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }
    }
}