using TCCLume.Models;
using TCCLume.Services;
using System.Globalization;

namespace TCCLume.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly User _user;

    private List<Transaction> transactions = new();

    private readonly CultureInfo _culture =
        CultureInfo.GetCultureInfo("pt-BR");


    // =========================================
    // CONSTRUTOR
    // =========================================

    public DashboardPage(User user)
    {
        InitializeComponent();

        _database = new DatabaseService();
        _user = user;

        WelcomeLabel.Text =
            $"Olá, {_user.Name}! 👋";


        // =========================================
        // FILTRO INICIAL
        // =========================================

        // Primeiro dia do mês atual
        StartDatePicker.Date =
            new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                1);

        // Até o dia atual
        EndDatePicker.Date =
            DateTime.Now.Date;


        MonthLabel.Text =
            $"Período: {StartDatePicker.Date:dd/MM/yyyy} até {EndDatePicker.Date:dd/MM/yyyy}";
    }


    // =========================================
    // AO ABRIR A TELA
    // =========================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadDashboard();
    }


    // =========================================
    // CARREGAR DASHBOARD
    // =========================================

    private async Task LoadDashboard()
    {
        transactions =
            await _database.GetTransactions(_user.Id);


        DateTime dataInicial =
            StartDatePicker.Date.Date;

        DateTime dataFinal =
            EndDatePicker.Date.Date;


        // =========================================
        // VALIDAR DATAS
        // =========================================

        if (dataInicial > dataFinal)
        {
            await DisplayAlert(
                "Atenção",
                "A data inicial não pode ser maior que a data final.",
                "OK");

            return;
        }


        // =========================================
        // SALDO TOTAL
        // =========================================

        // Aqui NÃO usamos o filtro.
        // O saldo total considera todas as movimentações
        // cadastradas pelo usuário.

        double receitasTotais =
            transactions
                .Where(x => x.Tipo == "Receita")
                .Sum(x => x.Valor);


        double despesasTotais =
            transactions
                .Where(x => x.Tipo == "Despesa")
                .Sum(x => x.Valor);


        double saldoTotal =
            receitasTotais - despesasTotais;


        // Atualizar SALDO TOTAL

        TotalBalanceLabel.Text =
            saldoTotal.ToString(
                "C",
                _culture);


        // =========================================
        // FILTRAR TRANSAÇÕES DO PERÍODO
        // =========================================

        var transacoesDoPeriodo =
            transactions
                .Where(x =>
                    DataEstaNoPeriodo(
                        x.Data,
                        dataInicial,
                        dataFinal))
                .ToList();


        // =========================================
        // RECEITAS DO PERÍODO
        // =========================================

        double receitasPeriodo =
            transacoesDoPeriodo
                .Where(x => x.Tipo == "Receita")
                .Sum(x => x.Valor);


        // =========================================
        // DESPESAS DO PERÍODO
        // =========================================

        double despesasPeriodo =
            transacoesDoPeriodo
                .Where(x => x.Tipo == "Despesa")
                .Sum(x => x.Valor);


        // =========================================
        // SALDO DO PERÍODO
        // =========================================

        double saldoPeriodo =
            receitasPeriodo - despesasPeriodo;


        // =========================================
        // ATUALIZAR SALDO DO PERÍODO
        // =========================================

        BalanceLabel.Text =
            saldoPeriodo.ToString(
                "C",
                _culture);


        // =========================================
        // RECEITAS
        // =========================================

        IncomeLabel.Text =
            receitasPeriodo.ToString(
                "C",
                _culture);


        // =========================================
        // DESPESAS
        // =========================================

        ExpenseLabel.Text =
            despesasPeriodo.ToString(
                "C",
                _culture);

        // =========================================
        // ECONOMIA
        // =========================================

        SavingsLabel.Text =
            saldoPeriodo.ToString(
                "C",
                _culture);


        // =========================================
        // PERÍODO EXIBIDO
        // =========================================

        MonthLabel.Text =
            $"Período: {dataInicial:dd/MM/yyyy} até {dataFinal:dd/MM/yyyy}";


        // =========================================
        // QUANTIDADE DE LANÇAMENTOS
        // =========================================

        TransactionsCountLabel.Text =
            transacoesDoPeriodo.Count.ToString();


        // =========================================
        // PERCENTUAL DA RENDA UTILIZADA
        // =========================================

        if (receitasPeriodo > 0)
        {
            double percentual =
                (despesasPeriodo / receitasPeriodo) * 100;

            PercentageLabel.Text =
                $"{percentual:F1}%";
        }
        else
        {
            PercentageLabel.Text =
                "0%";
        }


        // =========================================
        // MAIOR CATEGORIA DE GASTO
        // =========================================

        var maiorCategoria =
            transacoesDoPeriodo
                .Where(x => x.Tipo == "Despesa")
                .GroupBy(x => x.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    Total = g.Sum(x => x.Valor)
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();


        if (maiorCategoria != null)
        {
            BiggestExpenseLabel.Text =
                maiorCategoria.Categoria;
        }
        else
        {
            BiggestExpenseLabel.Text =
                "Nenhum";
        }


        // =========================================
        // STATUS DO SALDO
        // =========================================

        if (saldoPeriodo > 0)
        {
            BalanceStatusLabel.Text =
                "Neste período, você recebeu mais do que gastou. 👍";
        }
        else if (saldoPeriodo < 0)
        {
            BalanceStatusLabel.Text =
                "Atenção: suas despesas foram maiores que suas receitas neste período.";
        }
        else
        {
            BalanceStatusLabel.Text = "";
        }


        // =========================================
        // CATEGORIAS
        // =========================================

        LoadCategories(
            transacoesDoPeriodo);


        // =========================================
        // DICA DO LUME
        // =========================================

        LoadLumeInsight(
            transacoesDoPeriodo,
            receitasPeriodo,
            despesasPeriodo,
            saldoPeriodo,
            maiorCategoria?.Categoria);
    }


    // =========================================
    // VERIFICAR SE DATA ESTÁ NO PERÍODO
    // =========================================

    private bool DataEstaNoPeriodo(
        string data,
        DateTime dataInicial,
        DateTime dataFinal)
    {
        if (DateTime.TryParseExact(
            data,
            "dd/MM/yyyy HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime dataConvertida))
        {
            DateTime somenteData =
                dataConvertida.Date;

            return
                somenteData >= dataInicial.Date &&
                somenteData <= dataFinal.Date;
        }

        return false;
    }


    // =========================================
    // CATEGORIAS
    // =========================================

    private void LoadCategories(
        List<Transaction> transacoesDoPeriodo)
    {
        CategoriesLayout.Children.Clear();


        var categorias =
            transacoesDoPeriodo
                .Where(x => x.Tipo == "Despesa")
                .GroupBy(x => x.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    Total = g.Sum(x => x.Valor)
                })
                .OrderByDescending(x => x.Total)
                .ToList();


        if (categorias.Count == 0)
        {
            CategoriesLayout.Children.Add(
                new Label
                {
                    Text =
                        "Nenhuma despesa cadastrada neste período.",

                    TextColor =
                        Color.FromArgb("#A8AFB5"),

                    FontSize = 14
                });

            return;
        }


        double totalDespesas =
            categorias.Sum(x => x.Total);


        foreach (var categoria in categorias)
        {
            double percentual =
                totalDespesas > 0
                    ? categoria.Total / totalDespesas
                    : 0;


            var nomeLabel =
                new Label
                {
                    Text =
                        categoria.Categoria,

                    TextColor =
                        Colors.White,

                    FontSize = 14,

                    FontAttributes =
                        FontAttributes.Bold
                };


            var valorLabel =
                new Label
                {
                    Text =
                        categoria.Total.ToString(
                            "C",
                            _culture),

                    TextColor =
                        Color.FromArgb("#F2C94C"),

                    FontSize = 13
                };


            var linha =
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


            linha.Add(
                nomeLabel,
                0,
                0);


            linha.Add(
                valorLabel,
                1,
                0);


            var barra =
                new ProgressBar
                {
                    Progress =
                        percentual,

                    ProgressColor =
                        Color.FromArgb("#F2C94C"),

                    BackgroundColor =
                        Color.FromArgb("#2A2D3E"),

                    HeightRequest =
                        8
                };


            CategoriesLayout.Children.Add(
                linha);

            CategoriesLayout.Children.Add(
                barra);
        }
    }


    // =========================================
    // DICA DO LUME
    // =========================================

    private void LoadLumeInsight(
        List<Transaction> transacoesDoPeriodo,
        double receitas,
        double despesas,
        double resultado,
        string? maiorCategoria)
    {
        if (transacoesDoPeriodo.Count == 0)
        {
            LumeInsightLabel.Text =
                "Não encontrei movimentações neste período. Tente selecionar outro período ou cadastre novas receitas e despesas. ✨";

            return;
        }


        if (receitas == 0 && despesas > 0)
        {
            LumeInsightLabel.Text =
                "Você possui despesas registradas neste período, mas nenhuma receita. Verifique seus lançamentos e acompanhe seu saldo com atenção.";
        }
        else if (resultado < 0)
        {
            LumeInsightLabel.Text =
                "Suas despesas estão maiores que suas receitas neste período. Vale a pena revisar seus gastos e identificar onde é possível economizar.";
        }
        else if (!string.IsNullOrWhiteSpace(maiorCategoria))
        {
            LumeInsightLabel.Text =
                $"Sua maior categoria de gasto neste período é {maiorCategoria}. Acompanhar esse tipo de despesa pode ajudar você a economizar.";
        }
        else
        {
            LumeInsightLabel.Text =
                "Continue registrando suas movimentações. Assim posso ajudar você a entender melhor seus hábitos financeiros. ✨";
        }
    }


    // =========================================
    // FILTRO — APLICAR
    // =========================================

    private async void ApplyFilter_Clicked(
        object sender,
        EventArgs e)
    {
        if (StartDatePicker.Date >
            EndDatePicker.Date)
        {
            await DisplayAlert(
                "Atenção",
                "A data inicial não pode ser maior que a data final.",
                "OK");

            return;
        }


        await LoadDashboard();
    }


    // =========================================
    // FILTRO — HOJE
    // =========================================

    private async void TodayFilter_Clicked(
        object sender,
        EventArgs e)
    {
        DateTime hoje =
            DateTime.Now.Date;


        StartDatePicker.Date =
            hoje;

        EndDatePicker.Date =
            hoje;


        await LoadDashboard();
    }


    // =========================================
    // FILTRO — ÚLTIMOS 7 DIAS
    // =========================================

    private async void SevenDaysFilter_Clicked(
        object sender,
        EventArgs e)
    {
        DateTime hoje =
            DateTime.Now.Date;


        StartDatePicker.Date =
            hoje.AddDays(-6);

        EndDatePicker.Date =
            hoje;


        await LoadDashboard();
    }


    // =========================================
    // FILTRO — ESTE MÊS
    // =========================================

    private async void MonthFilter_Clicked(
        object sender,
        EventArgs e)
    {
        DateTime hoje =
            DateTime.Now.Date;


        StartDatePicker.Date =
            new DateTime(
                hoje.Year,
                hoje.Month,
                1);

        EndDatePicker.Date =
            hoje;


        await LoadDashboard();
    }


    // =========================================
    // LIMPAR FILTRO
    // =========================================

    private async void ClearFilter_Clicked(
        object sender,
        EventArgs e)
    {
        StartDatePicker.Date =
            new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                1);


        EndDatePicker.Date =
            DateTime.Now.Date;


        await LoadDashboard();
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
                "Minhas Economias",
                "Resumo Mensal",
                "Blog",
                "Configurações");


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