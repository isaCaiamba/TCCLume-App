using SQLite;

namespace TCCLume.Models;

public class FinancialGoal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Nome da meta
    public string Nome { get; set; } = string.Empty;

    // Valor total que deseja alcançar
    public double ValorMeta { get; set; }

    // Valor que já foi guardado
    public double ValorGuardado { get; set; }

    // Usuário responsável pela meta
    public int UserId { get; set; }
}