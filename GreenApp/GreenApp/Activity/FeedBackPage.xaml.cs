using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedBackPage : ContentPage
    {
        public FeedBackPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (entryfeedback.Text != null)
                {
                    var feedback = new TBL_Feedback
                    {
                        response = entryfeedback.Text
                    };
                    await TBL_Feedback.Insert(feedback);
                    await DisplayAlert("Info", "Your feedback has been successfully submitted, We'll review your feedback to improve the application. Thank you!!", "OK");
                    entryfeedback.Text = "";
                }
                else
                {
                    await DisplayAlert("Error", "Please Enter your feedback!", "OK");
                    entryfeedback.Focus();
                }
            }
            catch
            {
                await Navigation.PushAsync(new NoInternetPage(),true);
            }
        }
    }
}