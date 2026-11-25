using System.Text.RegularExpressions;
using TCCLume.Models;
using TCCLume.Services;

namespace TCCLume.Views;

public partial class TeladeCadastro : ContentPage
{
    private readonly DatabaseService _database;

    public TeladeCadastro(DatabaseService database)
    {
        InitializeComponent();

        _database = database;
    }

    // =========================================================
    // SENHA - INDICADOR DE FORÇA
    // =========================================================

    private void PasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string password = PasswordEntry.Text ?? "";

        // Se o campo estiver vazio, esconder os indicadores
        if (string.IsNullOrWhiteSpace(password))
        {
            StrengthBarsContainer.IsVisible = false;
            PasswordStrengthLabel.IsVisible = false;
            RequirementsContainer.IsVisible = false;

            ResetBars();

            CheckPasswordMatch();

            return;
        }

        // Mostrar os indicadores
        StrengthBarsContainer.IsVisible = true;
        PasswordStrengthLabel.IsVisible = true;
        RequirementsContainer.IsVisible = true;

        // Verificar os requisitos
        bool hasLength = password.Length >= 8;
        bool hasNumber = Regex.IsMatch(password, @"\d");
        bool hasUpper = Regex.IsMatch(password, @"[A-Z]");
        bool hasSpecial = Regex.IsMatch(password, @"[\W_]");

        int score = 0;

        if (hasLength)
            score++;

        if (hasNumber)
            score++;

        if (hasUpper)
            score++;

        if (hasSpecial)
            score++;

        // Atualizar os requisitos
        UpdateRequirement(
            LengthRequirement,
            hasLength,
            "Mínimo de 8 caracteres");

        UpdateRequirement(
            NumberRequirement,
            hasNumber,
            "Ter número");

        UpdateRequirement(
            UpperRequirement,
            hasUpper,
            "Letra maiúscula");

        UpdateRequirement(
            SpecialRequirement,
            hasSpecial,
            "Caractere especial");

        // Limpar as barras antes de preencher novamente
        ResetBars();

        Color barColor;

        // SENHA FRACA
        if (score <= 1)
        {
            barColor = Colors.Red;

            PasswordStrengthLabel.Text = "Senha fraca";
            PasswordStrengthLabel.TextColor = Colors.Red;
        }

        // SENHA MÉDIA
        else if (score <= 3)
        {
            barColor = Colors.Gold;

            PasswordStrengthLabel.Text = "Senha média";
            PasswordStrengthLabel.TextColor = Colors.Gold;
        }

        // SENHA FORTE
        else
        {
            barColor = Colors.LimeGreen;

            PasswordStrengthLabel.Text = "Senha forte";
            PasswordStrengthLabel.TextColor = Colors.LimeGreen;
        }

        // Preencher as barras conforme a pontuação
        if (score >= 1)
            Bar1.Color = barColor;

        if (score >= 2)
            Bar2.Color = barColor;

        if (score >= 3)
            Bar3.Color = barColor;

        if (score >= 4)
            Bar4.Color = barColor;

        // Verificar confirmação da senha
        CheckPasswordMatch();
    }


    // =========================================================
    // CONFIRMAR SENHA
    // =========================================================

    private void ConfirmPasswordEntry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        CheckPasswordMatch();
    }


    // =========================================================
    // VERIFICAR SE AS SENHAS COINCIDEM
    // =========================================================

    private void CheckPasswordMatch()
    {
        if (string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            PasswordMatchLabel.Text = "";
            return;
        }

        if (PasswordEntry.Text == ConfirmPasswordEntry.Text)
        {
            PasswordMatchLabel.Text =
                "✔ As senhas coincidem";

            PasswordMatchLabel.TextColor =
                Colors.LimeGreen;
        }
        else
        {
            PasswordMatchLabel.Text =
                "❌ As senhas não coincidem";

            PasswordMatchLabel.TextColor =
                Colors.Red;
        }
    }


    // =========================================================
    // MOSTRAR / ESCONDER SENHA
    // =========================================================

    private void PasswordVisibilityButton_Clicked(
    object sender,
    EventArgs e)
    {
        PasswordEntry.IsPassword =
            !PasswordEntry.IsPassword;

        PasswordVisibilityButton.Source =
            PasswordEntry.IsPassword
                ? "eye.png"
                : "offeye.png";
    }


    // =========================================================
    // MOSTRAR / ESCONDER CONFIRMAÇÃO DA SENHA
    // =========================================================

    private void ConfirmPasswordVisibilityButton_Clicked(
    object sender,
    EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword =
            !ConfirmPasswordEntry.IsPassword;

        ConfirmPasswordVisibilityButton.Source =
            ConfirmPasswordEntry.IsPassword
                ? "eye.png"
                : "offeye.png";
    }


    // =========================================================
    // ATUALIZAR REQUISITOS
    // =========================================================

    private void UpdateRequirement(
        Label label,
        bool valid,
        string text)
    {
        if (valid)
        {
            label.Text = $"✔ {text}";
            label.TextColor = Colors.LimeGreen;
        }
        else
        {
            label.Text = $"❌ {text}";
            label.TextColor = Colors.Gray;
        }
    }


    // =========================================================
    // RESETAR BARRAS
    // =========================================================

    private void ResetBars()
    {
        Bar1.Color =
            Color.FromArgb("#2A2A2A");

        Bar2.Color =
            Color.FromArgb("#2A2A2A");

        Bar3.Color =
            Color.FromArgb("#2A2A2A");

        Bar4.Color =
            Color.FromArgb("#2A2A2A");
    }


    // =========================================================
    // CADASTRAR
    // =========================================================

    private async void OnRegisterClicked(
        object sender,
        EventArgs e)
    {
        // Verificar se as senhas coincidem
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert(
                "Erro",
                "As senhas não coincidem",
                "OK");

            return;
        }

        // Verificar se o e-mail já existe
        var existingUser =
            await _database.GetUserByEmail(
                EmailEntry.Text);

        if (existingUser != null)
        {
            await DisplayAlert(
                "Erro",
                "Este e-mail já existe",
                "OK");

            return;
        }

        // Criar usuário
        User user = new User
        {
            Name = NameEntry.Text,
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };

        // Salvar no banco
        await _database.RegisterUser(user);

        await DisplayAlert(
            "Sucesso",
            "Conta criada!",
            "OK");

        // Voltar para o Login
        await Navigation.PopAsync();
    }


    // =========================================================
    // VOLTAR
    // =========================================================

    private async void OnBackClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}