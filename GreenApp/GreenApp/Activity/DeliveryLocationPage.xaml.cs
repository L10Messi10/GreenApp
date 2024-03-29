﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using static GreenApp.Activity.AddressesPage;
using static GreenApp.App;
using PermissionStatus = Xamarin.Essentials.PermissionStatus;


[assembly: Xamarin.Forms.Dependency(typeof(GreenApp.Utils.ILocSettings))]
namespace GreenApp.Activity
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryLocationPage
    {
        private string _label;
        public static bool _newAdd;
        private double _calc_distance;
        public DeliveryLocationPage()
        {
            InitializeComponent();
            //BindingContext = new DeliveryLocationPageViewModel();
            //Pin pin = new Pin()
            //{
            //    Type = PinType.Place,
            //    Label = "Tokyo SkyFree",
            //    Address = "Sumida-ku Tokyo, Japan",
            //    Position = new Position(35.71d,139.81d),
            //    Rotation = 33.3f,
            //    Tag = "id_tokyo"
            //};

            //map.Pins.Add(pin);
            //map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(5000)));
            //ApplyMapTheme();
            map.PropertyChanged += Map_PropertyChanged;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            //map.CameraIdled += Map_CameraIdled;
        }

        private async void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {

                Geocoder geoCoder = new Geocoder();
            var m = map;
            if (m.VisibleRegion != null)
            {
                var center = new Position(m.VisibleRegion.Center.Latitude, m.VisibleRegion.Center.Longitude);
                    //var span = new MapSpan(center, 0.001, 0.001);
                    //map.MoveToRegion(span);
                    IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(center);
                var enumerable = possibleAddresses as string[] ?? possibleAddresses.ToArray();
                picker.ItemsSource = enumerable.ToList();
                picker.SelectedIndex = 1;
                order_lat = m.VisibleRegion.Center.Latitude;
                order_long = m.VisibleRegion.Center.Longitude;
                
                //await DisplayAlert ("Alert","Lat: " + m.VisibleRegion.Center.Latitude.ToString() + " Lon:" + m.VisibleRegion.Center.Longitude.ToString() + " ","OK");
            }

            }
            catch
            {
                //this line doesn't need a display alert but a label to display the status of connection.
                //await DisplayAlert("Unexpected Error", "An unexpected error occured, please try again later. Please check your internet connectivity as well.", "OK");
                //_selectedAddressId = "";
                order_lat = 0;
                order_long = 0;
                var locator = CrossGeolocator.Current;
                locator.PositionChanged -= Locator_PositionChanged;
                map.PropertyChanged -= Map_PropertyChanged;
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                await locator.StopListeningAsync();
                //await Navigation.PopAsync();
            }
        }
        
        protected override async void OnDisappearing()
        {
            try
            {
                //_selectedAddressId = "";
                order_lat = 0;
                order_long = 0;
                var locator = CrossGeolocator.Current;
                locator.PositionChanged -= Locator_PositionChanged;
                map.PropertyChanged -= Map_PropertyChanged;
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                await locator.StopListeningAsync();
            }
            catch
            {
                //ignored
            }
        }

        protected override async void OnAppearing()
        {
            #region MapsSettings

            //try
            //{
            //    //await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location);
            //    //var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            //    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            //if (status != (Plugin.Permissions.Abstractions.PermissionStatus.Granted))
            //{
            //    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
            //    {
            //        await DisplayAlert("Need location", "Gunna need that location", "OK");
            //    }

            //    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            //    //Best practice to always check that the key exists
            //    if (results.ContainsKey(Permission.Location))
            //        status = results[Permission.Location];
            //}

            //if (status == (Plugin.Permissions.Abstractions.PermissionStatus.Granted))
            //{
            //    var results = await CrossGeolocator.Current.GetPositionAsync();
            //    //LabelGeolocation.Text = "Lat: " + results.Latitude + " Long: " + results.Longitude;
            //}
            //else if (status != (Plugin.Permissions.Abstractions.PermissionStatus)PermissionStatus.Unknown)
            //{
            //    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
            //}
            //}
            //catch (Exception ex)
            //{

            //    //LabelGeolocation.Text = "Error: " + ex;
            //}


            //try
            //{
            //    var location = await Geolocation.GetLastKnownLocationAsync();

            //    if (location != null)
            //    {
            //        //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
            //    }
            //}
            //catch (FeatureNotSupportedException fnsEx)
            //{
            //    // Handle not supported on device exception
            //    await DisplayAlert("Location", "Location service not supported!", "OK");
            //}
            //catch (FeatureNotEnabledException fneEx)
            //{
            //    // Handle not enabled on device exception
            //    await DisplayAlert("Location", "Location service not enabled!", "OK");
            //}
            //catch (PermissionException pEx)
            //{
            //    await DisplayAlert("Location", "Location service exception!", "OK");
            //    // Handle permission exception
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Location", "Location unable to get location!", "OK");
            //    // Unable to get location
            //}

            #endregion

            
            try
            {
                //********Get market location********************
                var geTblMarketLocation = (await MobileService.GetTable<TBL_MarketLocation>().ToListAsync()).FirstOrDefault();
                if (geTblMarketLocation != null)
                {
                    market_lat = geTblMarketLocation.m_lat;
                    market_long = geTblMarketLocation.m_long;
                    limit_distance = geTblMarketLocation.limit_distance;
                }

                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
                //some devices denies location in this line
                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    
                    if (await DisplayAlert("Permission", "Please allow this app to access your location by allowing it to the permissions.", "Go to Settings", "Cancel"))
                    {
                        permission_count = permission_count++;
                        if (permission_count > 1)
                        {
                            await DisplayAlert("Device Info.",
                                "For some reason, some devices tends to have a problem in permission setting especially in location permissions, if you believe you have already allowed this app to access your location and still wont display your location in the Map, just refresh your permission for this app by turning it OFF and ON again and come back to this page and try again.",
                                "OK");
                        }
                        AppInfo.ShowSettingsUI();
                        status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
                    }
                    //else
                    //{
                    //    await Navigation.PopAsync();
                    //}
                }

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    map.IsShowingUser = true;
                    map.PropertyChanged += Map_PropertyChanged;
                    //await DisplayAlert("Granted", "Permission granted!", "OK");
                    //Query permission
                }
                else if (status == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
                {
                    //error_wifi.IsVisible = true;
                    return;
                    //await DisplayAlert("Denied", "Permission denied!", "OK");
                    //AppInfo.ShowSettingsUI();
                }


                if (Device.RuntimePlatform == Device.Android)
                {
                    //DependencyService.Get<ISettingsService>().OpenSettings();
                    DependencyService.Get<Utils.ILocSettings>().OpenSettings();

                    if (_newAdd)
                    {
                        _label = "Home";
                        btnhome.BackgroundColor = Color.FromRgb(0, 158, 73);
                        btnhome.TextColor = Color.White;
                        btnwork.TextColor = Color.Black;
                        btnothers.TextColor = Color.Black;
                        btnwork.BackgroundColor = Color.Transparent;
                        btnothers.BackgroundColor = Color.Transparent;
                        var locator = CrossGeolocator.Current;
                        locator.PositionChanged += Locator_PositionChanged;
                        await locator.StartListeningAsync(new TimeSpan(30), 200);
                        var position = await locator.GetPositionAsync();
                        var center = new Position(position.Latitude, position.Longitude);
                        var span = new MapSpan(center, 0.001, 0.001);
                        map.MoveToRegion(span);
                    }
                    else
                    {
                        await ModifyAddress();
                    }
                }
                if (_newAdd)
                {
                    _label = "Home";
                    btnhome.BackgroundColor = Color.FromRgb(0, 158, 73);
                    btnhome.TextColor = Color.White;
                    btnwork.TextColor = Color.Black;
                    btnothers.TextColor = Color.Black;
                    btnwork.BackgroundColor = Color.Transparent;
                    btnothers.BackgroundColor = Color.Transparent;
                    var locator = CrossGeolocator.Current;
                    locator.PositionChanged += Locator_PositionChanged;
                    await locator.StartListeningAsync(new TimeSpan(30), 200);
                    var position = await locator.GetPositionAsync();
                    var center = new Position(position.Latitude, position.Longitude);
                    var span = new MapSpan(center, 0.001, 0.001);
                    map.MoveToRegion(span);
                }
                else
                {
                    await ModifyAddress();
                }
            }
            catch
            {
                //if(DisplayAlert)
                //var locator = CrossGeolocator.Current;
                //locator.PositionChanged -= Locator_PositionChanged;
                //map.PropertyChanged -= Map_PropertyChanged;
                //Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                //await locator.StopListeningAsync();
                //{
                //    a = 1;
                //    var result = await DisplayAlert("Permission", "You must allow permission for the app to locate your location", "Go Settings", "Cancel");
                //    if (result)
                //    {
                //        AppInfo.ShowSettingsUI();
                //    }
                //    else
                //        await Navigation.PopAsync();
                //}
            }

            //This gets the current location of the user's device.
            //await DisplayAlert("Info", "The app will detect your current location. Please allow the app to access your location.", "OK");

        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //error_wifi.IsVisible = false;
            error.IsVisible = e.NetworkAccess != NetworkAccess.Internet;
        }

        private async Task ModifyAddress()
        {
            try
            {

                var getAddresseses = (await MobileService.GetTable<TBL_Addresses>().Where(add => add.id == _selectedAddressId).ToListAsync()).FirstOrDefault();
                addressInfo.BindingContext = getAddresseses;
                
                if (getAddresseses != null)
                {

                    var center = new Position(getAddresseses.add_lat, getAddresseses.add_long);
                    var span = new MapSpan(center, 0.001, 0.001);
                    map.MoveToRegion(span);
                    txtbuilding.Text = getAddresseses.building_name;
                    txtnotes.Text = getAddresseses.Notes;
                }
                //txtbuilding.Text = getAddresseses.building_name;
                //txtnotes.Text = getAddresseses.Notes;
                btnaddnewaddress.Text = "Modify Address";
                if (labelas.Text == "Home")
                {
                    _label = "Home";
                    btnhome.BackgroundColor = Color.FromRgb(0, 158, 73);
                    btnhome.TextColor = Color.White;
                    btnwork.TextColor = Color.Black;
                    btnothers.TextColor = Color.Black;
                    btnwork.BackgroundColor = Color.Transparent;
                    btnothers.BackgroundColor = Color.Transparent;

                }

                if (labelas.Text == "Work")
                {
                    _label = "Work";
                    btnwork.BackgroundColor = Color.FromRgb(0, 158, 73);
                    btnhome.TextColor = Color.Black;
                    btnwork.TextColor = Color.White;
                    btnothers.TextColor = Color.Black;
                    btnhome.BackgroundColor = Color.Transparent;
                    btnothers.BackgroundColor = Color.Transparent;
                }

                if (labelas.Text == "Others")
                {
                    _label = "Others";
                    btnothers.BackgroundColor = Color.FromRgb(0, 158, 73);
                    btnhome.TextColor = Color.Black;
                    btnwork.TextColor = Color.Black;
                    btnothers.TextColor = Color.White;
                    btnwork.BackgroundColor = Color.Transparent;
                    btnhome.BackgroundColor = Color.Transparent;
                }
                //ListAddress.ItemsSource = getAddresseses;
            }
            catch
            {
                if (await DisplayAlert("Permission", "You must allow permission for the app to locate your location", "Settings", "Cancel"))
                {
                    AppInfo.ShowSettingsUI();
                }
                else
                    await Navigation.PopAsync();
            }

        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            try
            {
                order_lat = e.Position.Latitude;
                order_long = e.Position.Longitude;
                var center = new Position(e.Position.Latitude, e.Position.Longitude);
                var span = new MapSpan(center, 0.001, 0.001);
                map.MoveToRegion(span);
            }
            catch
            {
                //error_wifi.IsVisible = true;
            }
        }

        private async void Btnsetdelivery_OnClicked(object sender, EventArgs e)
        {
            try
            {
                CalculateDistance();
                bool isnotesEmpty = string.IsNullOrEmpty(txtnotes.Text);
                bool isubuildingEmpty = string.IsNullOrEmpty(txtbuilding.Text);

                if (_calc_distance > limit_distance)
                {
                    var round = Math.Round(limit_distance, 2);
                    await this.DisplayToastAsync("The address you've entered isn't covered by the service. Please try again with another location closer to the allowed distance from the market which is " + round + " KM");
                    //await DisplayAlert("Distance error", "The address you've entered isn't covered by the service. Please try again with another location closer to the allowed distance from the market which is " + round + " KM", "OK");
                    return;
                }

                if (isnotesEmpty || isubuildingEmpty)
                {
                    await this.DisplayToastAsync("Please enter a notes and building name with specific instruction for our riders.");
                    //txtnotes.Focus();
                }
                else
                {
                    if (btnaddnewaddress.Text != "Modify Address")
                    {
                        //add address
                        string c_add;
                        int selectedIndex = picker.SelectedIndex;
                        c_add = (string) picker.ItemsSource[selectedIndex];
                        if (_label != null)
                        {
                            progressplaceorder.IsVisible = true;
                            var addrress = new TBL_Addresses
                            {
                                user_id = user_id,
                                building_name = txtbuilding.Text,
                                Address = c_add,
                                add_lat = order_lat,
                                add_long = order_long,
                                Label = _label,
                                Notes = txtnotes.Text
                            };
                            await TBL_Addresses.Insert(addrress);
                            await DisplayAlert("Info", "New address added. You can now choose this address where you want the items to be delivered.", "OK");
                            await Navigation.PopAsync(true);
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please select a label for this address.", "OK");
                        }
                    }
                    else
                    {
                        //modify address
                        string c_add;
                        var selectedIndex = picker.SelectedIndex;
                        c_add = (string) picker.ItemsSource[selectedIndex];
                        if (_label != null)
                        {
                            progressplaceorder.IsVisible = true;
                            var addrress = new TBL_Addresses
                            {
                                id = _selectedAddressId,
                                user_id = user_id,
                                building_name = txtbuilding.Text,
                                Address = c_add,
                                add_lat = order_lat,
                                add_long = order_long,
                                Label = _label,
                                Notes = txtnotes.Text
                            };
                            await TBL_Addresses.Update(addrress);
                            await DisplayAlert("Info", "Address updated successfully.", "OK");
                            await Navigation.PopAsync(true);
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please select a label for this address.", "OK");
                        }
                    }
                }
            }
            catch
            {
                //error_wifi.IsVisible = true;
            }
        }

        private void CalculateDistance()
        {
            //this will get the distance of  the user and the market.
            var location = new Location(market_lat, market_long);
            var otherLocation = new Location(order_lat, order_long);
            _calc_distance = location.CalculateDistance(otherLocation, DistanceUnits.Kilometers);
        }

        private void Btnhome_OnClicked(object sender, EventArgs e)
        {
            _label = "Home";
            btnhome.BackgroundColor = Color.FromRgb(0, 158, 73);
            btnhome.TextColor=Color.White;
            btnwork.TextColor=Color.Black;
            btnothers.TextColor=Color.Black;
            btnwork.BackgroundColor=Color.Transparent;
            btnothers.BackgroundColor=Color.Transparent;
        }

        private void Btnwork_OnClicked(object sender, EventArgs e)
        {
            _label = "Work";
            btnwork.BackgroundColor = Color.FromRgb(0, 158, 73);
            btnhome.TextColor = Color.Black;
            btnwork.TextColor = Color.White;
            btnothers.TextColor = Color.Black;
            btnhome.BackgroundColor = Color.Transparent;
            btnothers.BackgroundColor = Color.Transparent;
        }

        private void Btnothers_OnClicked(object sender, EventArgs e)
        {
            _label = "Others";
            btnothers.BackgroundColor = Color.FromRgb(0, 158, 73);
            btnhome.TextColor = Color.Black;
            btnwork.TextColor = Color.Black;
            btnothers.TextColor = Color.White;
            btnwork.BackgroundColor = Color.Transparent;
            btnhome.BackgroundColor = Color.Transparent;
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            //error_wifi.IsVisible = false;
            error.IsVisible = Connectivity.NetworkAccess != NetworkAccess.Internet;
        }

        private void Btnsettings_OnClicked(object sender, EventArgs e)
        {
            AppInfo.ShowSettingsUI();
        }
    }
}