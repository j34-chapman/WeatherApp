

using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsVisible { get; set; }
        public bool IsLoading { get; set; }

        private HttpClient client; // HttpClient for API requests

        // Constructor to initialize HttpClient and weather data
        public WeatherViewModel()
        {
            client = new HttpClient();
            InitializeWeatherData();
        }

        // Command for searching weather based on user input
        public ICommand SearchCommand =>
            new Command(async (searchText) =>
            {
                if (searchText is string text)
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    {
                        // No internet connection, show error message
                        await DisplayErrorMessage("No internet connection available. Please check your network settings.");
                        return;
                    }

                    // Check if the input is empty
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        // Input is empty, show error message
                        await DisplayErrorMessage("Place name cannot be empty.");
                        return;
                    }

                    // Check if the input contains a number
                    if (text.Any(char.IsDigit))
                    {
                        // Input contains a number, show error message
                        await DisplayErrorMessage("Place name cannot contain numbers.");
                        return;
                    }

                    PlaceName = text;
                    var location = await GetCoordinatesAsync(text);

                    await GetWeather(location, false); // Pass false to indicate not to update PlaceName
                }
                else
                {
                    // Input is null, show error message
                    await DisplayErrorMessage("Place name cannot be null.");
                }
            });

        // Method to display error message
        private async Task DisplayErrorMessage(string message)
        {
            await App.Current.MainPage.DisplayAlert("Error", message, "OK");
        }

        private async Task InitializeWeatherData()
        {
            bool arePermissionsGranted = await CheckLocationPermissions();

            if (arePermissionsGranted)
            {
                GetCurrentLocationAndWeather();
            }
            else
            {
                await DisplayErrorMessage("Permission to access location is required to fetch weather automatically.");
            }
        }

        //// Method to check location permissions 
        //private async Task<bool> CheckLocationPermissions()
        //{
        //    const int maxRetries = 3;
        //    const int delayMilliseconds = 500; // 500 milliseconds delay

        //    var whenInUseStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //    var alwaysStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

        //    if (whenInUseStatus == PermissionStatus.Granted || alwaysStatus == PermissionStatus.Granted)
        //    {
        //        return true; // Permissions are granted
        //    }

        //    for (int i = 0; i < maxRetries; i++)
        //    {
        //        // Request location permissions
        //        whenInUseStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        alwaysStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();

        //        // Wait for a short delay before checking permissions again
        //        await Task.Delay(delayMilliseconds);

        //        // Check if either of the permissions is granted after requesting
        //        whenInUseStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //        alwaysStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

        //        if (whenInUseStatus == PermissionStatus.Granted || alwaysStatus == PermissionStatus.Granted)
        //        {
        //            return true; // Permissions are granted
        //        }

        //        // Wait for a short delay before retrying
        //        await Task.Delay(delayMilliseconds);
        //    }

        //    // Permissions are not granted after max retries
        //    return false;
        //}

        private async Task<bool> CheckLocationPermissions()
        {
            const int maxRetries = 50;
            const int delayMilliseconds = 500; // 500 milliseconds delay

            for (int i = 0; i < maxRetries; i++)
            {
                var whenInUseStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                var alwaysStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

                if (whenInUseStatus == PermissionStatus.Granted || alwaysStatus == PermissionStatus.Granted)
                {
                    return true; // Permissions are granted
                }

                // Request location permissions
                var whenInUseRequest = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                var alwaysRequest = Permissions.RequestAsync<Permissions.LocationAlways>();

                await Task.WhenAny(whenInUseRequest, alwaysRequest);

                // Wait for a short delay before checking permissions again
                await Task.Delay(delayMilliseconds);
            }

            // Permissions are not granted after max retries
            return false;
        }







        //Method to get current location and weather
        public async Task GetCurrentLocationAndWeather()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();

                if (location != null)
                {
                    // Use the obtained location to fetch weather details
                    await GetWeather(location);
                }
                else
                {
                    // If current location is not available, try to get the last known location
                    location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        // Use the obtained last known location to fetch weather details
                        await GetWeather(location);
                    }
                    else
                    {
                        Console.WriteLine("Unable to determine the current location.");
                        await DisplayErrorMessage("Unable to determine the current location. Please check your location settings.");
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Console.WriteLine(fnsEx.Message);
                await DisplayErrorMessage("Location services are not supported on this device.");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Console.WriteLine(pEx.Message);
                await DisplayErrorMessage("Location permission is not granted. Please enable location services and try again.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error getting location: {ex.Message}");
                await DisplayErrorMessage($"Error getting location: {ex.Message}");
            }
        }



        private async Task GetWeather(Location location, bool updatePlaceName = true)
        {
            // Construct the API URL using latitude and longitude from the location parameter
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m,weather_code,wind_speed_10m&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=Europe%2FLondon";

            IsLoading = true; 

            
            var response = await client.GetAsync(url);

            
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a stream
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    // Deserialize the JSON response into a WeatherData object
                    var data = await JsonSerializer.DeserializeAsync<WeatherData>(responseStream);
                    WeatherData = data;

                    // Iterate through each day's weather forecast to populate the nextdays list
                    for (int i = 0; i < WeatherData.daily.time.Length; i++)
                    {
                        var nextdays = new NextDays
                        {
                            time = WeatherData.daily.time[i],
                            weather_code = WeatherData.daily.weather_code[i],
                            temperature_2m_max = WeatherData.daily.temperature_2m_max[i],
                            temperature_2m_min = WeatherData.daily.temperature_2m_min[i],
                        };
                        WeatherData.nextdays.Add(nextdays); // Add the forecast to the nextdays list
                    }

                    IsVisible = true; // Set IsVisible to true to indicate that weather data is available

                    // Update the place name if specified
                    if (updatePlaceName)
                    {
                        string locationName = await GetLocationNameAsync(location);
                        PlaceName = locationName; // Set the PlaceName property to the retrieved location name
                    }
                }
            }

            IsLoading = false; // Set IsLoading to false after weather data retrieval is complete
        }



        private async Task<string> GetLocationNameAsync(Location location)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    // You can customize this logic to prioritize city name or use other address components
                    var cityName = placemark.Locality ?? placemark.AdminArea;

                    return cityName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting location name: {ex.Message}");
            }

            return string.Empty;
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


