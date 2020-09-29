using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                var getorders = (await App.MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id == App.user_id).ToListAsync());
                OrdersList.ItemsSource = getorders;
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
        }

        private async void Btnbrowseimage_OnClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

        }
    }
}