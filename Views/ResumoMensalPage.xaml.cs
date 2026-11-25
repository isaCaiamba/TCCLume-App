using TCCLume.Models;
using TCCLume.Services;

namespace TCCLume.Views;

public partial class ResumoMensalPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly User _user;

    // Lista das transações carregadas do banco
    private List<Transaction> transactions = new();

    public ResumoMensalPage(User user)
    {
        InitializeComponent();

        _database = new DatabaseService();
        _user = user;

        IncomeCategoryPicker.ItemsSource =
        FinancialCategories.Receitas;

        ExpenseCategoryPicker.ItemsSource =
            FinancialCategories.Despesas;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadTransactions();
    }

    // =========================================
    // CARREGAR TRANSAÇÕES
    // =========================================

    private async Task LoadTransactions()
    {
        // Busca as transações no banco
        transactions =
            await _database.GetTransactions(_user.Id);

        // Mostra as transações na tela
        TransactionsCollection.ItemsSource =
            transactions;
    }


    // =========================================
    // ADICIONAR RECEITA
    // =========================================

    private async void AddIncome_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            IncomeTitleEntry.Text) ||
            string.IsNullOrWhiteSpace(
            IncomeValueEntry.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Preencha todos os campos.",
                "OK");

            return;
        }

        if (!double.TryParse(
            IncomeValueEntry.Text,
            out double valor))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }

        if (IncomeCategoryPicker.SelectedItem == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione uma categoria.",
                "OK");

            return;
        }

        Transaction transaction = new()
        {
            Titulo = IncomeTitleEntry.Text,
            Valor = valor,
            Tipo = "Receita",
            Categoria = IncomeCategoryPicker.SelectedItem.ToString()!,
            Data = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            UserId = _user.Id
        };

        // Salva no banco
        await _database.AddTransaction(transaction);

        // Limpa a categoria selecionada
        IncomeCategoryPicker.SelectedItem = null;

        // Limpa os campos
        IncomeTitleEntry.Text = "";
        IncomeValueEntry.Text = "";

        // Atualiza a lista
        await LoadTransactions();
    }


    // =========================================
    // ADICIONAR DESPESA
    // =========================================

    private async void AddExpense_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            ExpenseTitleEntry.Text) ||
            string.IsNullOrWhiteSpace(
            ExpenseValueEntry.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Preencha todos os campos.",
                "OK");

            return;
        }

        if (!double.TryParse(
            ExpenseValueEntry.Text,
            out double valor))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }

        if (ExpenseCategoryPicker.SelectedItem == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione uma categoria.",
                "OK");

            return;
        }

        Transaction transaction = new()
        {
            Titulo = ExpenseTitleEntry.Text,
            Valor = valor,
            Tipo = "Despesa",
            Categoria = ExpenseCategoryPicker.SelectedItem.ToString()!,
            Data = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            UserId = _user.Id
        };

        // Salva no banco
        await _database.AddTransaction(transaction);

        // Limpa a categoria selecionada
        ExpenseCategoryPicker.SelectedItem = null;

        // Limpa os campos
        ExpenseTitleEntry.Text = "";
        ExpenseValueEntry.Text = "";

        // Atualiza a lista
        await LoadTransactions();
    }


    // =========================================
    // EDITAR TRANSAÇÃO
    // =========================================

    private async void EditTransaction_Clicked(
    object sender,
    EventArgs e)
    {
        Button button = (Button)sender;

        // Pega o ID enviado pelo botão
        int transactionId =
            Convert.ToInt32(
                button.CommandParameter);

        // Procura a transação na lista
        Transaction? transaction =
            transactions.FirstOrDefault(
                t => t.Id == transactionId);

        if (transaction == null)
            return;


        // =========================================
        // EDITAR TÍTULO
        // =========================================

        string? novoTitulo =
            await DisplayPromptAsync(
                "Editar movimentação",
                "Digite o novo título:",
                initialValue:
                    transaction.Titulo);

        if (string.IsNullOrWhiteSpace(novoTitulo))
            return;


        // =========================================
        // EDITAR VALOR
        // =========================================

        string? novoValor =
            await DisplayPromptAsync(
                "Editar movimentação",
                "Digite o novo valor:",
                initialValue:
                    transaction.Valor.ToString("F2"));

        if (string.IsNullOrWhiteSpace(novoValor))
            return;

        if (!double.TryParse(
            novoValor,
            out double valor))
        {
            await DisplayAlert(
                "Erro",
                "Digite um valor válido.",
                "OK");

            return;
        }


        // =========================================
        // EDITAR CATEGORIA
        // =========================================

        List<string> categorias;

        if (transaction.Tipo == "Receita")
        {
            categorias = FinancialCategories.Receitas;
        }
        else
        {
            categorias = FinancialCategories.Despesas;
        }

        string? novaCategoria =
            await DisplayActionSheet(
                "Escolha uma categoria",
                "Cancelar",
                null,
                categorias.ToArray());

        // Se apertar Cancelar
        if (novaCategoria == "Cancelar" ||
            string.IsNullOrWhiteSpace(novaCategoria))
        {
            return;
        }


        // =========================================
        // ATUALIZA OS DADOS
        // =========================================

        transaction.Titulo = novoTitulo;
        transaction.Valor = valor;
        transaction.Categoria = novaCategoria;


        // =========================================
        // SALVA NO BANCO
        // =========================================

        await _database.UpdateTransactionAsync(
            transaction);


        // =========================================
        // ATUALIZA A TELA
        // =========================================

        await LoadTransactions();

        await DisplayAlert(
            "Sucesso",
            "Movimentação atualizada.",
            "OK");
    }


    // =========================================
    // EXCLUIR TRANSAÇÃO
    // =========================================

    private async void DeleteTransaction_Clicked(
        object sender,
        EventArgs e)
    {
        Button button = (Button)sender;

        // Pega o ID enviado pelo botão
        int transactionId =
            Convert.ToInt32(
                button.CommandParameter);

        // Procura a transação
        Transaction? transaction =
            transactions.FirstOrDefault(
                t => t.Id == transactionId);

        if (transaction == null)
            return;

        // Confirma a exclusão
        bool confirmar =
            await DisplayAlert(
                "Excluir movimentação",
                $"Deseja excluir \"{transaction.Titulo}\"?",
                "Excluir",
                "Cancelar");

        if (!confirmar)
            return;

        // EXCLUI DO BANCO
        await _database.DeleteTransactionAsync(
            transaction);

        // Atualiza a tela
        await LoadTransactions();

        await DisplayAlert(
            "Sucesso",
            "Movimentação excluída.",
            "OK");
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
                "Minhas Economias",
                "Blog",
                "Configurações");

        if (action == "Lume")
        {
            await Navigation.PushAsync(
                new ChatPage(_user));
        }

        if (action == "Dashboard")
        {
            await Navigation.PopAsync();
        }

        if (action == "Minhas Economias")
        {
            await Navigation.PushAsync(
                new MinhasEconomiasPage(_user));
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