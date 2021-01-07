using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using static GreenApp.App;
using static Xamarin.Forms.Application;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            
        }
        
        protected override async void OnAppearing()
        {
            await GetSections();
        }

        private async Task GetSections()
        {
            try
            {
                RefreshView.IsRefreshing = true;
                if (!refresh)
                {
                    //var categories = await TBL_Category.Read();
                    //categorycollection.ItemsSource = categories;
                    
                    var categories = await TBL_Category.Read();
                    ListCategories.ItemsSource = categories;
                    //var samp = CurrentOrderId ;
                    refresh = true;
                    RefreshView.IsRefreshing = false;
                }
                //Try to retrieve incomplete order if any.
                //There might be a case of deleting the order
                //When an error occured.
                profpic.Source = propic;
                var getorderid = (await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id == App.user_id && orders.order_status == "Carted").ToListAsync()).FirstOrDefault();
                if (getorderid != null) CurrentOrderId = getorderid.id;
                if (CurrentOrderId != null)
                {
                    //var totalorders = await TBL_Order_Details.Read();
                    var getorders = await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == CurrentOrderId).ToListAsync();
                    lblcartcount.Text = getorders.Count.ToString(); //totalorders.Count(p => p.order_id.Contains(CurrentOrderId)).ToString();
                    RefreshView.IsRefreshing = false;
                    //double a = totalorders.Count(p => p.order_id.Equals(CurrentOrderId));
                    //double b=0;
                    //double e = a + b;
                    //await DisplayAlert("S", e.ToString() + user_id, "OK");
                }
                else
                {
                    lblcartcount.Text = "0";
                }
                RefreshView.IsRefreshing = false;
                ListCategories.SelectedItem = null;
            }
            catch
            {
                RefreshView.IsRefreshing = false;
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }
        private async Task XSearch(string query)
        {
            try
            {
                var products = (await MobileService.GetTable<TBL_Category>().ToListAsync());
                ListCategories.ItemsSource = products.Where(p => p.category_name.ToLower().Contains(query.ToLower())).ToList();
            }
            catch
            {
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
            
        }
        
        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CheckOutPage(), true);
        }

        private async void Prosearh_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await XSearch(prosearh.Text);
        }

        //private async void Categorycollection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    closecategory.IsVisible = true;
        //    var cat = (e.CurrentSelection.FirstOrDefault() as TBL_Category)?.category_name;
        //    var products = (await MobileService.GetTable<TBL_Products>().ToListAsync());
        //    ListProducts.ItemsSource = products.Where(p => cat != null && p.category_name.ToLower().Contains(cat.ToLower()));
        //}


        //var products = (await MobileService.GetTable<TBL_Products>().ToListAsync());
        //ListCategories.ItemsSource = products.Where(p => p.prod_desc.ToLower().Contains(query.ToLower()) ||
        //p.prod_name.ToLower().Contains(query.ToLower()) ||
        //p.category_name.ToLower().Contains(query.ToLower()) ||
        //p.prod_av.ToLower().Contains(query.ToLower())).ToList();


        //private async void Closecategory_OnClicked(object sender, EventArgs e)
        //{
        //    var categories = await TBL_Category.Read();
        //    categorycollection.ItemsSource = categories;
        //    var products = await TBL_Products.Read();
        //    ListProducts.ItemsSource = products;
        //    refresh = true;
        //    closecategory.IsVisible = false;
        //}
        private async void ListCategories_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListCategories.SelectedItem == null) return;
            Selected_CatID = (e.CurrentSelection.FirstOrDefault() as TBL_Category)?.category_name;
            await Navigation.PushAsync(new ProductsPage(), true);
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await GetSections();
        }
    }
}