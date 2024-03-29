﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        public EditProfilePage()
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
                lblprogressstat.Text = "Loading profile . . .";
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
                if (entryfullname.Text == null)
                {
                    await DisplayAlert("Error", "Please Enter your full name!", "Ok");
                    entryfullname.Focus();
                    return;
                }

                if (entrymobile.Text == null)
                {
                    await DisplayAlert("Error", "Please Enter your mobile number!", "Ok");
                    entrymobile.Focus();
                    return;
                }

                var mynumBer = entrymobile.Text;
                if (mynumBer.Length != 11)
                {
                    await DisplayAlert("Error", "Please Enter a valid 11 digit mobile number! Ex. 09101234578!", "Ok");
                    entrymobile.Focus();
                    return;
                }
                progresssave.IsVisible = true;
                lblprogressstat.Text = "Saving profile . . .";
                var profile = new TBL_Users
                {
                    Id = user_id,
                    full_name = entryfullname.Text,
                    mobile_num = entrymobile.Text,
                    emailadd = emailadd,
                    password = password,
                    datereg = datereg,
                    propic = propic,
                    picstr = picstr
                };
                fullname = entryfullname.Text;
                await TBL_Users.Update(profile);
                await this.DisplayToastAsync("Profile information updated!");
                progresssave.IsVisible = false;
                //await DisplayAlert("Info", "Profile information updated!", "Ok");
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