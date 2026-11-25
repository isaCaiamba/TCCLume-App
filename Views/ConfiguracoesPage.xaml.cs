using TCCLume.Models;
using TCCLume.Services;

namespace TCCLume.Views;

public partial class ConfiguracoesPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly User _user;

    public ConfiguracoesPage(User user)
    {
        InitializeComponent();

        _database = new DatabaseService();
        _user = user;
    }


    // =========================================================
    // CARREGAR INFORMAÇÕES
    // =========================================================

    private void CarregarInformacoes()
    {
        NameLabel.Text =
            string.IsNullOrWhiteSpace(_user.Name)
                ? "Usuário"
                : _user.Name;

        EmailLabel.Text =
            _user.Email ?? "E-mail não informado";
    }


    // =========================================================
    // ALTERAR NOME
    // =========================================================

    private async void AlterarNome_Tapped(
        object sender,
        TappedEventArgs e)
    {
        string? novoNome =
            await DisplayPromptAsync(
                "Alterar nome",
                "Digite seu novo nome:",
                "Salvar",
                "Cancelar",
                initialValue: _user.Name);

        if (string.IsNullOrWhiteSpace(novoNome))
            return;

        _user.Name = novoNome.Trim();

        await _database.UpdateUser(_user);

        NameLabel.Text = _user.Name;

        await DisplayAlert(
            "Sucesso",
            "Seu nome foi alterado.",
            "OK");
    }


    // =========================================================
    // ALTERAR E-MAIL
    // =========================================================

    private async void AlterarEmail_Tapped(
        object sender,
        TappedEventArgs e)
    {
        string? novoEmail =
            await DisplayPromptAsync(
                "Alterar e-mail",
                "Digite seu novo e-mail:",
                "Salvar",
                "Cancelar",
                initialValue: _user.Email);

        if (string.IsNullOrWhiteSpace(novoEmail))
            return;

        novoEmail = novoEmail.Trim();

        if (!novoEmail.Contains("@"))
        {
            await DisplayAlert(
                "E-mail inválido",
                "Digite um e-mail válido.",
                "OK");

            return;
        }

        var usuarioExistente =
            await _database.GetUserByEmail(novoEmail);

        if (usuarioExistente != null &&
            usuarioExistente.Id != _user.Id)
        {
            await DisplayAlert(
                "E-mail já cadastrado",
                "Esse e-mail já está sendo utilizado por outra conta.",
                "OK");

            return;
        }

        _user.Email = novoEmail;

        await _database.UpdateUser(_user);

        EmailLabel.Text = _user.Email;

        await DisplayAlert(
            "Sucesso",
            "Seu e-mail foi alterado.",
            "OK");
    }


    // =========================================================
    // ALTERAR SENHA
    // =========================================================

    private async void AlterarSenha_Tapped(
        object sender,
        TappedEventArgs e)
    {
        string? senhaAtual =
            await DisplayPromptAsync(
                "Alterar senha",
                "Digite sua senha atual:",
                "Continuar",
                "Cancelar");

        if (string.IsNullOrWhiteSpace(senhaAtual))
            return;

        if (senhaAtual != _user.Password)
        {
            await DisplayAlert(
                "Senha incorreta",
                "A senha atual está incorreta.",
                "OK");

            return;
        }

        string? novaSenha =
            await DisplayPromptAsync(
                "Nova senha",
                "Digite sua nova senha:",
                "Continuar",
                "Cancelar");

        if (string.IsNullOrWhiteSpace(novaSenha))
            return;

        if (novaSenha.Length < 8)
        {
            await DisplayAlert(
                "Senha inválida",
                "A senha deve possuir pelo menos 8 caracteres.",
                "OK");

            return;
        }

        string? confirmarSenha =
            await DisplayPromptAsync(
                "Confirmar senha",
                "Digite novamente a nova senha:",
                "Salvar",
                "Cancelar");

        if (novaSenha != confirmarSenha)
        {
            await DisplayAlert(
                "Senhas diferentes",
                "As senhas não coincidem.",
                "OK");

            return;
        }

        _user.Password = novaSenha;

        await _database.UpdateUser(_user);

        await DisplayAlert(
            "Sucesso",
            "Sua senha foi alterada.",
            "OK");
    }

    // =========================================================
    // SAIR
    // =========================================================

    private async void Sair_Clicked(
    object sender,
    EventArgs e)
    {
        bool confirmar =
            await DisplayAlert(
                "Sair da conta",
                "Deseja realmente sair da sua conta?",
                "Sair",
                "Cancelar");

        if (!confirmar)
            return;

        // Limpa os campos da tela de login
        Preferences.Remove("EmailLogin");
        Preferences.Remove("SenhaLogin");

        // Volta para o Login
        await Navigation.PopToRootAsync();
    }


    // =========================================================
    // MENU
    // =========================================================

    private async void MenuButton_Clicked(
        object sender,
        EventArgs e)
    {
        string action =
            await DisplayActionSheet(
                "Menu",
                "Cancelar",
                null,
                "Dashboard",
                "Lume",
                "Minhas Economias",
                "Resumo Mensal",
                "Blog");

        if (action == "Dashboard")
        {
            await Navigation.PushAsync(
                new DashboardPage(_user));

            return;
        }

        if (action == "Lume")
        {
            await Navigation.PushAsync(
                new ChatPage(_user));

            return;
        }

        if (action == "Minhas Economias")
        {
            await Navigation.PushAsync(
                new MinhasEconomiasPage(_user));

            return;
        }

        if (action == "Resumo Mensal")
        {
            await Navigation.PushAsync(
                new ResumoMensalPage(_user));

            return;
        }

        if (action == "Blog")
        {
            await Navigation.PushAsync(
                new BlogPage(_user));
        }
    }
}