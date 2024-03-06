using System.Globalization;
using SkiaSharp.Extended.UI.Controls;

namespace WeatherApp.Converters
{
    public class CodeToImageConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var code = (int)value;
            var lottienImageSource = new SKFileLottieImageSource
            {
                File = code switch
                {
                    0 => "sunny.json",
                    1 or 2 or 3 => "partly-cloudy.json",
                    45 or 48 => "foggy.json",
                    51 or 53 or 55 or 56 or 57 => "partly-shower.json",
                    61 or 63 or 65 => "stormshowersday.json",
                    66 or 67 or 71 or 73 or 75 or 77 => "snow.json",
                    80 or 81 or 82 or 85 or 86 => "storm.json",
                    95 or 96 or 99 => "thunder.json",
                    _ => "partly-cloudy.json" // Set a default value for unknown codes
                }
            };

            return lottienImageSource;
        }

        public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}

