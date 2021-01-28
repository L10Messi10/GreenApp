using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderHistoryPage : ContentPage
    {
        public OrderHistoryPage()
        {
            Device.SetFlags(new []{"SwipeView_Experimental"}); // Add here
            InitializeComponent();
        }
        public ICommand OnDeleteCommand => new Command(OnDelete);
        private async void OnDelete()
        {
            user_id = null;
            CurrentOrderId = null;
            refresh = false;
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            await Navigation.PushAsync(new LoginPage(), true);
        }
        protected override async void OnAppearing()
        {
            await getHistoryOrders();
        }

        private async Task getHistoryOrders()
        {
            try
            {
                xRefreshView.IsRefreshing = true;
                var getorders = await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id.ToLower().Contains(user_id)).ToListAsync();
                OrdersList.ItemsSource = getorders;
                Selected_orderID = null;
                OrdersList.SelectedItem = null;
                xRefreshView.IsRefreshing = false;
            }
            catch
            {
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async void Viewcode_OnClicked(object sender, EventArgs e)
        {
            if(Selected_orderID == null) return;
            checkout = false;
            await Navigation.PushAsync(new ConfirmationPage(), true);
        }

        private void OrdersList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersList.SelectedItem == null) return;
            Selected_orderID = (e.CurrentSelection.FirstOrDefault() as TBL_Orders)?.id;
            //await XGetOrders();
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await getHistoryOrders();
        }

        private async void SwipeItemdeLete_OnInvoked(object sender, EventArgs e)
        {
            var item = sender as SwipeItem;
            var model = item.BindingContext as TBL_Orders;

            var ans = await DisplayAlert("Delete", "Are you sure to remove this order?", "Yes", "No");
            if (!ans) return;
            var orderDetails = new TBL_Orders()
            {
                id = model.id,
            };
            await TBL_Orders.Void(orderDetails);
            var getorders = await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id.ToLower().Contains(user_id)).ToListAsync();
            OrdersList.ItemsSource = getorders;
        }
    }
}