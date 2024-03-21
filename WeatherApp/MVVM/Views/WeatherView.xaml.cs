using WeatherApp.MVVM.ViewModel;

namespace WeatherApp.MVVM.Views;

public partial class WeatherView : ContentPage
{
	public WeatherView()
	{
		InitializeComponent();
		BindingContext = new WeatherViewModel();


        // Hide the navigation bar
        NavigationPage.SetHasNavigationBar(this, false);
    }

	}

