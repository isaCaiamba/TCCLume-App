using Microsoft.Maui.Controls.Shapes;
using TCCLume.Models;

namespace TCCLume.Views;

public partial class BlogPage : ContentPage
{
    private readonly User _user;
    private string _categoriaAtual = "Todos";
    private HorizontalStackLayout? _categoriasContainer;

    private class Artigo
    {
        public string Titulo { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string Resumo { get; set; } = "";
        public string Conteudo { get; set; } = "";
    }

    private readonly List<Artigo> _artigos = new()
    {
        new Artigo
        {
            Titulo = "Como organizar seu dinheiro em 5 passos",
            Categoria = "Organização",
            Resumo = "Aprenda a planejar suas receitas e despesas.",
            Conteudo =
                "1. Anote todo o dinheiro que você recebe.\n\n" +
                "2. Registre seus gastos, inclusive os pequenos.\n\n" +
                "3. Separe as despesas essenciais das não essenciais.\n\n" +
                "4. Defina limites de gastos de acordo com sua renda.\n\n" +
                "5. Revise seu planejamento todos os meses.\n\n" +
                "O segredo é acompanhar sua situação financeira com frequência " +
                "e ajustar o planejamento sempre que necessário."
        },

        new Artigo
        {
            Titulo = "Pequenos gastos que pesam no orçamento",
            Categoria = "Organização",
            Resumo = "Veja como despesas pequenas podem se acumular.",
            Conteudo =
                "Um café, um lanche ou uma compra por impulso pode parecer " +
                "inofensivo quando observado isoladamente. Porém, quando " +
                "esses gastos se repetem, podem comprometer uma parte " +
                "importante do orçamento.\n\n" +
                "Experimente registrar todas as despesas durante um mês. " +
                "Depois, identifique quais gastos são necessários e quais " +
                "podem ser reduzidos sem prejudicar sua qualidade de vida."
        },

        new Artigo
        {
            Titulo = "Como começar a economizar",
            Categoria = "Economia",
            Resumo = "Pequenas atitudes para cuidar melhor da sua renda.",
            Conteudo =
                "Economizar não significa deixar de gastar com tudo aquilo " +
                "de que você gosta. Significa fazer escolhas de acordo com " +
                "suas prioridades.\n\n" +
                "Comece observando suas despesas, compare preços e evite " +
                "compras por impulso. Se possível, reserve uma quantia " +
                "compatível com sua realidade financeira.\n\n" +
                "O importante é criar um hábito sustentável, sem comprometer " +
                "as necessidades básicas."
        },

        new Artigo
        {
            Titulo = "Como criar uma meta financeira realista",
            Categoria = "Metas",
            Resumo = "Transforme seus objetivos em um plano possível.",
            Conteudo =
                "Primeiro, defina o que você deseja alcançar e quanto " +
                "precisará juntar. Depois, estabeleça um prazo que faça " +
                "sentido para sua situação financeira.\n\n" +
                "Divida o valor total em pequenas etapas e acompanhe " +
                "seu progresso. Se sua renda ou suas despesas mudarem, " +
                "reavalie o plano.\n\n" +
                "Uma meta realista deve considerar suas necessidades " +
                "e sua capacidade de guardar dinheiro."
        },

        new Artigo
        {
            Titulo = "Necessidade ou desejo?",
            Categoria = "Consumo consciente",
            Resumo = "Pense antes de comprar e priorize o que importa.",
            Conteudo =
                "Necessidades são itens importantes para o dia a dia, " +
                "como alimentação, moradia e saúde. Desejos são coisas " +
                "que podem trazer satisfação, mas nem sempre são urgentes.\n\n" +
                "Antes de comprar, pergunte a si mesmo: preciso disso agora? " +
                "A compra cabe no meu orçamento? Existe uma alternativa " +
                "mais econômica?\n\n" +
                "Fazer essas perguntas ajuda a tomar decisões mais conscientes."
        },

        new Artigo
        {
            Titulo = "Reserva de emergência: por onde começar?",
            Categoria = "Economia",
            Resumo = "Entenda a importância de se preparar para imprevistos.",
            Conteudo =
                "Uma reserva de emergência é um dinheiro separado para " +
                "situações inesperadas, como um conserto urgente ou uma " +
                "redução de renda.\n\n" +
                "Se ainda não consegue guardar muito, comece com uma " +
                "quantia pequena e regular. O valor ideal depende das " +
                "suas despesas e da estabilidade da sua renda.\n\n" +
                "Evite utilizar essa reserva para compras que podem ser " +
                "planejadas com antecedência."
        },

        new Artigo
        {
            Titulo = "O que fazer quando as contas não fecham?",
            Categoria = "Organização",
            Resumo = "Veja como analisar seu orçamento e definir prioridades.",
            Conteudo =
                "Quando as despesas ultrapassam as receitas, o primeiro " +
                "passo é entender para onde o dinheiro está indo.\n\n" +
                "Liste suas contas, identifique as despesas essenciais e " +
                "verifique onde é possível reduzir gastos. Priorize as " +
                "necessidades básicas e procure negociar dívidas quando " +
                "isso for adequado.\n\n" +
                "Evite assumir novos compromissos financeiros sem avaliar " +
                "se conseguirá pagá-los."
        },

        new Artigo
        {
            Titulo = "Planeje seus gastos antes de receber",
            Categoria = "Metas",
            Resumo = "Organize sua renda para evitar decisões de última hora.",
            Conteudo =
                "Antes de receber seu salário ou outra renda, faça uma " +
                "lista das contas previstas para o período.\n\n" +
                "Separe os valores para as despesas essenciais, considere " +
                "seus compromissos e avalie quanto poderá destinar às " +
                "suas metas.\n\n" +
                "Um planejamento simples ajuda a visualizar suas " +
                "possibilidades e a evitar gastos acima do que pode pagar."
        }
    };

