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
    public partial class AddressesPage : ContentPage
    {
        public AddressesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await getUserAddresses();
        }

        private async Task getUserAddresses()
        {
            try
            {
                RefreshView.IsRefreshing = true;
                var getAddresseses = await MobileService.GetTable<TBL_Addresses>().Where(usersId => usersId.user_id == user_id).ToListAsync();
                ListAddress.ItemsSource = getAddresseses;
                RefreshView.IsRefreshing = false;
            }
            catch
            {
                RefreshView.IsRefreshing = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeliveryLocationPage());
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await getUserAddresses();
        }
    }
}