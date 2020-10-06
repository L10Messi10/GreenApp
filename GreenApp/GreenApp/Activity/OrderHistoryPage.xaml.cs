using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    public partial class OrderHistoryPage : ContentPage
    {
        public OrderHistoryPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            //refresh = false;
        }

        protected override async void OnAppearing()
        {
            try
            {
                string ord = "1";
                if (historyloaded == false)
                {
                    var getorders = await MobileService.GetTable<TBL_Orders>().Where(orders => orders.stat.Contains(ord) && orders.users_id.ToLower().Contains(user_id)).ToListAsync();
                    OrdersList.ItemsSource = getorders;
                    historyloaded = true;
                }
                OrderDetailsList.ItemsSource = null;
                lblsum.Text = "Php. 0.00";
                CurrentOrderId = null;
                Selected_orderID = null;
                OrdersList.SelectedItem = null;
            }
            catch
            {
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
            
        }

        private async void OrdersList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Selected_orderID = (e.CurrentSelection.FirstOrDefault() as TBL_Orders)?.id;
                await XGetOrders();
            }
            catch
            {
                //ignored
            }
        }

        private async Task XGetOrders()
        {
            try
            {
                var getorderdetails = (await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == Selected_orderID).ToListAsync());
                OrderDetailsList.ItemsSource = getorderdetails;
                var amount = getorderdetails.Sum(p => p.sub_total).ToString("N");
                //AnimateText();
                lblsum.Text = "Php. " + amount;
            }
            catch
            {
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private void AnimateAmount()
        {
            var stw = new Stopwatch();
            stw.Start();
            Device.StartTimer(TimeSpan.FromSeconds(1 / 100f), () =>
            {
                double t = stw.Elapsed.TotalMilliseconds % 500 / 500;
                return true;
            });
        }

        private void Reload_OnClicked(object sender, EventArgs e)
        {
            historyloaded = false;
            OnAppearing();
        }

        private async void Viewcode_OnClicked(object sender, EventArgs e)
        {
            if(Selected_orderID == null) return;
            checkout = false;
            await Navigation.PushAsync(new ConfirmationPage(), true);
        }
    }
}