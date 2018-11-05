using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Champinator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;
        }
        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                var stream = photo.GetStream();
                var imageS = ImageSource.FromStream(() => { return photo.GetStream(); });
                Data d = new Data(ref stream,ref imageS);
                await Navigation.PushModalAsync(d);
            }
        }
    }
}
