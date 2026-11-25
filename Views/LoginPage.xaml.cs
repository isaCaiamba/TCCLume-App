using TCCLume.Services;

namespace TCCLume.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _database;

    public LoginPage(DatabaseService database)
    {
        InitializeComponent();

        _database = database;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        var user = await _database.Login(email, password);

        if (user == null)
        {
            await DisplayAlert(
                "Erro",
                "E-mail ou senha inválidos.",
                "OK");

            return;
        }

        await DisplayAlert(
            "Bem-vindo ✨",
            $"Olá, {user.Name}!",
            "Entrar");

        await Navigation.PushAsync(
            new DashboardPage(user));
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TeladeCadastro(_database));
    }

    //ESCONDER E MOSTRAR SENHA
    private bool passwordVisible = false;

    private void PasswordVisibilityButton_Clicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

        if (PasswordEntry.IsPassword)
        {
            PasswordVisibilityButton.Source = "eye.png";
        }
        else
        {
            PasswordVisibilityButton.Source = "offeye.png";
        }
    }
}