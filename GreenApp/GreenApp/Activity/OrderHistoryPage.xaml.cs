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
        List<SwipeView> swipeViews { set; get; }
        public OrderHistoryPage()
        {
            InitializeComponent();
            swipeViews = new List<SwipeView>();
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
                var getorders = await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id.ToLower().Contains(user_id) && orders.stat.ToLower().Contains("2")).ToListAsync();
                OrdersList.ItemsSource = getorders;
                Selected_orderID = null;
                OrdersList.SelectedItem = null;
                xRefreshView.IsRefreshing = false;
            }
            catch
            {
                xRefreshView.IsRefreshing = false;
                await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
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
            try
            {
                if (OrdersList.SelectedItem == null) return;
                Selected_orderID = (e.CurrentSelection.FirstOrDefault() as TBL_Orders)?.id;
                //await XGetOrders();
            }
            catch
            {
               //ignored
            }
            
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await getHistoryOrders();
        }

        private async void SwipeItemdeLete_OnInvoked(object sender, EventArgs e)
        {
            try
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
                await getHistoryOrders();
            }
            catch
            {
                await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
            }
        }

        private void XSwipeViews_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            if (swipeViews.Count == 1)
            {
                swipeViews[0].Close();
                swipeViews.Remove(swipeViews[0]);
            }
        }

        private void XSwipeViews_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            swipeViews.Add(new SwipeView());
        }
    }
}