namespace TCCLume.Models;

public static class FinancialCategories
{
    // =========================================
    // RECEITAS
    // =========================================

    public static List<string> Receitas { get; } = new()
    {
        "Salário",
        "Freelance",
        "Investimentos",
        "Presente",
        "Outras receitas"
    };


    // =========================================
    // DESPESAS
    // =========================================

    public static List<string> Despesas { get; } = new()
    {
        "Alimentação",
        "Moradia",
        "Transporte",
        "Saúde",
        "Educação",
        "Lazer",
        "Compras",
        "Contas",
        "Assinaturas",
        "Outras despesas"
    };
}