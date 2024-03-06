
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using WeatherApp.MVVM.Model;

namespace WeatherApp.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]


    public class WeatherViewModel 
    {
        

        public WeatherData WeatherData { get; set; }
        public Current current { get; set; }
        public string PlaceName { get; set; }
        public DateTime Date { get; set; } =
             DateTime.Now;

        public bool IsVisible { get; set; }
        public bool IsLoading { get; set; }

        private HttpClient client;

        public WeatherViewModel()
        {
            client = new HttpClient();

            RequestLocationPermission();

            // Move the logic to get current location and weather here
            GetCurrentLocationAndWeather();


        }

        public ICommand SearchCommand =>
             new Command(async (searchText) =>
             {
                 PlaceName = searchText.ToString();
                 var location =
                        await GetCoordinatesAsync(searchText.ToString());
                 await GetWeather(location);
             });



        private async void RequestLocationPermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                {
                    // Location permission is already granted, proceed to get weather details
                    GetCurrentLocationAndWeather();
                }
                else
                {
                    // Request location permission
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                    if (status == PermissionStatus.Granted)
                    {
                        // Location permission granted, proceed to get weather details
                        GetCurrentLocationAndWeather();
                    }
                    else
                    {
                        // Handle the case where location permission is denied
                        Console.WriteLine("Location permission denied.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, if any
                Console.WriteLine($"Error requesting location permission: {ex.Message}");
            }
        }



        private async Task GetCurrentLocationAndWeather()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    // Use the obtained location to fetch weather details
                    await GetWeather(location);
                }
                else
                {
                    // If last known location is not available, try to get the current location
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);

                    if (location != null)
                    {
                        // Use the obtained location to fetch weather details
                        await GetWeather(location);
                    }
                    else
                    {
                        // Handle the case where current location is not available
                        Console.WriteLine("Unable to determine the current location.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, if any
                Console.WriteLine($"Error getting current location: {ex.Message}");
            }
        }


        private async Task GetWeather(Location location)
        {
            var url =
                 $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m,weather_code,wind_speed_10m&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=Europe%2FLondon";
         
            var response =
              await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var data = await JsonSerializer
                         .DeserializeAsync<WeatherData>(responseStream);
                        WeatherData = data;
                    for (int i = 0; i< WeatherData.daily.time.Length; i++)
                    {
                        var nextdays = new NextDays
                        {
                            time = WeatherData.daily.time[i],
                            weather_code = WeatherData.daily.weather_code[i],
                            temperature_2m_max = WeatherData.daily.temperature_2m_max[i],
                            temperature_2m_min = WeatherData.daily.temperature_2m_min[i],

                        };
                        WeatherData.nextdays.Add(nextdays);

                    }



                    
                    
                    //Console.Write($"Weather: {WeatherData.ToString()}");
                }
            }
       
        }

        private async Task<Location> GetCoordinatesAsync(string address)
        {
            IEnumerable<Location> locations = await Geocoding
                 .Default.GetLocationsAsync(address);

            Location location = locations?.FirstOrDefault();

            if (location != null)
                Console
                     .WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
            return location;
        }

    }
}

