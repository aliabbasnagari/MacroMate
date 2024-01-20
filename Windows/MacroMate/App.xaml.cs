using MacroMate.View;

namespace MacroMate
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var appShell = new NavigationPage(new Profiles("192.168.0.98", 7214));
            MainPage = appShell;
        }
    }
}
