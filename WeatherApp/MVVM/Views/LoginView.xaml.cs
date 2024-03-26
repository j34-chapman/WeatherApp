using WeatherApp.MVVM.ViewModel;namespace WeatherApp.MVVM.Views;public partial class LoginView : ContentPage{    public LoginView()    {        InitializeComponent();        BindingContext = new LoginViewModel();        NavigationPage.SetHasNavigationBar(this, false);    }

    //Error message method
    private async Task DisplayErrorMessage(string message)
    {
        await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
    }



    public async void TakePhoto(object sender, EventArgs e)
    {
        // Check if camera permissions are granted
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            // Camera permissions are not granted, request permissions
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                // Camera permissions denied, show error message
                await DisplayAlert("Error", "Permission to access camera is required to take photos.", "OK");
                return;
            }
        }

        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult myPhoto = await MediaPicker.Default.CapturePhotoAsync();
            if (myPhoto != null)
            {
                // Convert the captured photo to a byte array
                byte[] imageData;
                using (var stream = await myPhoto.OpenReadAsync())
                {
                    var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }

                // Convert the byte array to an ImageSource
                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageData));

                // Update the Image property in the view model
                profileImage.Source = imageSource;

                // Show confirmation message
                await DisplayAlert("Success", "Photo captured and updated successfully.", "OK");
            }
            else
            {
                // Show error message if photo is null
                await DisplayAlert("Error", "Failed to capture photo.", "OK");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Oops Not supported!", "Device isn't supported", "Ok");
        }
    }    private async void Button_Clicked(object sender, EventArgs e)    {        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            // No internet connection, show error message
            await DisplayErrorMessage("No internet connection available. Please check your network settings.");
            return;
        }

         Navigation.PushAsync(new WeatherView());         //Clear the navigation stack, making it a one-way navigation         Application.Current.MainPage = new NavigationPage(new WeatherView());            }}