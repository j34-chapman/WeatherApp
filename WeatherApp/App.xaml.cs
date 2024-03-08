using WeatherApp.MVVM.Views;

namespace WeatherApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new NavigationPage(new LoginView());
        //MainPage = new WeatherView();
    }
}

