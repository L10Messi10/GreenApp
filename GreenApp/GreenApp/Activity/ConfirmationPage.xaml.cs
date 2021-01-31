using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using ZXing.PDF417.Internal;
using static GreenApp.App;
using static Xamarin.Forms.LayoutOptions;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class ConfirmationPage : ContentPage
    {
        public ConfirmationPage()
        {
            InitializeComponent();
        }

        protected override async void OnDisappearing()
        {
            if (checkout)
            {
                historyloaded = false;
                CurrentOrderId = null;
                refresh = false;
                int numModals = Application.Current.MainPage.Navigation.ModalStack.Count;
                for (int currModal = 0; currModal < numModals; currModal++)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    //refresh = false;
                }
            }
            else
            {
                Selected_orderID = null;
                //refresh = false;
                int numModals = Application.Current.MainPage.Navigation.ModalStack.Count;
                for (int currModal = 0; currModal < numModals; currModal++)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    //refresh = false;
                }
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                if (!checkout)
                {
                    var barcode = new ZXingBarcodeImageView
                    {
                        HorizontalOptions = CenterAndExpand,
                        VerticalOptions = CenterAndExpand,
                        AutomationId = "zxingBarcodeImageView"
                    };
                    barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
                    barcode.BarcodeValue = Selected_orderID;
                    barcode.BarcodeOptions.Height = 150;
                    barcode.BarcodeOptions.Width = 150;
                    barcode.WidthRequest = 150;
                    barcode.HeightRequest = 150;
                    stackpanel.Children.Insert(0, barcode);
                    var getorders = (await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == Selected_orderID).ToListAsync()).FirstOrDefault();
                    mainlayout.BindingContext = getorders;
                    if (getorders != null)
                    {
                        lblorderid.Text = getorders.order_id;
                        lblfullname.Text = getorders.full_name;
                        lbltotpayable.Text = getorders.tot_payable;
                    }
                    else
                    {
                        lblorderid.Text = "Null";
                        lblfullname.Text = "Null";
                        lbltotpayable.Text = "Null";
                    }
                }
                else
                {
                    var barcode = new ZXingBarcodeImageView
                    {
                        HorizontalOptions = CenterAndExpand,
                        VerticalOptions = CenterAndExpand,
                        AutomationId = "zxingBarcodeImageView"
                    };
                    barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
                    barcode.BarcodeValue = CurrentOrderId;
                    barcode.BarcodeOptions.Height = 300;
                    barcode.BarcodeOptions.Width = 300;
                    barcode.WidthRequest = 300;
                    barcode.HeightRequest = 300;
                    stackpanel.Children.Insert(0, barcode);
                    var getorders = (await MobileService.GetTable<V_Orders>().Where(orders => orders.order_id == CurrentOrderId).ToListAsync()).FirstOrDefault();
                    mainlayout.BindingContext = getorders;
                    if (getorders != null)
                    {
                        lblorderid.Text = getorders.order_id;
                        lblfullname.Text = getorders.full_name;
                        lbltotpayable.Text = getorders.tot_payable;
                    }
                    else
                    {
                        lblorderid.Text = "Null";
                        lblfullname.Text = "Null";
                        lbltotpayable.Text = "Null";
                    }
                }
                
            }
            catch
            {
                await Navigation.PushAsync(new NoInternetPage(), true);
            }
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            if (checkout)
            {
                historyloaded = false;
                CurrentOrderId = null;
                refresh = false;
                int numModals = Application.Current.MainPage.Navigation.ModalStack.Count;
                for (int currModal = 0; currModal < numModals; currModal++)
                {
                    //refresh = false;
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    
                }
            }
            else
            {

                Selected_orderID = null;
                //refresh = false;
                int numModals = Application.Current.MainPage.Navigation.ModalStack.Count;
                for (int currModal = 0; currModal < numModals; currModal++)
                {
                    //refresh = false;
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    
                }
            }
            //await Navigation.PushAsync(new MenuPage(),true);
        }
    }
}