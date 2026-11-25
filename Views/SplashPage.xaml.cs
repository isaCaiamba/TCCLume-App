
using TCCLume.Services;

namespace TCCLume.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Estado inicial da animação
        LogoImage.Scale = 0.5;
        AppName.TranslationY = 15;
        Subtitle.TranslationY = 10;

        await Task.Delay(300);

        // Animação da logo
        await Task.WhenAll(
            LogoImage.FadeTo(1, 1000, Easing.CubicOut),
            LogoImage.ScaleTo(1, 1000, Easing.CubicOut)
        );

        // Animação do nome
        await Task.WhenAll(
            AppName.FadeTo(1, 700, Easing.CubicOut),
            AppName.TranslateTo(0, 0, 700, Easing.CubicOut)
        );

        // Animação da frase
        await Task.WhenAll(
            Subtitle.FadeTo(1, 600, Easing.CubicOut),
            Subtitle.TranslateTo(0, 0, 600, Easing.CubicOut)
        );

        // Pequena pausa antes de abrir o login
        await Task.Delay(900);

        // Abre o login
        Application.Current!.MainPage =
            new NavigationPage(
                new LoginPage(new DatabaseService()));
    }
}
