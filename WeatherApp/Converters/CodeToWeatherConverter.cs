using System.Globalization;

namespace WeatherApp.Converters
{
    public class CodeToWeatherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var code = (int)value;

            return code switch
            {
                0 => "Clear Sky",
                1 => "Mainly Clear",
                2 => "Partly Cloudy",
                3 => "Overcast",
                45 => "Fog",
                48 => "Depositing rime fog",
                51 => "Drizzle: Light",
                53 => "Drizzle: Moderate",
                55 => "Drizzle: Dense Intensity",
                56 => "Freezing Drizzle: Light",
                57 => "Freezing Drizzle: Dense Intensity",
                61 => "Rain: Slight",
                63 => "Rain: Moderate",
                65 => "Rain: Heavy Intensity",
                66 => "Freezing Rain: Light",
                67 => "Freezing Rain: Heavy intensity",
                71 => "Snow fall: Slight",
                73 => "Snow fall: Moderate",
                75 => "Snow fall: Heavy intensity",
                77 => "Snow grains",
                80 => "Rain showers: Slight",
                81 => "Rain showers: Moderate",
                82 => "Rain showers: Violent",
                85 => "Snow showers: Slight",
                86 => "Snow showers: Heavy",
                95 => "Thunderstorm: Slight or moderate",
                96 or 99 => "Thunderstorm with slight and heavy hail",
                _ => "Unknown"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

