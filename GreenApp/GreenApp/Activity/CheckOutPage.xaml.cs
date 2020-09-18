using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.DateTime;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckOutPage : ContentPage
    {
        private double totaSum;
        private string itemid;
        public CheckOutPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                if (CurrentOrderId != null)
                {
                    var getorders = await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == CurrentOrderId).ToListAsync();
                    ordercollection.ItemsSource = getorders;
                    //ordercollection.ItemsSource.re
                    totaSum = getorders.AsQueryable().Sum(ord => ord.sub_total);
                    lblsubtotal.Text = totaSum.ToString(CultureInfo.InvariantCulture);
                    totalpayable.Text = totaSum.ToString(CultureInfo.InvariantCulture);
                    itemid = null;
                    Selected_ProdId = null;
                }
                else
                {
                    lblsubtotal.Text = "0";
                    totalpayable.Text = "0";
                    itemid = null;
                    Selected_ProdId = null;
                }
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
            
        }

        private async void Btncheckout_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (totalpayable.Text != "0")
                {
                    var stat = (await MobileService.GetTable<TBL_MarketStatus>().ToListAsync()).FirstOrDefault();
                    if (stat != null) MarketStatus = stat.status;
                    if (MarketStatus == "1")
                    {
                        var answer = await DisplayAlert("Confirm", "Do you want to confirm this order?", "Yes", "No");
                        if (!answer) return;
                        var orderDetails = new TBL_Orders()
                        {
                            id = CurrentOrderId,
                            users_id = user_id,
                            order_date = Now.ToString("yyyy-MM-dd"),
                            stat = "1",
                            order_status = "Ordered",
                            tot_payable = totaSum.ToString(CultureInfo.InvariantCulture)
                        };
                        await TBL_Orders.Update(orderDetails);
                        checkout = true;
                        await Navigation.PushAsync(new ConfirmationPage(), true);
                    }
                    else
                    {
                        await DisplayAlert("Close", "Green Market is currently closed. Please try again later. You can place your order later when the Market is open. Thank you.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Cart empty", "Your cart is empty!", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
        }

        private void Ordercollection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                itemid = (e.CurrentSelection.FirstOrDefault() as V_Orders)?.id;
                Selected_ProdId = (e.CurrentSelection.FirstOrDefault() as V_Orders)?.prod_id;
            }
            catch
            {
                //ignored
            }
        }

        private async void Deleteitem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (itemid != null)
                {
                    var confirm = await DisplayAlert("Remove", "Do you really want to remove this item on your cart?", "Yes", "No");
                    if (!confirm) return;
                    var order_details = new TBL_Order_Details
                    {
                        id = itemid,
                    };
                    await TBL_Order_Details.Delete(order_details);
                    itemid = null;
                    Selected_ProdId = null;
                    OnAppearing();
                }
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
            
        }

        private async void Edititem_OnClicked(object sender, EventArgs e)
        {
            if (itemid != null)
            {
                await Navigation.PushAsync(new AddtoCartPage(),true);
            }
            else
            {
                await DisplayAlert("No item selected", "Please select an item on your cart to modify!", "OK");
            }
        }

        private async void Voiditem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var answer = await DisplayAlert("Void", "Do you want to void this order?", "Yes", "No");
                if (!answer) return;
                var orderDetails = new TBL_Orders()
                {
                    id = CurrentOrderId,
                };
                await TBL_Orders.Void(orderDetails);
                itemid = null;
                Selected_ProdId = null;
                CurrentOrderId = null;
                OnAppearing();
                await DisplayAlert("Order cancelled", "Your order has been cancelled successfully.", "OK");
                await Navigation.PopToRootAsync(true);
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
            
        }
    }
}