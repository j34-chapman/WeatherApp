using WeatherApp.MVVM.ViewModel;

namespace WeatherApp.MVVM.Views;

public partial class WeatherView : ContentPage
{
	public WeatherView()
	{
		InitializeComponent();
		BindingContext = new WeatherViewModel();
<<<<<<< HEAD


        // Hide the navigation bar
        NavigationPage.SetHasNavigationBar(this, false);
    }

	}

=======
	}
}
>>>>>>> parent of 9f7942d (Working point with loading screens)
