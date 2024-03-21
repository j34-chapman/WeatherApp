using WeatherApp.MVVM.ViewModel;

namespace WeatherApp.MVVM.Views;

public partial class WeatherView : ContentPage
{
    public WeatherView()
    {
        InitializeComponent();
        BindingContext = new WeatherViewModel();

        NavigationPage.SetHasNavigationBar(this, false);


    }
}
