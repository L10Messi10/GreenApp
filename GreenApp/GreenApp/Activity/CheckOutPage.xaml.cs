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
        private int itemcount;
        public CheckOutPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                progressplaceorder.IsVisible = true;
                lblorderstate.Text = "Loading order . . .";
                if (CurrentOrderId != null)
                {
                    var getorders = await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == CurrentOrderId).ToListAsync();
                    ordercollection.ItemsSource = getorders;
                    itemcount = getorders.Count;
                    //ordercollection.ItemsSource.re
                    totaSum = getorders.AsQueryable().Sum(ord => ord.sub_total);
                    lblsubtotal.Text = totaSum.ToString(CultureInfo.InvariantCulture);
                    totalpayable.Text ="Php "+ totaSum.ToString(CultureInfo.InvariantCulture);
                    itemid = null;
                    Selected_ProdId = null;
                    progressplaceorder.IsVisible = false;
                    totItems.Text = "Your order (" + itemcount + ") items";
                }
                else
                {
                    progressplaceorder.IsVisible = false;
                    lblsubtotal.Text = "0";
                    totalpayable.Text = "0";
                    itemid = null;
                    Selected_ProdId = null;
                }
            }
            catch
            {
                progressplaceorder.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
            
        }

        private async void Btncheckout_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (picker.SelectedItem != null)
                {
                    if (totalpayable.Text != "0")
                    {
                        var stat = (await MobileService.GetTable<TBL_MarketStatus>().ToListAsync()).FirstOrDefault();
                        if (stat != null) MarketStatus = stat.status;
                        if (MarketStatus == "1")
                        {
                            var answer = await DisplayAlert("Confirm", "Do you want to confirm this order?", "Yes", "No");
                            if (answer)
                            {
                                progressplaceorder.IsVisible = true;
                                lblorderstate.Text = "Placing your order . . .";
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
                                progressplaceorder.IsVisible = false;
                                await Navigation.PushAsync(new ConfirmationPage(), true);
                            }
                        }
                        else
                        {
                            progressplaceorder.IsVisible = false;
                            await Navigation.PushAsync(new MarketClosePage(), true);
                        }
                    }
                    else
                    {
                        progressplaceorder.IsVisible = false;
                        await DisplayAlert("Cart empty", "Your cart is empty!", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please select order option!", "OK");
                }
            }
            catch
            {
                progressplaceorder.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
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
                if (itemcount == 1)
                {
                    if (itemid != null)
                    {
                        var answer = await DisplayAlert("Void", "There's only one item remaining on the list. Do you want to void this transaction?", "Yes", "No");
                        if (!answer) return;
                        progressplaceorder.IsVisible = true;
                        lblorderstate.Text = "Voiding your order . . .";
                        var orderDetails = new TBL_Orders()
                        {
                            id = CurrentOrderId,
                        };
                        await TBL_Orders.Void(orderDetails);
                        itemid = null;
                        Selected_ProdId = null;
                        CurrentOrderId = null;
                        progressplaceorder.IsVisible = false;
                        await DisplayAlert("Order cancelled", "Your successfully cancelled your order", "OK");
                        await Navigation.PopToRootAsync(true);
                    }
                    else
                    {
                        await DisplayAlert("No item selected", "Please select an item on your cart to remove!", "OK");
                    }
                }
                else if (itemcount == 0)
                {
                    await DisplayAlert("Remove", "Nothing to remove", "OK");
                }
                else
                {
                    //ordercollection.
                    if (itemid != null)
                    {
                        
                        var confirm = await DisplayAlert("Remove", "Do you really want to remove this item on your cart?", "Yes", "No");
                        if (!confirm) return;
                        progressplaceorder.IsVisible = true;
                        lblorderstate.Text = "Removing item . . .";
                        var order_details = new TBL_Order_Details
                        {
                            id = itemid,
                        };
                        await TBL_Order_Details.Delete(order_details);
                        itemid = null;
                        Selected_ProdId = null;
                        progressplaceorder.IsVisible = false;
                        OnAppearing();
                    }
                    else
                    {
                        await DisplayAlert("No item selected", "Please select an item on your cart to remove!", "OK");
                    }
                }
            }
            catch
            {
                progressplaceorder.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async void Edititem_OnClicked(object sender, EventArgs e)
        {
            if (itemcount == 0)
            {
                await DisplayAlert("Void", "Nothing to modify", "OK");
            }
            else
            {
                if (itemid != null)
                {
                    await Navigation.PushAsync(new AddtoCartPage(), true);
                }
                else
                {
                    await DisplayAlert("No item selected", "Please select an item on your cart to modify!", "OK");
                }
            }
            
        }

        private async void Voiditem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (itemcount == 0)
                {
                    await DisplayAlert("Void", "Nothing to void", "OK");
                }
                else
                {
                    var answer = await DisplayAlert("Void", "Do you want to void this order?", "Yes", "No");
                    if (!answer) return;
                    progressplaceorder.IsVisible = true;
                    lblorderstate.Text = "Voiding your order . . .";
                    var orderDetails = new TBL_Orders()
                    {
                        id = CurrentOrderId,
                    };
                    await TBL_Orders.Void(orderDetails);
                    itemid = null;
                    Selected_ProdId = null;
                    CurrentOrderId = null;
                    progressplaceorder.IsVisible = true;
                    await DisplayAlert("Order cancelled", "Your order has been cancelled successfully.", "OK");
                    await Navigation.PopToRootAsync(true);
                }
            }
            catch
            {
                progressplaceorder.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string selected;
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                selected = (string)picker.ItemsSource[selectedIndex];
                if (selected == "Delivery")
                {
                    await Navigation.PushAsync(new DeliveryLocationPage(), true);
                }
            }
        }
    }
}