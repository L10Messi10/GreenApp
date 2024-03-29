﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.Activity.MenuPage;
using static GreenApp.App;
using Xamarin.Essentials;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductsPage : ContentPage
    {
        public ProductsPage()
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
                var getproducts = await MobileService.GetTable<TBL_Products>().Take(300).Where(p => p.category_name == Selected_CatID && p.prod_av=="Available").ToListAsync();
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
                ListProducts.ItemsSource = null;
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
                ListProducts.IsVisible = true;
                ErrorLayout.IsVisible = false;
                RefreshView.IsRefreshing = true;
                var products = await MobileService.GetTable<TBL_Products>().Take(200).ToListAsync();
                ListProducts.ItemsSource = products.Where(p => p.prod_name.ToLower().Contains(query.ToLower())  && 
                                                           p.category_name.ToLower().Equals(Selected_CatID.ToLower()) && p.prod_av == "Available").ToList();
                RefreshView.IsRefreshing = false;
            }
            catch
            {
                ListProducts.SelectedItem = null;
                RefreshView.IsRefreshing = false;
                //xRefreshView.IsRefreshing = false;
                //await Navigation.PushAsync(new NoInternetPage(), true);
                //progressLoading.IsVisible = false;
                ListProducts.IsVisible = false;
                ErrorLayout.IsVisible = true;
                //await this.DisplayToastAsync("A network error occured, please check your internet connectivity and try again.");

            }
        }

        private async void Prosearh_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await XSearchProduct(prosearh.Text);
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            await getProducts();
        }

        private async void Btnretry_OnClicked(object sender, EventArgs e)
        {
            await getProducts();
        }
    }
}