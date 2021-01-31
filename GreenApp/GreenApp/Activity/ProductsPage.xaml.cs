using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.Activity.MenuPage;
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
            await getProducts();
        }

        private async Task getProducts()
        {
            try
            {
                catImg.Source = cat_img_uri;
                lblSection.Text = Selected_CatID;
                lblcatdesc.Text = cat_desc;
                ListProducts.IsVisible = true;
                ErrorLayout.IsVisible = false;
                var getproducts = await MobileService.GetTable<TBL_Products>().Where(p => p.category_name == Selected_CatID).ToListAsync();
                ListProducts.ItemsSource = getproducts;
                var getorderid = (await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id == user_id && orders.order_status == "Carted").ToListAsync()).FirstOrDefault();
                if (getorderid != null) CurrentOrderId = getorderid.id;
                if (CurrentOrderId != null)
                {
                    //var totalorders = await TBL_Order_Details.Read();
                    var getorders = await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == CurrentOrderId).ToListAsync();
                    lblcartcount.Text = getorders.Count.ToString(); //totalorders.Count(p => p.order_id.Equals(CurrentOrderId)).ToString();
                }
                else
                {
                    lblcartcount.Text = "0";
                }
                RefreshView.IsRefreshing = false;
                ListProducts.SelectedItem = null;
            }
            catch
            {
                ListProducts.SelectedItem = null;
                RefreshView.IsRefreshing = false;
                //await Navigation.PushAsync(new NoInternetPage(), true);
                //progressLoading.IsVisible = false;
                ListProducts.IsVisible = false;
                ErrorLayout.IsVisible = true;
            }
        }
        private async void ListProducts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListProducts.SelectedItem == null) return;
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
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async void Prosearh_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await XSearchProduct(prosearh.Text);
            //if (prosearh.Text == null)
            //{
            //    await getProducts();
            //}
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await getProducts();
        }
    }
}