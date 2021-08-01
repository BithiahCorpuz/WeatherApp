using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Helper;
using Weather.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetRootobject();
        }
        private string Location { get; set; } = "Thailand";
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    Location = await GetCity(location);

                    GetRootobject();
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }

        private async Task<string> GetCity(Location location)
        {
            var place = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = place?.FirstOrDefault();

            if (currentPlace != null)
            
                return $"{currentPlace.Locality},{currentPlace.CountryName}";

            return null;

        }

        private async void GetBackground()
        {
            var url = $"https://api.pexels.com/v1/search?query={Location}&per_page=1";

            var result = await ApiCaller.Get(url, "563492ad6f9170000100000146a03ed0916d4e08a73680f5029e5eef"); 
            
            if (result.Successful)
            {
                var bgInfo = JsonConvert.DeserializeObject<BackgroundInfo>(result.Response);
                  if (bgInfo != null && bgInfo.photos.Length > 0)
                    BgImg.Source = ImageSource.FromUri(new Uri(bgInfo.photos 
                        [new Random().Next(0, bgInfo.photos.Length -1)].src.medium));   
            }
        }


        private async void GetRootobject()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={Location}&appid=30bde5553ad9d1ac30c74600d50be816&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var rootobject = JsonConvert.DeserializeObject<Rootobject>(result.Response);
                    descriptionTxt.Text = rootobject.weather[0].description.ToUpper();
                    iconsImg.Source = $"w{rootobject.weather[0].icon}";
                    cityTxt.Text = rootobject.name.ToUpper();
                    temperatureTxt.Text = rootobject.main.temp.ToString();
                    humidityTxt.Text = $"{rootobject.main.humidity}%";
                    pressureTxt.Text = $"{rootobject.main.pressure}hPa";
                    windTxt.Text = $"{rootobject.wind.speed}m/s";
                    cloudinessTxt.Text = $"{rootobject.clouds.all}%";

                    var dt = new DateTime().ToUniversalTime().AddSeconds(rootobject.dt);
                    dateTxt.Text = dt.ToString("MMMM, dddd, dd").ToUpper();

                    GetForecast();
                    GetBackground();

                }
                catch (Exception ex)
                {

                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No Info Available", "Ok");
            }
        }
        private async void GetForecast()
        {
            var url = $"https://pro.openweathermap.org/data/2.5/forecast/climate?q={Location}&appid=30bde5553ad9d1ac30c74600d50be816&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var foreCastInfo = JsonConvert.DeserializeObject<ForeCastInfo>(result.Response);
                    List<List> allList = new List<List>();

                    foreach (var list in foreCastInfo.list)
                    {
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    //dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    //dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("MM dd");
                    //iconOneImg.Source = $"w{allList[0].weather.icon}";
                    //TempOneTxt.Text = allList[0].main.temp.ToString();

                   // dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("MM dd");
                   // dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MM");
                   // iconTwoImg.Source = $"w{allList[1].weather.icon}";
                   // tempTwoTxt.Text = allList[1].main.temp.ToString();

                   // dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("MM dd");
                   // dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MM");
                   // iconThreeImg.Source = $"w{allList[2].weather.icon}";
                   // tempThreeTxt.Text = allList[2].main.temp.ToString();

                   // dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("MM dd");
                   //dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MM");
                   //iconFourImg.Source = $"w{allList[3].weather.icon}";
                   //tempFourTxt.Text = allList[3].main.temp.ToString();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "ok");
                }
            }
            
                
        }
    }
}













