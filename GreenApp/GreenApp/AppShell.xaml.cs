using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GreenApp.Activity;
using GreenApp.Models;
using GreenApp.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;
using static Xamarin.Forms.Application;

namespace GreenApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            //Navigation.PushAsync(new MenuPage());
            //GetProfile();
        }
        public ICommand OnLogoutCommand => new Command(OnLogout);
        public ICommand OnProfileCommand => new Command(OnProfile);
        public ICommand OnAddressCommand => new Command(OnAddress);
        public ICommand OnOrdersCommand => new Command(OnOrders);
        public ICommand OnFeedBackCommand => new Command(OnFeedback);
        public ICommand OnAboutCommand => new Command(OnAbout);

        private async void OnLogout()
        {
            user_id = null;
            CurrentOrderId = null;
            refresh = false;
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            await Navigation.PushAsync(new LoginPage(), true);
        }
        private async void OnProfile()
        {
            Current.FlyoutIsPresented = false;
            await Navigation.PushAsync(new ProfilePage(), true);

        }
        private async void OnAddress()
        {
            Current.FlyoutIsPresented = false;
            await Navigation.PushAsync(new AddressesPage(), true);

        }
        private async void OnOrders()
        {
            Current.FlyoutIsPresented = false;
            await Navigation.PushAsync(new OrderHistoryPage(), true);

        }
        private async void OnFeedback()
        {
            Current.FlyoutIsPresented = false;
            await Navigation.PushAsync(new FeedBackPage(), true);

        }
        private async void OnAbout()
        {
            Current.FlyoutIsPresented = false;
            await Navigation.PushAsync(new AboutPage(), true);

        }
    }
}