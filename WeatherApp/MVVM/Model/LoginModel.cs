using System;
using PropertyChanged;

namespace WeatherApp.MVVM.Model
{

    [AddINotifyPropertyChangedInterface]

    public class LoginModel
	{
		public ImageSource Image { get; set; }
		
	}
}

