using MacroMate.View;

namespace MacroMate
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var appShell = new NavigationPage(new MainPage());
            MainPage = appShell;
        }
    }
}
