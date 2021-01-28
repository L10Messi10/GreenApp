using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.Activity.DeliveryLocationPage;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressesPage : ContentPage
    {
        public static string _selectedAddressId;
        public static bool _CheckingOut;
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
                if (getAddresseses.Count == 0)
                {
                    imgnofound.IsVisible = true;
                    ListAddress.IsVisible = false;
                }
                else
                {
                    imgnofound.IsVisible = false;
                    ListAddress.IsVisible = true;
                }
            }
            catch
            {
                RefreshView.IsRefreshing = false;
                await Navigation.PushAsync(new NoInternetPage(), false);
            }
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            _newAdd = true;
            await Navigation.PushAsync(new DeliveryLocationPage(),false);
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await getUserAddresses();
        }

        private async void ListAddress_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListAddress.SelectedItem == null) return;
            _selectedAddressId = (e.CurrentSelection.FirstOrDefault() as TBL_Addresses)?.id;
            if (_CheckingOut)
            {
                var getAddresses = (await MobileService.GetTable<TBL_Addresses>().Where(p => p.id == _selectedAddressId).ToListAsync()).FirstOrDefault();
                if (getAddresses != null)
                {
                    _selectedAddressId = getAddresses.id;
                    order_long = getAddresses.add_long;
                    order_lat = getAddresses.add_lat;
                    order_rcvr_add = getAddresses.Address;
                    order_notes = getAddresses.Notes;
                }
                _CheckingOut = false;
                await Navigation.PopAsync(false);
            }
        }

        private async void Deleteitem_OnClicked(object sender, EventArgs e)
        {
            if (_selectedAddressId != null)
            {
                var addresses = new TBL_Addresses()
                {
                    id = _selectedAddressId,
                };
                await TBL_Addresses.Remove(addresses);
                await getUserAddresses();
            }
            
        }

        private async void Edititem_OnClicked(object sender, EventArgs e)
        {
            if (_selectedAddressId != null)
            {
                _newAdd = false;
                await Navigation.PushAsync(new DeliveryLocationPage());
            }
        }
    }
}