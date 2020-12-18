using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Map = Xamarin.Essentials.Map;
using Position = Xamarin.Forms.Maps.Position;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackOrderPage : ContentPage
    {
        public TrackOrderPage()
        {
            InitializeComponent();
        }

        #region Map Identification for user's location

        //Identifying the location of the user in the Map
        //protected override async void OnAppearing()
        //{

        //    base.OnAppearing();
        //    var locator = CrossGeolocator.Current;
        //    locator.PositionChanged += Locator_PositionChanged;
        //    await locator.StartListeningAsync(new TimeSpan(0), 100);
        //    var position = await locator.GetPositionAsync();

        //    var center = new Position(position.Latitude, position.Longitude);
        //    var span = (new MapSpan(center, 2, 2));
        //    LocationsMap.MoveToRegion(span);
        //    using (SQLiteConnection conn = new SQLiteConnection(App.Databaselocation))
        //    {
        //        conn.CreateTable<Post>();
        //        var posts = conn.Table<Post>().ToList();

        //        DisplayInMap(posts);
        //    }
        //}

        //protected override async void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    var locator = CrossGeolocator.Current;
        //    locator.PositionChanged -= Locator_PositionChanged;
        //    await locator.StopListeningAsync();
        //}

        //private void DisplayInMap(List<Post> posts)
        //{
        //    foreach (var post in posts)
        //    {
        //        try
        //        {


        //            var position = new Position(post.Latitude, post.Longitude);
        //            var pin = new Pin()
        //            {
        //                Type = PinType.SavedPin,
        //                Position = position,
        //                Label = post.VenueName,
        //                Address = post.Address
        //            };
        //            LocationsMap.Pins.Add(pin);
        //        }
        //        catch (NullReferenceException nre) { }
        //        catch (Exception ex) { }
        //    }
        //}

        //private void Locator_PositionChanged(object sender, PositionEventArgs e)
        //{
        //    var center = new Position(e.Position.Latitude, e.Position.Longitude);
        //    var span = (new MapSpan(center, 2, 2));
        //    LocationsMap.MoveToRegion(span);
        //}

        #endregion
    }
}