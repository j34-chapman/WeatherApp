﻿

using WeatherApp.MVVM.ViewModel;

namespace WeatherApp.MVVM.Views;

public partial class LoginView : ContentPage
{
    public LoginView()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();


    }

    public async void TakePhoto(object sender, EventArgs e)
    {
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
            await Shell.Current.DisplayAlert("Oops Not supported!", "Device isnt suppourted", "Ok");
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

        Navigation.PushAsync(new WeatherView());

        // Clear the navigation stack, making it a one-way navigation
        Application.Current.MainPage = new NavigationPage(new WeatherView());
    }



}






