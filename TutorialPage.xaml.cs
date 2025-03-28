namespace BE_App;

public partial class TutorialPage : ContentPage
{
	public TutorialPage()
	{
		InitializeComponent();
	}

	private async void Button_Clicked(System.Object sender, System.EventArgs e)
	{
		await Shell.Current.GoToAsync("Game");

	}

}