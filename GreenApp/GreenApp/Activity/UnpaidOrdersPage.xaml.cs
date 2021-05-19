using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnpaidOrdersPage : ContentPage
    {
        List<SwipeView> swipeViews { set; get; }
        List<SwipeView> swipViews { set; get; }
        private SwipeView XSwipeViews;
        public UnpaidOrdersPage()
        {
            InitializeComponent();
            //swipeViews = new List<SwipeView>();
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
                var getorders = await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id.ToLower().Contains(user_id) && !orders.stat.ToLower().Contains("2")).ToListAsync();
                OrdersList.ItemsSource = getorders;
                Selected_orderID = null;
                OrdersList.SelectedItem = null;
                xRefreshView.IsRefreshing = false;
            }
            catch
            {
                await this.DisplayToastAsync("A network error occured, please check your internet connectivity and try again.");
                //await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
                xRefreshView.IsRefreshing = false;
            }
        }
        private async void OrdersList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OrdersList.SelectedItem == null) return;
                TrackOrderPage.t_order_id = (e.CurrentSelection.FirstOrDefault() as TBL_Orders)?.id;
                await Navigation.PushModalAsync(new TrackOrderPage());
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
        //private void XSwipeViews_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        //{
        //    if (swipeViews.Count == 1)
        //    {
        //        swipeViews[0].Close();
        //        swipeViews.Remove(swipeViews[0]);
        //    }
        //}

        //private void XSwipeViews_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        //{
        //    if (swipeViews.Count == 1)
        //    {
        //        swipeViews.Add(swipeViews[0]);
        //    }
            
        //}
        private async void SwipeItem_OnInvoked(object sender, EventArgs e)
        {
            checkout = false;
            var item = sender as SwipeItem;
            var model = item?.BindingContext as TBL_Orders;
            if (model != null) Selected_orderID = model.id;
            await Navigation.PushModalAsync(new ConfirmationPage());
        }
    }
}