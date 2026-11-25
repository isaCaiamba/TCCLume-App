using TCCLume.Services;
using TCCLume.Views;

namespace TCCLume;

public partial class App : Application
{
    public App(DatabaseService database)
    {
        InitializeComponent();

        MainPage = new NavigationPage(new SplashPage());
    }
}