using SQLite;

namespace TCCLume.Models;

public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public double Valor { get; set; }

    // Receita ou Despesa
    public string Tipo { get; set; } = string.Empty;

    // Exemplo: Salário, Alimentação, Transporte...
    public string Categoria { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;

    // Usuário responsável pela movimentação
    public int UserId { get; set; }
}