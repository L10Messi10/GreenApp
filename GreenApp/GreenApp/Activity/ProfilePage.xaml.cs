using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenApp.Models;
using Plugin.Media;
using Azure.Storage.Blobs;
using Azure.Storage;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GreenApp.App;

namespace GreenApp.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private string _imgId;
        private string _url;
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                var getorders = (await MobileService.GetTable<TBL_Orders>().Where(orders => orders.users_id == user_id).ToListAsync());
                OrdersList.ItemsSource = getorders;

                var profile = (await MobileService.GetTable<TBL_Users>().Where(prof => prof.Id == user_id).ToListAsync()).FirstOrDefault();
                profilegrid.BindingContext = profile;
            }
            catch
            {
                await DisplayAlert("Error", "Error processing your request, please check you internet connection.", "OK");
            }
        }

        private async void Btnbrowseimage_OnClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Camera", "Upload is not supported on this device", "OK");
                return;
            }
            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            if (selectedImageFile == null)
            {
                //await DisplayAlert("Error", "There was an error trying to get the image.", "OK");
                return;
            }
            profileimg.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
            await UploadImage(selectedImageFile.Path);
        }
        //private async void UploadImage(string getPath)
        //{
        //    var connectionString = "DefaultEndpointsProtocol=https;AccountName=imarketimagestorage;AccountKey=e7Panrt3KDC5WRWIq7jE3ZQ6kP6iDesjUkvWkQanU1oSYHxrgOEcCu528Tg9vA2Ra7Birbu5WOv57ga5pE9puw==;EndpointSuffix=core.windows.net";
        //    var containerName = "imagestorage";
        //    var filepath = getPath;
        //    var container = new BlobContainerClient(connectionString, containerName);
        //    await container.CreateIfNotExistsAsync();

        //    var name = Guid.NewGuid().ToString();
        //    await container.UploadBlobAsync($"{name}.jpg", File.OpenRead(filepath));
        //    string url = container.Uri.OriginalString;
        //}
        private async Task UploadImage(string getPath)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=imarketimagestorage;AccountKey=oQsfLFmDDr+FAKDyzKTeccautiNEJJDsfG4fzMJwbK2tO2q2gUrpaVowrfz7Q+dWWLAASPbdtCvMpEpE9/R8/A==;EndpointSuffix=core.windows.net";
            var containerName = "imagecontainer";
            var filepath = getPath;
            var container = new BlobContainerClient(connectionString, containerName);
            await container.CreateIfNotExistsAsync();
            //*************************
            //string a = "8b7b564b-4a34-4a34-aee7-5741b8f92ece.jpg";
            BlobClient picuri = container.GetBlobClient(picstr);
            if (picuri.Name != "")
            {
                await picuri.DeleteAsync();
            }
            _imgId = Guid.NewGuid().ToString();
            await container.UploadBlobAsync($"{_imgId}.jpg", File.OpenRead(filepath));
            _url = container.Uri.OriginalString;
            await Updateprofile();
        }
        //public static async Task<bool> DeleteFileAsync(ContainerType containerType, string name)
        //{
        //    var container = GetContainer(containerType);
        //    var blob = container.GetBlobReference(name);
        //    return await blob.DeleteIfExistsAsync();
        //}
        private async Task Updateprofile()
        {
            var user = new TBL_Users
            {
                Id = user_id,
                full_name = fullname,
                address = address,
                mobile_num = mobilenum,
                emailadd = emailadd,
                password = password,
                datereg = datereg,
                propic = $"{_url}/{_imgId}.jpg",
                picstr = $"{_imgId}.jpg"
            };
            propic = user.propic;
            picstr = user.picstr;
            await TBL_Users.Update(user);
        }

        private async void Btneditprofile_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfilePage(),true);
        }

        private async void Btnsecuritysettings_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditSecurityPage(), true);
        }
    }
}