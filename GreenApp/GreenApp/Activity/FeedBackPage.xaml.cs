using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedBackPage : ContentPage
    {
        public FeedBackPage()
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

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (entryfeedback.Text != null)
                {
                    progressLoading.IsVisible = true;
                    var feedback = new TBL_Feedback
                    {
                        response = entryfeedback.Text
                    };
                    await TBL_Feedback.Insert(feedback);
                    await this.DisplayToastAsync("Your feedback has been successfully submitted, We'll review your feedback to improve the application. Thank you!",5000);
                    //await DisplayAlert("Info", "!", "OK");
                    entryfeedback.Text = "";
                    progressLoading.IsVisible = false;
                }
                else
                {
                    await DisplayAlert("Error", "Please Enter your feedback!", "OK");
                    entryfeedback.Focus();
                }
            }
            catch
            {
                progressLoading.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(),true);
            }
        }
    }
}