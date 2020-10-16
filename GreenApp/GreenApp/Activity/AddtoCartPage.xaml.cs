using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using GreenApp.Renderers;
using Plugin.Toasts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.DateTime;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddtoCartPage
    {
        private bool _newprodorder;
        private string _orderdetailsId;
        private double _defquantity = 1;
        public AddtoCartPage()
        {
            Random generator = new Random();
            InitializeComponent();
            qtystepper.Text = _defquantity.ToString(CultureInfo.CurrentCulture);
            lblrndnumpeople.Text = generator.Next(1, 20).ToString();
        }

        protected override async void OnAppearing()
        {
            try
            {
                if (CurrentOrderId == null)
                {
                    var selectedproduct = (await MobileService.GetTable<TBL_Products>().Where(id => id.id == Selected_ProdId).ToListAsync()).FirstOrDefault();
                    selectedproductcontainer.BindingContext = selectedproduct;
                    txtsubtotal.Text = (double.Parse(qtystepper.Text) * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
                    _newprodorder = true;
                }
                else
                {
                    var existingseletedproduct = (await MobileService.GetTable<V_Orders>().Where(ordertails => ordertails.order_id == CurrentOrderId && ordertails.prod_id == Selected_ProdId).ToListAsync()).FirstOrDefault();
                    if (existingseletedproduct == null)
                    {
                        var newselectedproduct = (await MobileService.GetTable<TBL_Products>().Where(id => id.id == Selected_ProdId).ToListAsync()).FirstOrDefault();
                        selectedproductcontainer.BindingContext = newselectedproduct;
                        txtsubtotal.Text = (double.Parse(qtystepper.Text) * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
                        _newprodorder = true;
                    }
                    else
                    {
                        selectedproductcontainer.BindingContext = existingseletedproduct;
                        qtystepper.Text = existingseletedproduct.qty;
                        _orderdetailsId = existingseletedproduct.id;
                        txtsubtotal.Text = (double.Parse(qtystepper.Text) * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
                        _newprodorder = false;
                    }
                }
            }
            catch
            {
                progressaddtocart.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }
        private async void Btnaddtocart_OnClicked(object sender, EventArgs e)
        {
            if (CurrentOrderId == null)
            {
                progressaddtocart.IsVisible = true;
                await InsertOrder();
            }
            else
            {
                progressaddtocart.IsVisible = true;
                await XGetOrderID();
                await InsertOrder_Details();
            }
        }

        private async Task InsertOrder()
        {
            try
            {
                var order = new TBL_Orders
                {
                    users_id = user_id,
                    order_date = Now.ToString("yyyy-MM-dd"),
                    stat = "0",
                    order_status = "Carted"
                };
                await TBL_Orders.Insert(order);
                //*****************************
                //Retrieving the orderID will continue coz of the logic userid and order status
                //the user Needs to complete the transaction for it to change the status, or cancel it in the server side
                //for the new order of the user

                //var getorderid = (await App.MobileService.GetTable<TBL_Orders>().Where(order_ => order_.users_id == App.user_id && order_.order_status == "Carted").ToListAsync()).FirstOrDefault();
                //if (getorderid != null) App.CurrentOrderId = getorderid.id;
                await XGetOrderID();
                //There's a chance that getting the order id will also be triggered in the menu
                //To identify if there is currently existing order when an internet
                //interruption occured.
                await InsertOrder_Details();
            }
            catch
            {
                progressaddtocart.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async Task XGetOrderID()
        {
            try
            {
                var getorderid = (await MobileService.GetTable<TBL_Orders>().Where(order => order.users_id == user_id && order.order_status == "Carted").ToListAsync()).FirstOrDefault();
                if (getorderid != null)
                {
                    CurrentOrderId = getorderid.id;
                }
            }
            catch
            {
                progressaddtocart.IsVisible = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async Task InsertOrder_Details()
        {
            //There might be a case of deleting the order
            //When an error occured.
            try
            {
                if (_newprodorder)
                {
                    var orderDetails = new TBL_Order_Details
                    {
                        order_id = CurrentOrderId,
                        prod_id = Selected_ProdId,
                        qty = qtystepper.Text.ToString(CultureInfo.InvariantCulture),
                        sell_price = txtprice.Text,
                        cart_datetime = Now.ToString(CultureInfo.CurrentCulture),
                        sub_total = txtsubtotal.Text
                    };
                    await TBL_Order_Details.Insert(orderDetails);
                    progressaddtocart.IsVisible = false;
                    await DisplayAlert("Success", "Item successfully added to cart!", "OK");
                    await Navigation.PopAsync(true);
                }
                else
                {
                    var orderDetails = new TBL_Order_Details
                    {
                        id = _orderdetailsId,
                        order_id = CurrentOrderId,
                        prod_id = Selected_ProdId,
                        qty = qtystepper.Text.ToString(CultureInfo.InvariantCulture),
                        sell_price = txtprice.Text,
                        cart_datetime = Now.ToString(CultureInfo.CurrentCulture),
                        sub_total = txtsubtotal.Text
                    };
                    await TBL_Order_Details.Update(orderDetails);
                    //var notificator = DependencyService.Get<IToastNotificator>();
                    //var options = new NotificationOptions()
                    //{
                    //    Title = "Green Market",
                    //    Description = "Cart updated!"
                    //};
                    //await notificator.Notify(options);
                    progressaddtocart.IsVisible = false;
                    await DisplayAlert("Success", "Cart successfully updated!", "OK");
                    await Navigation.PopAsync(true);
                }
            }
            catch
            {
                progressaddtocart.IsVisible = false;
                //await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private void Btnincreaseqty_OnClicked(object sender, EventArgs e)
        {
           IncreaseQty(double.Parse(qtystepper.Text));
        }

        private void IncreaseQty(double qty)
        {
            qtystepper.Text = (qty + .5).ToString(CultureInfo.CurrentCulture);
            txtsubtotal.Text = (double.Parse(qtystepper.Text) * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
        }
        private void DecreaseQty(double qty)
        {
            if (qtystepper.Text == "1") return;
            qtystepper.Text = (qty - .5).ToString(CultureInfo.CurrentCulture);
            txtsubtotal.Text = (double.Parse(qtystepper.Text) * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
        }

        private void Btndecreaseqty_OnClicked(object sender, EventArgs e)
        {
            DecreaseQty(double.Parse(qtystepper.Text));
        }

        private void Btnrefresh_OnClicked(object sender, EventArgs e)
        {
            qtystepper.Text = "1";
            txtsubtotal.Text = (double.Parse(qtystepper.Text) * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
        }
    }
}