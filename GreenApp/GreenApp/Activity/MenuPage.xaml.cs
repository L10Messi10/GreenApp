using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenApp.Models;
using GreenApp.Utils;
using Plugin.FirebasePushNotification;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using static GreenApp.Activity.AddressesPage;
using static GreenApp.App;
using static Xamarin.Forms.Application;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public static string cat_img_uri;
        public MenuPage()
        {
            InitializeComponent();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
            //getUserDetails();

        }
        private async void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            //might need to save the token in database.
            //System.Diagnostics.Debug.WriteLine($"Token :{e.Token}");
            try
            {
                //tokenStr = e.Token;
                if (SignedIn)
                {
                    var users = (await MobileService.GetTable<TBL_Token>().Where(userid => userid.user_id == user_id).ToListAsync()).FirstOrDefault();
                    if (users == null)
                    {
                        var token = new TBL_Token
                        {
                            user_id = user_id,
                            push_token = e.Token
                        };
                        await TBL_Token.Insert(token);
                    }
                    else
                    {
                        var token = new TBL_Token
                        {
                            id = users.id,
                            user_id = user_id,
                            push_token = e.Token
                        };
                        await TBL_Token.Update(token);
                    }
                }
            }
            catch
            {
                //ignored
            }

        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.None)
            {
                await this.DisplayToastAsync("You have no internet connection.");

            }
            else if (e.NetworkAccess == NetworkAccess.Internet)
            {
                await this.DisplayToastAsync("You internet connection was restored.");
                await GetSections();
                await GetUserDetails();
               

            }
        }

        protected override async void OnAppearing()
        {
            await GetSections();
            await GetUserDetails();
        }

        private async Task GetUserDetails()
        {
            try
            {
                if (propic != null)
                {
                    menuTray.IsVisible = false;
                    profilepic.IsVisible = true;
                    profilepic.Source = propic;
                }
                else
                {
                    menuTray.IsVisible = true;
                    profilepic.IsVisible = false;
                }
                
                if (SignedIn) return;
                if (Settings.LastUsedEmail != string.Empty)
                {
                    var users = (await MobileService.GetTable<TBL_Users>().Where(mail => mail.emailadd == Settings.LastUsedEmail).ToListAsync()).FirstOrDefault();
                    if (users == null) return;
                    user_id = users.Id;
                    fullname = users.full_name;
                    mobilenum = users.mobile_num;
                    emailadd = users.emailadd;
                    password = users.password;
                    datereg = users.datereg;
                    propic = users.propic;
                    picstr = users.picstr;
                    //user_id = null;
                    CurrentOrderId = null;
                    refresh = false;
                    SignedIn = true;
                    hasnetwork = true;
                    if (propic != null)
                    {
                        menuTray.IsVisible = false;
                        profilepic.IsVisible = true;
                        profilepic.Source = propic;
                    }
                    else
                    {
                        menuTray.IsVisible = true;
                        profilepic.IsVisible = false;
                    }

                    //indicatorloader.IsVisible = false;
                    //Settings.LastUsedEmail = chkremember.IsChecked ? emailentry.Text : "";
                    //await DisplayAlert("Success", "Email or password is incorrect!", "OK");
                    //Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = new AppShell(); });
                    //await Navigation.PushAsync(new MenuPage(), true);
                    //var page = MenuPage as NavigationPage;
                }
                else
                {
                    if (propic != null)
                    {
                        menuTray.IsVisible = false;
                        profilepic.IsVisible = true;
                        profilepic.Source = propic;
                    }
                    else
                    {
                        menuTray.IsVisible = true;
                        profilepic.IsVisible = false;
                    }
                    hasnetwork = true;
                    SignedIn = false;
                    //Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = new LoginPage(); });
                    //await Navigation.PushAsync(new LoginPage(), true);
                }

            }
            catch
            {
                if (propic != null)
                {
                    menuTray.IsVisible = false;
                    profilepic.IsVisible = true;
                    profilepic.Source = propic;
                }
                else
                {
                    menuTray.IsVisible = true;
                    profilepic.IsVisible = false;
                }
                hasnetwork = false;
                SignedIn = false;
            }
            
        }
        private async Task GetSections()
        {
            try
            {
                //RefreshView.IsRefreshing = true;
                MarketClosed.IsVisible = false;
                ListCategories.IsVisible = true;
                ErrorLayout.IsVisible = false;
                progressLoading.IsVisible = true;
                //Check if the market is open
                var stat = (await MobileService.GetTable<TBL_MarketStatus>().ToListAsync()).FirstOrDefault();
                if (stat != null) MarketStatus = stat.status;
                if (MarketStatus == "0")
                {
                    MarketClosed.IsVisible = true;
                }
                if (!refresh)
                {
                    var categories = await V_Categories_Display.Read();
                    if (categories.Count != 0)
                    {
                        ListCategories.ItemsSource = categories;
                        //var samp = CurrentOrderId ;
                        //EmptyLayout.IsVisible = false;
                        refresh = true;
                        RefreshView.IsRefreshing = false;
                        progressLoading.IsVisible = false;
                        //MarketClosed.IsVisible = false;
                    }
                    else
                    {
                        //EmptyLayout.IsVisible = true;
                        refresh = true;
                        ListCategories.ItemsSource = null;
                        //await Navigation.PushAsync(new NoInternetPage(), true);
                        progressLoading.IsVisible = false;
                        ListCategories.IsVisible = false;
                        progressLoading.IsVisible = false;
                    }
                }

                if (_selectedAddressId == "")
                {
                    var getAddresses = (await MobileService.GetTable<TBL_Addresses>().Where(p => p.user_id == user_id).ToListAsync()).FirstOrDefault();
                    if (getAddresses != null)
                    {
                        _selectedAddressId = getAddresses.id;
                        order_long = getAddresses.add_long;
                        order_lat = getAddresses.add_lat;
                        order_rcvr_add = getAddresses.Address;
                        build_name = getAddresses.building_name;
                        order_notes = getAddresses.Notes;
                    }
                }
                //Try to retrieve incomplete order if any.
                //There might be a case of deleting the order
                //When an error occured.
                
                var getorderid = (await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id == user_id && orders.order_status == "Carted").ToListAsync()).FirstOrDefault();
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
                progressLoading.IsVisible = false;
                ListCategories.SelectedItem = null;
            }
            catch
            {
                RefreshView.IsRefreshing = false;
                //EmptyLayout.IsVisible = false;
                ListCategories.ItemsSource = null;
                //await Navigation.PushAsync(new NoInternetPage(), true);
                progressLoading.IsVisible = false;
                ListCategories.IsVisible = false;
                ErrorLayout.IsVisible = true;
            }
        }
        private async Task XSearch(string query)
        {
            try
            {
                ListCategories.IsVisible = true;
                ErrorLayout.IsVisible = false;
                progressLoading.IsVisible = true;
                var products = await MobileService.GetTable<V_Categories_Display>().Take(100).ToListAsync();
                ListCategories.ItemsSource = products.Where(p => p.category_name.ToLower().Contains(query.ToLower())).ToList();
                RefreshView.IsRefreshing = false;
                progressLoading.IsVisible = false;
            }
            catch
            {
                RefreshView.IsRefreshing = false;
                //await Navigation.PushAsync(new NoInternetPage(), true);
                progressLoading.IsVisible = false;
                ListCategories.IsVisible = false;
                ErrorLayout.IsVisible = true;
            }
            
        }
        
        private async void Checkout_OnTapped(object sender, EventArgs e)
        {
            if (progressLoading.IsVisible || ErrorLayout.IsVisible || MarketClosed.IsVisible || MarketStatus == "0") return;
            await Navigation.PushAsync(new CheckOutPage(), true);
        }

        private async void Prosearh_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (progressLoading.IsVisible || ErrorLayout.IsVisible) return;
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
            Selected_CatID = (e.CurrentSelection.FirstOrDefault() as V_Categories_Display)?.category_name;
            cat_desc = (e.CurrentSelection.FirstOrDefault() as V_Categories_Display)?.cat_desc;
            cat_img_uri = (e.CurrentSelection.FirstOrDefault() as V_Categories_Display)?.catimg_uri;
            await Navigation.PushAsync(new ProductsPage(), true);
        }

        private async void RefreshView_OnRefreshing(object sender, EventArgs e)
        {
            refresh = false;
            await GetSections();
            await GetUserDetails();
        }

        private async void TapMenu_OnTapped(object sender, EventArgs e)
        {
            if (progressLoading.IsVisible || ErrorLayout.IsVisible ) return;
            await Navigation.PushAsync(new MenuTrayPage(), true);
        }

        private async void Btnretry_OnClicked(object sender, EventArgs e)
        {
            refresh = false;
            await GetSections();
            await GetUserDetails();
        }
    }
}