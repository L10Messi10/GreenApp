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
    public partial class TrackOrderPage : ContentPage
    {
        public static string t_order_id;
        public TrackOrderPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await XTracking();
        }

        private async Task XTracking()
        {
            try
            {
                xRefreshView.IsRefreshing = true;
                var getTrackOrders = await MobileService.GetTable<V_OrderTracking>().Where(tOId => tOId.order_id == t_order_id).ToListAsync();
                ListTrack.ItemsSource = getTrackOrders.OrderByDescending(i => i.track_num);
                OrderLayout.BindingContext = getTrackOrders.FirstOrDefault();
                xRefreshView.IsRefreshing = false;
            }
            catch
            {
                await this.DisplayToastAsync("A network error occured, please check your internet connectivity and try again.");
                //await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
                xRefreshView.IsRefreshing = false;
            }
            
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            t_order_id = "";
            await Navigation.PopModalAsync();
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await XTracking();
        }
    }
}