    public BlogPage(User user)
    {
        InitializeComponent();

        _user = user;
        _categoriasContainer = CategoriasContainer;

        MostrarArtigos();
    }

    private void MostrarArtigos()
    {
        ArticlesContainer.Children.Clear();

        string busca = SearchEntry?.Text?.Trim() ?? "";

        var artigosFiltrados = _artigos.Where(artigo =>
            (_categoriaAtual == "Todos" ||
             artigo.Categoria == _categoriaAtual) &&
            (string.IsNullOrWhiteSpace(busca) ||
             artigo.Titulo.Contains(
                 busca,
                 StringComparison.OrdinalIgnoreCase) ||
             artigo.Resumo.Contains(
                 busca,
                 StringComparison.OrdinalIgnoreCase) ||
             artigo.Categoria.Contains(
                 busca,
                 StringComparison.OrdinalIgnoreCase))
        ).ToList();

        foreach (var artigo in artigosFiltrados)
        {
            var categoriaLabel = new Label
            {
                Text = artigo.Categoria.ToUpper(),
                FontSize = 10,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#F2C94C")
            };

            var tituloLabel = new Label
            {
                Text = artigo.Titulo,
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            };

            var resumoLabel = new Label
            {
                Text = artigo.Resumo,
                FontSize = 13,
                TextColor = Color.FromArgb("#B0B3C8")
            };

            var lerLabel = new Label
            {
                Text = "Ler artigo  →",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#F2C94C")
            };

            var conteudo = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    categoriaLabel,
                    tituloLabel,
                    resumoLabel,
                    lerLabel
                }
            };

            var card = new Border
            {
                BackgroundColor = Color.FromArgb("#121424"),
                Stroke = Color.FromArgb("#2A2D3E"),
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 16
                },
                Padding = 16,
                Content = conteudo
            };

            var toque = new TapGestureRecognizer();

            toque.Tapped += async (sender, e) =>
            {
                await Navigation.PushAsync(
                    new ArtigoPage(
                        _user,
                        artigo.Titulo,
                        artigo.Categoria,
                        artigo.Conteudo));
            };

            card.GestureRecognizers.Add(toque);

            ArticlesContainer.Children.Add(card);
        }

        EmptyLabel.IsVisible = artigosFiltrados.Count == 0;
    }

    private void SearchEntry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        MostrarArtigos();
    }

    private void Categoria_Clicked(object sender, EventArgs e)
    {
        if (sender is not Button botao)
            return;

        _categoriaAtual =
            botao.CommandParameter?.ToString() ?? "Todos";

        if (_categoriasContainer != null)
        {
            foreach (var item in _categoriasContainer.Children)
            {
                if (item is Button categoriaBotao)
                {
                    bool selecionado =
                        categoriaBotao.CommandParameter?.ToString()
                        == _categoriaAtual;

                    categoriaBotao.BackgroundColor = selecionado
                        ? Color.FromArgb("#F2C94C")
                        : Color.FromArgb("#121424");

                    categoriaBotao.TextColor = selecionado
                        ? Color.FromArgb("#070812")
                        : Colors.White;
                }
            }
        }

        MostrarArtigos();
    }

    private async void Destaque_Clicked(object sender, EventArgs e)
    {
        var artigo = _artigos[0];

        await Navigation.PushAsync(
            new ArtigoPage(
                _user,
                artigo.Titulo,
                artigo.Categoria,
                artigo.Conteudo));
    }

    private async void MenuButton_Clicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet(
            "Menu",
            "Cancelar",
            null,
            "Dashboard",
            "Lume",
            "Minhas Economias",
            "Resumo Mensal",
            "Configurações");

        if (action == "Dashboard")
        {
            await Navigation.PushAsync(
                new DashboardPage(_user));
        }

        if (action == "Lume")
        {
            await Navigation.PushAsync(
                new ChatPage(_user));
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
        }

        if (action == "Configurações")
        {
            await Navigation.PushAsync
                (new ConfiguracoesPage(_user));
        }
    }
}