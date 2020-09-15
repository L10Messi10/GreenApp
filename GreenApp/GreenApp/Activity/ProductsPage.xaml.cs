using System;
using System.Collections.Generic;
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
    public partial class ProductsPage : ContentPage
    {
        public ProductsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                var getproducts = await MobileService.GetTable<TBL_Products>().Where(p => p.category_name == Selected_CatID).ToListAsync();
                ListProducts.ItemsSource = getproducts;
                var getorderid = (await App.MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id == App.user_id && orders.order_status == "Carted").ToListAsync()).FirstOrDefault();
                if (getorderid != null) CurrentOrderId = getorderid.id;
                if (CurrentOrderId != null)
                {
                    var totalorders = await TBL_Order_Details.Read();
                    lblcartcount.Text = totalorders.Count(p => p.order_id.Equals(CurrentOrderId)).ToString();
                }
                else
                {
                    lblcartcount.Text = "0";
                }
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
        }

        private async void ListProducts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Selected_ProdId = (e.CurrentSelection.FirstOrDefault() as TBL_Products)?.id;
                await Navigation.PushAsync(new AddtoCartPage());
            }
            catch
            {
                //ignored
            }
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CheckOutPage(), true);
        }

        private async Task XSearchProduct (string query)
        {
            try
            {

            
                var products = (await MobileService.GetTable<TBL_Products>().ToListAsync());
                ListProducts.ItemsSource = products.Where(p => p.prod_name.ToLower().Contains(query.ToLower())  && 
                                                           p.category_name.ToLower().Equals(Selected_CatID.ToLower())).ToList();
            }
            catch
            {
                await DisplayAlert("Error", "Unexpected error occured. Please check your internet connectivity!", "OK");
            }
        }

        private async void Prosearh_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await XSearchProduct(prosearh.Text);
            if (prosearh.Text == null)
            {
                OnAppearing();
            }
        }
    }
}