using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TinifyAPI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Champinator
{
    public class Predictions
    {
        public double probability { get; set; }
        public string tagName { get; set; }
    }
    public class Response
    {
        public List<Predictions> predictions { get; set; }
    }
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Data : ContentPage
	{
        private static readonly HttpClient client = new HttpClient();
        private static string predKey = "f011cedabc6749a6bebd8cede806948d";
        private static string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/78c25564-6154-4feb-a40d-164746a18fae/image";
        public Response ListPredictions { get; set; }
        public Data (ref Stream picture,ref ImageSource jj)
		{
            InitializeComponent();
            var j = picture;
            PhotoImage.Source = jj;
            DetectShroom(j);
        }
        public async void DetectShroom(Stream i)
        {
            Tinify.Key= "0sIgAauHE8NFGOksrxYXO9p9kVcDocLc";
            var sourceData = File.ReadAllBytes(i.ToString());
            var resultData = await Tinify.FromBuffer(sourceData).ToBuffer();
            client.DefaultRequestHeaders.Add("Prediction-Key", predKey);
            //client.DefaultRequestHeaders.Add("Content-Type", contentType);
            ByteArrayContent byteContent = new ByteArrayContent(resultData);
            Response predictions = new Response();
            try
            {
                var response1 = await client.PostAsync(url, byteContent);
                var responseString1 = await response1.Content.ReadAsStringAsync();
                predictions = JsonConvert.DeserializeObject<Response>(responseString1);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            if (predictions.predictions[1].probability > predictions.predictions[0].probability)
            {
                Response predictionsSwap = new Response();
                List<Predictions> list = new List<Predictions>();
                for (int j = predictions.predictions.Count - 1; j >= 0; j--)
                {
                    list.Add(predictions.predictions[j]);
                }
                predictionsSwap.predictions = list;
                predictions = predictionsSwap;
            }
            ListPredictions = predictions;
            ListPoss.ItemsSource = ListPredictions.predictions;
        }
        private static byte[] Compress(Stream input)
        {
            using (var compressStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
            {
                input.CopyTo(compressor);
                compressor.Close();
                return compressStream.ToArray();
            }
        }
    }
}