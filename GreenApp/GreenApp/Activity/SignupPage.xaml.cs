using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage
    {
        public SignupPage()
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
        private async void Btnregister_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (NameEntry.Text == null)
                {
                    await DisplayAlert("Field required", "Please enter you full name. This will be used upon checking out items or picking them up.", "OK");
                    NameEntry.Focus();
                    return;
                }

                if (mobileentry.Text == null)
                {
                    await DisplayAlert("Field required", "Please enter you mobile number.", "OK");
                    mobileentry.Focus();
                    return;
                }
                var mynumBer = mobileentry.Text;
                if (mynumBer.Length != 11)
                {
                    await DisplayAlert("Error", "Please Enter a valid 11 digit mobile number! Ex. 09101234578!", "Ok");
                    mobileentry.Focus();
                    return;
                }

                if (emailentry.Text == null)
                {
                    await DisplayAlert("Field required", "Please enter your email address", "OK");
                    emailentry.Focus();
                    return;
                }

                if (passentry.Text == null)
                {
                    await DisplayAlert("Field required", "Please enter your password", "OK");
                    passentry.Focus();
                    return;
                }

                if (confirmpassentry.Text == null)
                {
                    await DisplayAlert("Field required", "Please confirm your password", "OK");
                    confirmpassentry.Focus();
                    return;
                }

                if (lblerror.IsVisible)
                {
                    await DisplayAlert("Error", "Invalid email address!", "OK");
                    emailentry.Focus();
                    return;
                }

                if (loadingindicator.IsVisible) return;
                if (passentry.Text == confirmpassentry.Text)
                {
                    var user = new TBL_Users
                    {
                        full_name = NameEntry.Text,
                        mobile_num = mobileentry.Text,
                        emailadd = emailentry.Text,
                        password = passentry.Text,
                        datereg = DateTime.Now
                    };
                    loadingindicator.IsVisible = true;
                    loadingindicator.IsRunning = true;
                    indicatornot.IsVisible = true;
                    await TBL_Users.Insert(user);
                    await DisplayAlert("Success", "You've successfully signed up! Please login to your account now!", "OK");
                    indicatornot.IsVisible = false;
                    loadingindicator.IsVisible = false;
                    await Navigation.PopModalAsync(true);
                }
                else
                {
                    await this.DisplayToastAsync( "Password did not match!");
                    confirmpassentry.Focus();
                }
            }
            catch
            {
                await this.DisplayToastAsync( "There was an error processing your request. " +
                                            "The email address you've entered already exist. Please try another one. " +
                                            "Please check you internet connectivity as well.", 5000);
                indicatornot.IsVisible = false;
                loadingindicator.IsVisible = false;
            }
        }

        private void Emailentry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var email = emailentry.Text;

            var emailPattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";
            lblerror.IsVisible = !Regex.IsMatch(email, emailPattern);
        }
    }
}