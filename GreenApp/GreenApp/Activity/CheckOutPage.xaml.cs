using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using static System.DateTime;
using static GreenApp.Activity.AddressesPage;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckOutPage
    {
        private double totaSum,delivery_fee, payable;
        private string itemid;
        private int itemcount;
        public CheckOutPage()
        {
            InitializeComponent();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.None)
            {
                this.DisplayToastAsync("You have no internet connection.");
            }
            else if (e.NetworkAccess == NetworkAccess.Internet)
            {
                this.DisplayToastAsync("You internet connection was restored.");
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                progressplaceorder.IsVisible = true;
                pickupTime.Time = Now.TimeOfDay;
                lblchoice.Text = "Delivery Address";
                deliveryAddLayout.IsVisible = true;
                pickUpLayout.IsVisible = false;
                lblorderstate.Text = "Loading order . . .";
                //Get orders
                var getAddresses = (await MobileService.GetTable<TBL_Addresses>().Where(p => p.id == _selectedAddressId).ToListAsync()).FirstOrDefault();
                deliveryAddLayout.BindingContext = getAddresses;
                //Get delivery fee
                var getdelFee = (await MobileService.GetTable<TBL_DeliveryFee>().ToListAsync()).FirstOrDefault();
                if (getdelFee != null)
                {
                    del_fee.Text = "₱ "+ getdelFee.d_fee.ToString(CultureInfo.InvariantCulture);
                    delivery_fee = getdelFee.d_fee;
                }

                if (CurrentOrderId != null)
                {
                    var getorders = await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == CurrentOrderId).ToListAsync();
                    ordercollection.ItemsSource = getorders;
                    itemcount = getorders.Count;
                    //ordercollection.ItemsSource.re
                    totaSum = getorders.AsQueryable().Sum(ord => ord.sub_total);
                    payable = Convert.ToDouble(totaSum + delivery_fee);
                    lblsubtotal.Text = "₱ " + totaSum.ToString(CultureInfo.InvariantCulture);
                    totalpayable.Text = "₱ " + payable.ToString(CultureInfo.InvariantCulture);
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
                if (SignedIn)
                {

                    if (progressplaceorder.IsVisible) return;
                    if (totalpayable.Text != "0")
                    {
                        
                        var stat = (await MobileService.GetTable<TBL_MarketStatus>().ToListAsync()).FirstOrDefault();
                        if (stat != null) MarketStatus = stat.status;
                        if (MarketStatus == "1")
                        {
                            if (lbladd.Text == "" && Switch.IsToggled)
                            {
                                await DisplayAlert("Address", "No address present, please setup your address first!", "OK");
                                return;
                            }
                            var answer = await DisplayAlert("Confirm", "Do you want to confirm this order?", "Yes", "No");
                            if (!answer) return;
                            progressplaceorder.IsVisible = true;
                            lblorderstate.Text = "Placing your order . . .";
                            if (Switch.IsToggled)
                            {
                                //Save delivery order
                                
                                    var orderDetails = new TBL_Orders()
                                    {
                                        id = CurrentOrderId,
                                        users_id = user_id,
                                        order_date = Now.ToString("yyyy-MM-dd"),
                                        cart_datetime = Now.ToString("ddd, dd MMM yyyy h:mm tt"),
                                        stat = "1",
                                        order_status = "Ordered",
                                        order_choice = "Delivery",
                                        del_address = order_rcvr_add,
                                        building_name = build_name,
                                        notes = order_notes,
                                        del_lat = order_lat.ToString(CultureInfo.InvariantCulture),
                                        del_long = order_long.ToString(CultureInfo.InvariantCulture),
                                        pickup_time = "-",
                                        itms_qty = itemcount.ToString(),
                                        tot_payable = payable.ToString(CultureInfo.InvariantCulture)
                                    };
                                    await TBL_Orders.Update(orderDetails);

                                    var track = new TBL_OrderTracking()
                                    {
                                        order_id = CurrentOrderId,
                                        track_status = "Order placed",
                                        track_desc = "Order Successfully placed for delivery.",
                                        track_time = Now.ToString("h:mm tt"),
                                        track_num = "1"
                                    };
                                    await TBL_OrderTracking.Insert(track);

                            }
                            else
                            {
                                //Save Pickup order
                                var orderDetails = new TBL_Orders()
                                {
                                    id = CurrentOrderId,
                                    users_id = user_id,
                                    order_date = Now.ToString("yyyy-MM-dd"),
                                    cart_datetime = Now.ToString("ddd, dd MMM yyyy h:mm tt"),
                                    stat = "1",
                                    order_status = "Ordered",
                                    order_choice = "Pickup",
                                    del_address = "-",
                                    building_name = "-",
                                    notes = "-",
                                    del_lat = "-",
                                    del_long = "-",
                                    pickup_time = pickupTime.Time.ToString(),
                                    itms_qty = itemcount.ToString(),
                                    tot_payable = payable.ToString(CultureInfo.InvariantCulture)
                                };
                                await TBL_Orders.Update(orderDetails);

                                var track = new TBL_OrderTracking()
                                {
                                    order_id = CurrentOrderId,
                                    track_status = "Order placed",
                                    track_desc = "Order Successfully placed for pick-up.",
                                    track_time = Now.ToString("h:mm tt"),
                                    track_num = "1"
                                };
                                await TBL_OrderTracking.Insert(track);
                            }

                            checkout = true;
                            progressplaceorder.IsVisible = false;
                            await Navigation.PushModalAsync(new ConfirmationPage(), true);
                            await Navigation.PopToRootAsync(true);
                            refresh = false;
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
                    await DisplayAlert("Login", "Please login or create an account first before doing any transaction in the market! It's FREE!", "OK");
                    await Navigation.PushAsync(new LoginPage());
                }
            }
            catch
            {
                await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
                progressplaceorder.IsVisible = false;
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
                        await DisplayAlert("Order cancelled", "You have successfully cancelled your order.", "OK");
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
                await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
                progressplaceorder.IsVisible = false;
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
                await DisplayAlert("Network Error", "A network error occured, please check your internet connectivity and try again.", "OK");
            }
        }

        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            if (Switch.IsToggled)
            {
                
                lblchoice.Text = "Delivery Address";
                deliveryAddLayout.IsVisible = true;
                pickUpLayout.IsVisible = false;
                payable = Convert.ToDouble(totaSum + delivery_fee);
                lblsubtotal.Text = "₱ " + totaSum.ToString(CultureInfo.InvariantCulture);
                totalpayable.Text = "₱ " + payable.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                DateTime currentTime = Now;
                DateTime x30MinsLater = currentTime.AddMinutes(45);
                pickupTime.Time = x30MinsLater.TimeOfDay;
                lblchoice.Text = "Pickup time: ";
                deliveryAddLayout.IsVisible = false;
                pickUpLayout.IsVisible = true;
                payable = Convert.ToDouble(totaSum + 0);
                lblsubtotal.Text = "₱ " + totaSum.ToString(CultureInfo.InvariantCulture);
                totalpayable.Text = "₱ " + payable.ToString(CultureInfo.InvariantCulture);
            }
        }

        private async void Btnchange_OnClicked(object sender, EventArgs e)
        {
            if (SignedIn)
            {
                _CheckingOut = true;
                await Navigation.PushAsync(new AddressesPage(), true);
            }
            else
            {
                await DisplayAlert("Login", "You're not logged-in, please login to continue.", "OK");
                await Navigation.PushAsync(new LoginPage(), true);
            }
        }
           
    }
}
