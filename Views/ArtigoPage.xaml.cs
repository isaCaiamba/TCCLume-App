using TCCLume.Models;

namespace TCCLume.Views;

public partial class ArtigoPage : ContentPage
{
    public ArtigoPage(
        User user,
        string titulo,
        string categoria,
        string conteudo)
    {
        InitializeComponent();

        CategoriaLabel.Text = categoria.ToUpper();
        TituloLabel.Text = titulo;
        ConteudoLabel.Text = conteudo;
    }

    private async void Voltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}