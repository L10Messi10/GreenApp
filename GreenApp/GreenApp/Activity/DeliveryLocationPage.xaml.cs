using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GreenApp.ViewModels;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryLocationPage
    {
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
        }

        private void ApplyMapTheme()
        {
            var assembly = typeof(DeliveryLocationPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"GreenApp.MapResources.MapTheme.json");
            string themeFile;
            using (var reader = new System.IO.StreamReader(stream))
            {
                themeFile = reader.ReadToEnd();
                map.MapStyle=MapStyle.FromJson(themeFile);
            }
        }

        protected override async void OnAppearing()
        {
            //This gets the current location of the user's device.
            del_long = 0;
            del_lat = 0;
            txtrecvrname.Text = fullname;
            txtrecvraddress.Text = address;
            txtrecvrnum.Text = mobilenum;
            await DisplayAlert("Info", "The app will detect your current location. Please allow the app to access your location.", "OK");
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(new TimeSpan(0), 200);


            var position = await locator.GetPositionAsync();
            var center = new Position(position.Latitude, position.Longitude);
            Pin locationPin = new Pin()
            {
                Label = "My location",
                Type = PinType.Place,
                Icon = (Device.RuntimePlatform == Device.Android ? BitmapDescriptorFactory.FromBundle("marker.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "marker.png", WidthRequest = 30, HeightRequest = 30 })),
                Position = center,
                IsDraggable = true
            };
            map.Pins.Add(locationPin);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(200)));
        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            del_lat = e.Position.Latitude;
            del_long = e.Position.Longitude;
            var center = new Position(e.Position.Latitude, e.Position.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(200)));
        }

        //This will let the user drag the pin and put it where he/she wants the idem to be delivered.
        private async void Map_OnPinDragEnd(object sender, PinDragEventArgs e)
        {
           var position = new Position(e.Pin.Position.Latitude,e.Pin.Position.Longitude);
           map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(100)));
           await DisplayAlert("Alert", "Delivery location: Latitude: " + e.Pin.Position.Latitude + " Longitude: " + e.Pin.Position.Longitude, "OK");
           del_lat = e.Pin.Position.Latitude;
           del_long = e.Pin.Position.Longitude;
        }

        private async void Btnsetdelivery_OnClicked(object sender, EventArgs e)
        {
            if (del_long == 0 || del_lat == 0)
            {
                await DisplayAlert("Alert", "Delivery location wasn't set.", "OK");
                return;
            }

            if (txtrecvrname.Text == null)
            {
                await DisplayAlert("Alert", "Please enter receiver's name!", "OK");
                return;
            }
            if (txtrecvrname.Text == null)
            {
                await DisplayAlert("Alert", "Please enter receiver's address!", "OK");
                return;
            }
            else
            {
                if (await DisplayAlert("Confirm Delivery info", "Are you sure this delivery infos are correct?", "Yes", "No"))
                {
                    order_choice = "Delivery";
                    order_rcvr_name = txtrecvrname.Text;
                    order_rcvr_add = txtrecvraddress.Text;
                    order_rcvr_num = txtrecvrnum.Text;
                    await Navigation.PopAsync(true);
                }
            }
        }
    }
}