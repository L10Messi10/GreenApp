using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using GreenApp.ViewModels;
using Plugin.Geolocator;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using static GreenApp.Activity.AddressesPage;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryLocationPage
    {
        private string _label;
        public static bool _newAdd;
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

            //map.CameraIdled += Map_CameraIdled;
        }

        private async void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                await DisplayAlert("Connection slow", "Your might be offline! Please try again later.", "OK");
                return;
            }

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
                //await DisplayAlert("Unexpected Error", "An unexpected error occured. Please try again later.", "OK");
                _selectedAddressId = "";
                order_lat = 0;
                order_long = 0;
                var locator = CrossGeolocator.Current;
                locator.PositionChanged -= Locator_PositionChanged;
                map.PropertyChanged -= Map_PropertyChanged;
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                await locator.StopListeningAsync();
                await Navigation.PopAsync();
            }
        }
        
        protected override async void OnDisappearing()
        {
            try
            {
                _selectedAddressId = "";
                order_lat = 0;
                order_long = 0;
                var locator = CrossGeolocator.Current;
                Geocoder geoCoder = new Geocoder();
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
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            try
            {
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
                    await locator.StartListeningAsync(new TimeSpan(0), 200);
                    var position = await locator.GetPositionAsync();
                    var center = new Position(position.Latitude, position.Longitude);
                    var span = new MapSpan(center, 0.001, 0.001);
                    map.MoveToRegion(span);
                }
                else
                {
                    await modifyAddress();
                }
            }
            catch
            {
                error_wifi.IsVisible = true;
            }
            
            //This gets the current location of the user's device.
            //await DisplayAlert("Info", "The app will detect your current location. Please allow the app to access your location.", "OK");
            
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            error_wifi.IsVisible = false;
            error.IsVisible = e.NetworkAccess != NetworkAccess.Internet;
        }

        private async Task modifyAddress()
        {
            try
            {

                var getAddresseses = (await MobileService.GetTable<TBL_Addresses>().Where(add => add.id == _selectedAddressId).ToListAsync()).FirstOrDefault();
                addressInfo.BindingContext = getAddresseses;
                var center = new Position(order_lat, order_long);
                var span = new MapSpan(center, 0.001, 0.001);
                map.MoveToRegion(span);

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
                error_wifi.IsVisible = true;
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
                error_wifi.IsVisible = true;
            }
        }

        private async void Btnsetdelivery_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (txtnotes.Text == "" || txtbuilding.Text == "")
                {
                    await DisplayAlert("Notes", "Please enter a notes and building name with specific instruction for our riders.", "OK");
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
                error_wifi.IsVisible = true;
            }
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
            error_wifi.IsVisible = false;
            error.IsVisible = Connectivity.NetworkAccess != NetworkAccess.Internet;
        }
    }
}