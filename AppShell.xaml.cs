namespace BE_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Settings", typeof(SettingsPage));
            Routing.RegisterRoute("Game", typeof(GamePage));
            Routing.RegisterRoute("Tutorial", typeof(TutorialPage));
        }
    }
}
