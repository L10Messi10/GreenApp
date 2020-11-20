using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GreenApp.Activity;
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
        }
        public ICommand OnLogoutCommand => new Command(OnLogout);

        private async void OnLogout()
        {
            user_id = null;
            CurrentOrderId = null;
            refresh = false;
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            await Navigation.PushAsync(new LoginPage(), true);
        }
    }
}