using System.Globalization;
using TCCLume.Models;
using TCCLume.Services;

namespace TCCLume.Views;

public partial class MinhasEconomiasPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly User _user;

    private List<FinancialGoal> goals = new();

    private readonly CultureInfo _culture =
        CultureInfo.GetCultureInfo("pt-BR");


    // =========================================
    // CONSTRUTOR
    // =========================================

    public MinhasEconomiasPage(User user)
    {
        InitializeComponent();

        _database = new DatabaseService();
        _user = user;
    }


    // =========================================
    // AO ABRIR A PÁGINA
    // =========================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadGoals();
    }


    // =========================================
    // CARREGAR METAS
    // =========================================

    private async Task LoadGoals()
    {
        goals =
            await _database.GetFinancialGoals(
                _user.Id);

        UpdateSummary();

        GoalsLayout.Children.Clear();


        // =====================================
        // NENHUMA META
        // =====================================

        if (goals.Count == 0)
        {
            GoalsLayout.Children.Add(
                new Label
                {
                    Text =
                        "Você ainda não possui nenhuma meta financeira.",
                    TextColor =
                        Color.FromArgb("#A8AFB5"),
                    FontSize = 14,
                    HorizontalOptions =
                        LayoutOptions.Center,
                    Margin =
                        new Thickness(0, 10)
                });

            return;
        }


        // =====================================
        // CRIAR CARDS
        // =====================================

        foreach (var goal in goals)
        {
            CreateGoalCard(goal);
        }
    }


    // =========================================
    // ATUALIZAR RESUMO
    // =========================================

    private void UpdateSummary()
    {
        double totalGuardado =
            goals.Sum(x => x.ValorGuardado);

        TotalSavedLabel.Text =
            totalGuardado.ToString(
                "C",
                _culture);


        int quantidade =
            goals.Count;

        GoalsCountLabel.Text =
            quantidade == 1
                ? "1 meta ativa"
                : $"{quantidade} metas ativas";
    }


    // =========================================
    // CRIAR CARD DA META
    // =========================================

    private void CreateGoalCard(
        FinancialGoal goal)
    {
        double percentual = 0;

        if (goal.ValorMeta > 0)
        {
            percentual =
                goal.ValorGuardado /
                goal.ValorMeta;
        }


        // Impede que a barra ultrapasse 100%
        percentual =
            Math.Clamp(
                percentual,
                0,
                1);


        double falta =
            Math.Max(
                goal.ValorMeta -
                goal.ValorGuardado,
                0);


        // =====================================
        // TÍTULO
        // =====================================

        var titleLabel =
            new Label
            {
                Text =
                    $"🎯 {goal.Nome}",
                FontSize = 17,
                FontAttributes =
                    FontAttributes.Bold,
                TextColor =
                    Colors.White
            };


        // =====================================
        // VALORES
        // =====================================

        var valueLabel =
            new Label
            {
                Text =
                    $"{goal.ValorGuardado.ToString("C", _culture)} de {goal.ValorMeta.ToString("C", _culture)}",
                FontSize = 12,
                TextColor =
                    Color.FromArgb("#A8AFB5")
            };


        // =====================================
        // BARRA
        // =====================================

        var progressBar =
            new ProgressBar
            {
                Progress =
                    percentual,
                ProgressColor =
                    Color.FromArgb("#F2C94C"),
                BackgroundColor =
                    Color.FromArgb("#2A2D3E"),
                HeightRequest = 10
            };


        // =====================================
        // PORCENTAGEM
        // =====================================

        var percentageLabel =
            new Label
            {
                Text =
                    $"{percentual * 100:F0}%",
                FontSize = 12,
                FontAttributes =
                    FontAttributes.Bold,
                TextColor =
                    Color.FromArgb("#F2C94C")
            };


        // =====================================
        // VALOR RESTANTE
        // =====================================

        var remainingLabel =
            new Label
            {
                Text =
                    falta > 0
                        ? $"Faltam {falta.ToString("C", _culture)}"
                        : "🎉 Meta alcançada!",
                FontSize = 12,
                TextColor =
                    Color.FromArgb("#A8AFB5"),
                HorizontalOptions =
                    LayoutOptions.End
            };


        var percentageGrid =
            new Grid
            {
                ColumnDefinitions =
                    new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(
                            GridLength.Star),

                        new ColumnDefinition(
                            GridLength.Auto)
                    }
            };


        percentageGrid.Add(
            percentageLabel,
            0,
            0);

        percentageGrid.Add(
            remainingLabel,
            1,
            0);


        // =====================================
        // BOTÕES
        // =====================================

        var addButton =
            new Button
            {
                Text =
                    "+ Adicionar",
                FontSize = 12,
                HeightRequest = 38,
                BackgroundColor =
                    Color.FromArgb("#1A1C2B"),
                TextColor =
                    Color.FromArgb("#F2C94C"),
                CornerRadius = 8
            };


        addButton.Clicked +=
            async (sender, e) =>
            {
                await AddSaving(goal);
            };


        var editButton =
            new Button
            {
                Text =
                    "Editar",
                FontSize = 12,
                HeightRequest = 38,
                BackgroundColor =
                    Color.FromArgb("#2A2D3E"),
                TextColor =
                    Colors.White,
                CornerRadius = 8
            };


        editButton.Clicked +=
            async (sender, e) =>
            {
                await EditGoal(goal);
            };


        var deleteButton =
            new Button
            {
                Text =
                    "Excluir",
                FontSize = 12,
                HeightRequest = 38,
                BackgroundColor =
                    Color.FromArgb("#2A2D3E"),
                TextColor =
                    Color.FromArgb("#FF5B5B"),
                CornerRadius = 8
            };


        deleteButton.Clicked +=
            async (sender, e) =>
            {
                await DeleteGoal(goal);
            };


        var buttonsGrid =
            new Grid
            {
                ColumnDefinitions =
                    new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(
                            GridLength.Star),

                        new ColumnDefinition(
                            GridLength.Star),

                        new ColumnDefinition(
                            GridLength.Star)
                    },

                ColumnSpacing = 8
            };


        buttonsGrid.Add(
            addButton,
            0,
            0);

        buttonsGrid.Add(
            editButton,
            1,
            0);

        buttonsGrid.Add(
            deleteButton,
            2,
            0);


        // =====================================
        // CONTEÚDO DO CARD
        // =====================================

        var layout =
            new VerticalStackLayout
            {
                Spacing = 10
            };


        layout.Children.Add(
            titleLabel);

        layout.Children.Add(
            valueLabel);

        layout.Children.Add(
            progressBar);

        layout.Children.Add(
            percentageGrid);

        layout.Children.Add(
            buttonsGrid);


        // =====================================
        // CARD
        // =====================================

        var frame =
            new Frame
            {
                BackgroundColor =
                    Color.FromArgb("#121424"),
                BorderColor =
                    Color.FromArgb("#2A2D3E"),
                CornerRadius = 18,
                Padding = 18,
                Content = layout
            };


        GoalsLayout.Children.Add(
            frame);
    }


    // =========================================
    // NOVA META
    // =========================================

    private async void NewGoalButton_Clicked(
        object sender,
        EventArgs e)
    {
        string? nome =
            await DisplayPromptAsync(
                "Nova meta",
                "Digite o nome da meta:");

        if (string.IsNullOrWhiteSpace(nome))
            return;


        string? valorMetaTexto =
            await DisplayPromptAsync(
                "Nova meta",
                "Digite o valor que deseja alcançar:",
                initialValue: "1000");


        if (string.IsNullOrWhiteSpace(
            valorMetaTexto))
        {
            return;
        }


        if (!double.TryParse(
            valorMetaTexto,
            NumberStyles.Currency,
            _culture,
            out double valorMeta))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }


        if (valorMeta <= 0)
        {
            await DisplayAlert(
                "Erro",
                "O valor da meta deve ser maior que zero.",
                "OK");

            return;
        }


        string? guardadoTexto =
            await DisplayPromptAsync(
                "Nova meta",
                "Quanto você já possui guardado?",
                initialValue: "0");


        if (string.IsNullOrWhiteSpace(
            guardadoTexto))
        {
            return;
        }


        if (!double.TryParse(
            guardadoTexto,
            NumberStyles.Currency,
            _culture,
            out double valorGuardado))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }


        if (valorGuardado < 0)
        {
            await DisplayAlert(
                "Erro",
                "O valor guardado não pode ser negativo.",
                "OK");

            return;
        }


        if (valorGuardado > valorMeta)
        {
            valorGuardado =
                valorMeta;
        }


        var goal =
            new FinancialGoal
            {
                Nome =
                    nome.Trim(),

                ValorMeta =
                    valorMeta,

                ValorGuardado =
                    valorGuardado,

                UserId =
                    _user.Id
            };


        await _database.AddFinancialGoal(
            goal);


        await LoadGoals();


        await DisplayAlert(
            "Sucesso ✨",
            "Sua meta foi criada com sucesso!",
            "OK");
    }


    // =========================================
    // ADICIONAR ECONOMIA
    // =========================================

    private async Task AddSaving(
        FinancialGoal goal)
    {
        string? valorTexto =
            await DisplayPromptAsync(
                "Adicionar economia",
                $"Quanto deseja adicionar à meta \"{goal.Nome}\"?",
                initialValue: "100");


        if (string.IsNullOrWhiteSpace(
            valorTexto))
        {
            return;
        }


        if (!double.TryParse(
            valorTexto,
            NumberStyles.Currency,
            _culture,
            out double valor))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }


        if (valor <= 0)
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor maior que zero.",
                "OK");

            return;
        }


        goal.ValorGuardado += valor;


        if (goal.ValorGuardado >
            goal.ValorMeta)
        {
            goal.ValorGuardado =
                goal.ValorMeta;
        }


        await _database.UpdateFinancialGoal(
            goal);


        await LoadGoals();
    }


    // =========================================
    // EDITAR META
    // =========================================

    private async Task EditGoal(
        FinancialGoal goal)
    {
        string? novoNome =
            await DisplayPromptAsync(
                "Editar meta",
                "Nome da meta:",
                initialValue:
                    goal.Nome);


        if (string.IsNullOrWhiteSpace(
            novoNome))
        {
            return;
        }


        string? novoValorTexto =
            await DisplayPromptAsync(
                "Editar meta",
                "Valor da meta:",
                initialValue:
                    goal.ValorMeta.ToString(
                        "F2",
                        _culture));


        if (string.IsNullOrWhiteSpace(
            novoValorTexto))
        {
            return;
        }


        if (!double.TryParse(
            novoValorTexto,
            NumberStyles.Currency,
            _culture,
            out double novoValor))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }


        if (novoValor <= 0)
        {
            await DisplayAlert(
                "Erro",
                "O valor da meta deve ser maior que zero.",
                "OK");

            return;
        }


        goal.Nome =
            novoNome.Trim();

        goal.ValorMeta =
            novoValor;


        if (goal.ValorGuardado >
            goal.ValorMeta)
        {
            goal.ValorGuardado =
                goal.ValorMeta;
        }


        await _database.UpdateFinancialGoal(
            goal);


        await LoadGoals();
    }


    // =========================================
    // EXCLUIR META
    // =========================================

    private async Task DeleteGoal(
        FinancialGoal goal)
    {
        bool confirmar =
            await DisplayAlert(
                "Excluir meta",
                $"Deseja realmente excluir a meta \"{goal.Nome}\"?",
                "Excluir",
                "Cancelar");


        if (!confirmar)
            return;


        await _database.DeleteFinancialGoal(
            goal);


        await LoadGoals();
    }


    // =========================================
    // MENU
    // =========================================

    private async void MenuButton_Clicked(
        object sender,
        EventArgs e)
    {
        string action =
            await DisplayActionSheet(
                "Menu",
                "Cancelar",
                null,
                "Lume",
                "Dashboard",
                "Resumo Mensal",
                "Blog",
                "Configurações");

        if (action == "Lume")
        {
            await Navigation.PushAsync(
                new ChatPage(_user));

            return;
        }

        if (action == "Dashboard")
        {
            await Navigation.PushAsync(
                new DashboardPage(_user));

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

        if (action == "Configurações")
        {
            await Navigation.PushAsync(
                new ConfiguracoesPage(_user));

            return;
        }
    }
}