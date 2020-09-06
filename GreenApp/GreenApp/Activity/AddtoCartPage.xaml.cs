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

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddtoCartPage
    {
        private bool _newprodorder;
        private string _orderdetailsId;
        public AddtoCartPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            //btnaddtocart.Clicked += Btnaddtocart_OnClicked;
            try
            {
                if (App.CurrentOrderId == null)
                {
                    var selectedproduct = (await App.MobileService.GetTable<TBL_Products>().Where(id => id.id == App.Selected_ProdId).ToListAsync()).FirstOrDefault();
                    selectedproductcontainer.BindingContext = selectedproduct;
                    txtsubtotal.Text = (qtystepper.Value * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
                    _newprodorder = true;
                }
                else
                {
                    var existingseletedproduct = (await App.MobileService.GetTable<V_Orders>().Where(ordertails => ordertails.order_id == App.CurrentOrderId && ordertails.prod_id == App.Selected_ProdId).ToListAsync()).FirstOrDefault();
                    if (existingseletedproduct == null)
                    {
                        var newselectedproduct = (await App.MobileService.GetTable<TBL_Products>().Where(id => id.id == App.Selected_ProdId).ToListAsync()).FirstOrDefault();
                        selectedproductcontainer.BindingContext = newselectedproduct;
                        txtsubtotal.Text = (qtystepper.Value * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
                        _newprodorder = true;
                    }
                    else
                    {
                        selectedproductcontainer.BindingContext = existingseletedproduct;
                        qtystepper.Value = double.Parse(existingseletedproduct.qty);
                        _orderdetailsId = existingseletedproduct.id;
                        txtsubtotal.Text = (qtystepper.Value * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
                        _newprodorder = false;
                    }
                }
            }
            catch
            {
                await DisplayAlert("Error", "An error occured. Please check your internet connection.", "OK");
            }
        }

        private void Qtystepper_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (qtystepper != null) txtsubtotal.Text = (qtystepper.Value * double.Parse(txtprice.Text)).ToString(CultureInfo.InvariantCulture);
        }

        private async void Btnaddtocart_OnClicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Confirm", "Do you want to add this to your cart?", "Yes", "No");
            if (answer)
            {
                if (App.CurrentOrderId == null)
                {
                    await InsertOrder();
                }
                else
                {
                    await XGetOrderID();
                    await InsertOrder_Details();
                }
            }
        }

        private async Task InsertOrder()
        {
            try
            {
                var order = new TBL_Orders
                {
                    users_id = App.user_id,
                    order_date = DateTime.Now,
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
                await DisplayAlert("Error", "An error occured. Please check your internet connection.", "OK");
            }
        }

        private async Task XGetOrderID()
        {
            var getorderid = (await App.MobileService.GetTable<TBL_Orders>().Where(order_ => order_.users_id == App.user_id && order_.order_status == "Carted").ToListAsync()).FirstOrDefault();
            if (getorderid != null)
            {
                App.CurrentOrderId = getorderid.id;
            }
        }
        private async Task InsertOrder_Details()
        {
            //There might be a case of deleting the order
            //When an error occured.
            //try
            //{
                if (_newprodorder)
                {
                    var order_details = new TBL_Order_Details
                    {
                        order_id = App.CurrentOrderId,
                        prod_id = App.Selected_ProdId,
                        qty = qtystepper.Value.ToString(CultureInfo.InvariantCulture),
                        sell_price = txtprice.Text,
                        sub_total = txtsubtotal.Text
                    };
                    await TBL_Order_Details.Insert(order_details);
                    var s = App.CurrentOrderId; ;
                await DisplayAlert("Success", "Item successfully added to cart!", "OK");
                await Navigation.PopAsync(true);
                }
                else
                {
                    var order_details = new TBL_Order_Details
                    {
                        id = _orderdetailsId,
                        order_id = App.CurrentOrderId,
                        prod_id = App.Selected_ProdId,
                        qty = qtystepper.Value.ToString(CultureInfo.InvariantCulture),
                        sell_price = txtprice.Text,
                        sub_total = txtsubtotal.Text
                    };
                    await TBL_Order_Details.Update(order_details);
                //var notificator = DependencyService.Get<IToastNotificator>();
                //var options = new NotificationOptions()
                //{
                //    Title = "Green Market",
                //    Description = "Cart updated!"
                //};
                //await notificator.Notify(options);
                await DisplayAlert("Success", "Cart successfully updated!", "OK");
                await Navigation.PopAsync(true);
                }
            //}
            //catch
            //{
            //    await DisplayAlert("Error", "An error occured. Please check your internet connection.", "OK");
            //}
        }
    }
}