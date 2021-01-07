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
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
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
            Geocoder geoCoder = new Geocoder();
            var m = (Map) sender;

            if (m.VisibleRegion != null)
            {
                var center = new Position(m.VisibleRegion.Center.Latitude, m.VisibleRegion.Center.Longitude);
                IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(center);
                var enumerable = possibleAddresses as string[] ?? possibleAddresses.ToArray();
                picker.ItemsSource = enumerable.ToList();
                picker.SelectedIndex = 1;
                order_lat = m.VisibleRegion.Center.Latitude;
                order_long = m.VisibleRegion.Center.Longitude;
                //await DisplayAlert ("Alert","Lat: " + m.VisibleRegion.Center.Latitude.ToString() + " Lon:" + m.VisibleRegion.Center.Longitude.ToString() + " ","OK");
            }
        }
        
        protected override async void OnDisappearing()
        {
            var locator = CrossGeolocator.Current;
            Geocoder geoCoder = new Geocoder();
            locator.PositionChanged -= Locator_PositionChanged;
            await locator.StopListeningAsync();
        }

        protected override async void OnAppearing()
        {
            if (_newAdd)
            {
                //new address
            }
            else
            {
                //modify address
            }
            //This gets the current location of the user's device.
            //await DisplayAlert("Info", "The app will detect your current location. Please allow the app to access your location.", "OK");
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(new TimeSpan(0), 200);


            var position = await locator.GetPositionAsync();
            var center = new Position(position.Latitude, position.Longitude);
            
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(200)));
        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            order_lat = e.Position.Latitude;
            order_long = e.Position.Longitude;
            var center = new Position(e.Position.Latitude, e.Position.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(200)));
        }

        private async void Btnsetdelivery_OnClicked(object sender, EventArgs e)
        {
            string c_add;
            int selectedIndex = picker.SelectedIndex;

            c_add = "" + txtstreet.Text + ", " + txtfloor.Text + " "+ (string)picker.ItemsSource[selectedIndex];
            if (_label != null)
            {
                if (txtstreet != null && txtfloor.Text != null && txtnotes.Text != null)
                {
                    progressplaceorder.IsVisible = true;
                    var addrress = new TBL_Addresses
                    {
                        user_id = user_id,
                        Address = c_add,
                        add_lat = order_lat,
                        add_long = order_long,
                        Label = _label,
                        Notes = txtnotes.Text
                    };
                    await TBL_Addresses.Insert(addrress);
                }
                else
                {
                    progressplaceorder.IsVisible = true;
                    var addrress = new TBL_Addresses
                    {
                        user_id = user_id,
                        Address = (string)picker.ItemsSource[selectedIndex],
                        add_lat = order_lat,
                        add_long = order_long,
                        Label = _label,
                    };
                    await TBL_Addresses.Insert(addrress);
                }
                
                await DisplayAlert("Info", "New address added. You can now choose this address where you want the items to be delivered.", "OK");
                await Navigation.PopAsync(true);
            }
            else
            {
                await DisplayAlert("Alert", "Please select a label for this address.", "OK");
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
    }
}