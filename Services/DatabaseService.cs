using SQLite;
using TCCLume.Models;

namespace TCCLume.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "tcclume.db3");

        System.Diagnostics.Debug.WriteLine(
            $"BANCO SQLITE: {dbPath}");

        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<User>().Wait();
        _database.CreateTableAsync<Transaction>().Wait();
        _database.CreateTableAsync<FinancialGoal>().Wait();
    }

    // =========================================
    // USUÁRIOS
    // =========================================

    // CADASTRAR USUÁRIO
    public async Task<int> RegisterUser(User user)
    {
        return await _database.InsertAsync(user);
    }

    // LOGIN
    public async Task<User?> Login(
        string email,
        string password)
    {
        return await _database
            .Table<User>()
            .Where(x =>
                x.Email == email &&
                x.Password == password)
            .FirstOrDefaultAsync();
    }

    // VERIFICAR EMAIL
    public async Task<User?> GetUserByEmail(
        string email)
    {
        return await _database
            .Table<User>()
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
    }

    //ATUALIZAR NOME E SENHA
    public async Task<int> UpdateUser(User user)
    {
        return await _database.UpdateAsync(user);
    }


    // =========================================
    // TRANSAÇÕES
    // =========================================

    // ADICIONAR
    public async Task<int> AddTransaction(
        Transaction transaction)
    {
        return await _database.InsertAsync(transaction);
    }

    // BUSCAR TODAS AS TRANSAÇÕES DO USUÁRIO
    public async Task<List<Transaction>> GetTransactions(
        int userId)
    {
        return await _database
            .Table<Transaction>()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    // ATUALIZAR
    public async Task<int> UpdateTransactionAsync(
        Transaction transaction)
    {
        return await _database.UpdateAsync(transaction);
    }

    // EXCLUIR
    public async Task<int> DeleteTransactionAsync(
        Transaction transaction)
    {
        return await _database.DeleteAsync(transaction);
    }

    // =========================================
    // METAS FINANCEIRAS
    // =========================================

    public async Task<int> AddFinancialGoal(
        FinancialGoal goal)
    {
        return await _database.InsertAsync(goal);
    }


    public async Task<List<FinancialGoal>> GetFinancialGoals(
        int userId)
    {
        return await _database
            .Table<FinancialGoal>()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }


    public async Task<int> UpdateFinancialGoal(
        FinancialGoal goal)
    {
        return await _database.UpdateAsync(goal);
    }


    public async Task<int> DeleteFinancialGoal(
        FinancialGoal goal)
    {
        return await _database.DeleteAsync(goal);
    }

    // =========================================
    // CONSULTAS PARA O DASHBOARD
    // =========================================

    // TOTAL DE RECEITAS
    public async Task<double> GetTotalIncome(
        int userId)
    {
        var transactions = await _database
            .Table<Transaction>()
            .Where(x =>
                x.UserId == userId &&
                x.Tipo == "Receita")
            .ToListAsync();

        return transactions.Sum(x => x.Valor);
    }

    // TOTAL DE DESPESAS
    public async Task<double> GetTotalExpense(
        int userId)
    {
        var transactions = await _database
            .Table<Transaction>()
            .Where(x =>
                x.UserId == userId &&
                x.Tipo == "Despesa")
            .ToListAsync();

        return transactions.Sum(x => x.Valor);
    }
}