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

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryLocationPage : ContentPage
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
           

            base.OnAppearing();
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(new TimeSpan(0), 100);


            var position = await locator.GetPositionAsync();
            var center = new Position(position.Latitude, position.Longitude);
            var span = new MapSpan(center, 2, 2);
            Pin locationPin = new Pin()
            {
                Label = "My location",
                Type = PinType.Place,
                Icon = (Device.RuntimePlatform == Device.Android ? BitmapDescriptorFactory.FromBundle("marker.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "marker.png", WidthRequest = 30, HeightRequest = 30 })),
                Position = center,
                IsDraggable = true
            };
            map.Pins.Add(locationPin);
            map.MoveToRegion(span);
        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var center = new Position(e.Position.Latitude, e.Position.Longitude);
            var span = new MapSpan(center, 2, 2);
            map.MoveToRegion(span);
        }
        private async void Map_OnPinDragEnd(object sender, PinDragEventArgs e)
        {
           var position = new Position(e.Pin.Position.Latitude,e.Pin.Position.Longitude);
           map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(500)));
           await DisplayAlert("Alert", "Pickup location: Latitude: " + e.Pin.Position.Latitude + " Longitude: " + e.Pin.Position.Longitude, "OK");
        }
    }
}