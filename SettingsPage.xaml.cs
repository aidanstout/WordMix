namespace BE_App;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        Console.WriteLine("settings page");
		
	}
    private async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync("Game");
        System.Diagnostics.Debug.WriteLine("going to game");

    }
    void OnToggled(object sender, ToggledEventArgs e)
    {
        // Perform an action after examining e.Value
    }
}