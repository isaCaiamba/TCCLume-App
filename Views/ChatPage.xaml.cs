using System.Linq;
using System.Net.Http.Json;
using TCCLume.Models;
using TCCLume.Services;

namespace TCCLume.Views;

public partial class ChatPage : ContentPage
{
    private readonly User _user;

    private readonly DatabaseService _database;

    private readonly HttpClient _httpClient;

    private VerticalStackLayout? _digitandoContainer;


    // =====================================================
    // HISTÓRICO TEMPORÁRIO DO CHAT
    // =====================================================

    private static readonly List<ChatMessage> Historico =
        new();


    public ChatPage(User user)
    {
        InitializeComponent();

        _user = user;

        _database = new DatabaseService();

        _httpClient = new HttpClient();


        CarregarHistorico();
    }


    // =====================================================
    // CARREGAR HISTÓRICO
    // =====================================================

    private void CarregarHistorico()
    {
        MessagesLayout.Children.Clear();


        // Mensagem inicial
        AdicionarMensagemNaTela(
            "Lume",
            "Olá! 👋\n\nEstou aqui para ajudar você a organizar suas finanças. Pode me perguntar sobre seus gastos, receitas ou metas.",
            false,
            false);


        // Histórico
        foreach (var mensagem in Historico)
        {
            AdicionarMensagemNaTela(
                mensagem.Remetente,
                mensagem.Texto,
                mensagem.EhUsuario,
                false);
        }
    }


    // =====================================================
    // BOTÃO ENVIAR
    // =====================================================

    private async void EnviarMensagem_Clicked(
        object sender,
        EventArgs e)
    {
        await EnviarMensagem();
    }


    // =====================================================
    // ENTER
    // =====================================================

    private async void MessageEntry_Completed(
        object sender,
        EventArgs e)
    {
        await EnviarMensagem();
    }


    // =====================================================
    // ENVIAR
    // =====================================================

    private async Task EnviarMensagem()
    {
        string mensagem =
            MessageEntry.Text?.Trim() ?? "";


        if (string.IsNullOrWhiteSpace(mensagem))
            return;


        // =================================================
        // ANIMAÇÃO DO BOTÃO
        // =================================================

        await SendButton.ScaleTo(
            0.80,
            80);

        await SendButton.ScaleTo(
            1.0,
            100);


        // =================================================
        // ADICIONAR MENSAGEM DO USUÁRIO
        // =================================================

        AdicionarMensagemNaTela(
            "Você",
            mensagem,
            true,
            true);


        Historico.Add(
            new ChatMessage
            {
                Remetente = "Você",
                Texto = mensagem,
                EhUsuario = true
            });


        // Limpar campo
        MessageEntry.Text = "";


        // =================================================
        // DESABILITAR ENVIO
        // =================================================

        SendButton.IsEnabled = false;

        MessageEntry.IsEnabled = false;


        // =================================================
        // MOSTRAR "DIGITANDO..."
        // =================================================

        var digitando =
            AdicionarDigitando();


        try
        {
            // =================================================
            // BUSCAR TRANSAÇÕES
            // =================================================

            List<Transaction> transacoes =
                await _database.GetTransactions(
                    _user.Id);


            // =================================================
            // BUSCAR METAS
            // =================================================

            List<FinancialGoal> metas =
                await _database.GetFinancialGoals(
                    _user.Id);


            // =================================================
            // MONTAR REQUISIÇÃO
            // =================================================

            var request =
                new ChatRequest
                {
                    UserId = _user.Id,

                    Mensagem = mensagem,

                    Transacoes =
                        transacoes
                            .Select(x =>
                                new TransacaoContexto
                                {
                                    Titulo = x.Titulo,
                                    Valor = x.Valor,
                                    Tipo = x.Tipo,
                                    Categoria = x.Categoria,
                                    Data = x.Data
                                })
                            .ToList(),

                    Metas =
                        metas
                            .Select(x =>
                                new MetaContexto
                                {
                                    Nome = x.Nome,
                                    ValorMeta = x.ValorMeta,
                                    ValorGuardado = x.ValorGuardado
                                })
                            .ToList()
                };


            // =================================================
            // API
            // =================================================

            string url =
                "https://localhost:7051/api/Chat";


            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    url,
                    request);


            // =================================================
            // REMOVER DIGITANDO
            // =================================================

            if (_digitandoContainer != null)
            {
                MessagesLayout.Children.Remove(
                    _digitandoContainer);

                _digitandoContainer = null;
            }


            // =================================================
            // VERIFICAR API
            // =================================================

            if (!response.IsSuccessStatusCode)
            {
                AdicionarMensagemNaTela(
                    "Lume",
                    "Desculpe, não consegui acessar meu servidor no momento. 😕",
                    false,
                    true);

                return;
            }


            // =================================================
            // LER RESPOSTA
            // =================================================

            ChatResponse? resultado =
                await response.Content
                    .ReadFromJsonAsync<ChatResponse>();


            string resposta =
                resultado?.Resposta
                ?? "Não consegui gerar uma resposta.";


            // =================================================
            // MOSTRAR RESPOSTA
            // =================================================

            AdicionarMensagemNaTela(
                "Lume",
                resposta,
                false,
                true);


            // =================================================
            // SALVAR HISTÓRICO
            // =================================================

            Historico.Add(
                new ChatMessage
                {
                    Remetente = "Lume",
                    Texto = resposta,
                    EhUsuario = false
                });
        }
        catch (Exception)
        {
            if (_digitandoContainer != null)
            {
                MessagesLayout.Children.Remove(
                    _digitandoContainer);

                _digitandoContainer = null;
            }

            AdicionarMensagemNaTela(
                "Lume",
                "Não consegui me conectar ao servidor. Verifique se a API está funcionando. ⚠️",
                false,
                true);
        }

        finally
        {
            SendButton.IsEnabled = true;

            MessageEntry.IsEnabled = true;

            MessageEntry.Focus();
        }
    }


    // =====================================================
    // ADICIONAR MENSAGEM
    // =====================================================

    private void AdicionarMensagemNaTela(
        string remetente,
        string texto,
        bool ehUsuario,
        bool animar)
    {
        var container =
            new VerticalStackLayout
            {
                Spacing = 3,

                HorizontalOptions =
                    ehUsuario
                        ? LayoutOptions.End
                        : LayoutOptions.Start,

                MaximumWidthRequest = 320,

                Opacity = animar ? 0 : 1
            };


        // =================================================
        // NOME
        // =================================================

        var nome =
            new Label
            {
                Text = remetente,

                FontSize = 11,

                FontAttributes =
                    FontAttributes.Bold,

                TextColor =
                    ehUsuario
                        ? Color.FromArgb("#8E93A3")
                        : Color.FromArgb("#F2C94C"),

                HorizontalTextAlignment =
                    ehUsuario
                        ? TextAlignment.End
                        : TextAlignment.Start
            };


        // =================================================
        // BOLHA
        // =================================================

        var bolha =
            new Frame
            {
                BackgroundColor =
                    ehUsuario
                        ? Color.FromArgb("#F2C94C")
                        : Color.FromArgb("#121424"),

                BorderColor =
                    ehUsuario
                        ? Color.FromArgb("#F2C94C")
                        : Color.FromArgb("#2A2D3E"),

                CornerRadius = 18,

                Padding =
                    new Thickness(
                        15,
                        11),

                HasShadow = false
            };


        // =================================================
        // TEXTO
        // =================================================

        var textoLabel =
            new Label
            {
                Text = texto,

                FontSize = 15,

                TextColor =
                    ehUsuario
                        ? Colors.Black
                        : Colors.White,

                LineBreakMode =
                    LineBreakMode.WordWrap
            };


        bolha.Content =
            textoLabel;


        container.Children.Add(nome);

        container.Children.Add(bolha);


        MessagesLayout.Children.Add(
            container);


        // =================================================
        // ANIMAÇÃO
        // =================================================

        if (animar)
        {
            _ = AnimarMensagem(container);
        }


        RolarParaFinal();
    }


    // =====================================================
    // ANIMAÇÃO DA MENSAGEM
    // =====================================================

    private async Task AnimarMensagem(
        VisualElement elemento)
    {
        elemento.TranslationY = 12;

        await Task.WhenAll(
            elemento.FadeTo(
                1,
                180),

            elemento.TranslateTo(
                0,
                0,
                180,
                Easing.CubicOut));
    }


    // =====================================================
    // "DIGITANDO..."
    // =====================================================

    private VerticalStackLayout AdicionarDigitando()
    {
        var container =
            new VerticalStackLayout
            {
                Spacing = 3,

                HorizontalOptions =
                    LayoutOptions.Start,

                MaximumWidthRequest = 200
            };


        var nome =
            new Label
            {
                Text = "Lume",

                FontSize = 11,

                FontAttributes =
                    FontAttributes.Bold,

                TextColor =
                    Color.FromArgb("#F2C94C")
            };


        var bolha =
            new Frame
            {
                BackgroundColor =
                    Color.FromArgb("#121424"),

                BorderColor =
                    Color.FromArgb("#2A2D3E"),

                CornerRadius = 18,

                Padding =
                    new Thickness(
                        15,
                        10),

                HasShadow = false
            };


        var texto =
            new Label
            {
                Text = "Digitando...",

                FontSize = 14,

                FontAttributes =
                    FontAttributes.Bold,

                TextColor =
                    Color.FromArgb("#A8AFB5")
            };


        bolha.Content = texto;


        container.Children.Add(nome);

        container.Children.Add(bolha);


        MessagesLayout.Children.Add(container);


        // Guardar qual é o container do "Digitando..."
        _digitandoContainer = container;


        // Iniciar animação
        _ = AnimarDigitando(
            texto,
            container);


        RolarParaFinal();


        return container;
    }


    // =====================================================
    // ANIMAÇÃO DO "DIGITANDO..."
    // =====================================================

    private async Task AnimarDigitando(
    Label label,
    VerticalStackLayout container)
    {
        while (_digitandoContainer == container)
        {
            await label.FadeTo(
                0.35,
                500);

            if (_digitandoContainer != container)
                break;

            await label.FadeTo(
                1,
                500);
        }
    }

    // =====================================================
    // ROLAR PARA ÚLTIMA MENSAGEM
    // =====================================================

    private void RolarParaFinal()
    {
        Dispatcher.StartTimer(
            TimeSpan.FromMilliseconds(100),
            () =>
            {
                MainThread.BeginInvokeOnMainThread(
                    async () =>
                    {
                        await MessagesScrollView
                            .ScrollToAsync(
                                0,
                                MessagesLayout.Height,
                                true);
                    });

                return false;
            });
    }


    // =====================================================
    // MENU
    // =====================================================

    private async void MenuButton_Clicked(
        object sender,
        EventArgs e)
    {
        string action =
            await DisplayActionSheet(
                "Menu",
                "Cancelar",
                null,
                "Dashboard",
                "Minhas Economias",
                "Resumo Mensal",
                "Blog",
                "Configurações");


        if (action == "Dashboard")
        {
            await Navigation.PushAsync(
                new DashboardPage(_user));

            return;
        }


        if (action == "Minhas Economias")
        {
            await Navigation.PushAsync(
                new MinhasEconomiasPage(_user));

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

            return;
        }


        if (action == "Configurações")
        {
            await Navigation.PushAsync(
                new ConfiguracoesPage(_user));

            return;
        }
    }
}


// =========================================================
// HISTÓRICO
// =========================================================

public class ChatMessage
{
    public string Remetente { get; set; } =
        string.Empty;

    public string Texto { get; set; } =
        string.Empty;

    public bool EhUsuario { get; set; }
}


// =========================================================
// REQUEST
// =========================================================

public class ChatRequest
{
    public int UserId { get; set; }

    public string Mensagem { get; set; } =
        string.Empty;

    public List<TransacaoContexto> Transacoes { get; set; } =
        new();

    public List<MetaContexto> Metas { get; set; } =
        new();
}


// =========================================================
// TRANSAÇÃO
// =========================================================

public class TransacaoContexto
{
    public string Titulo { get; set; } =
        string.Empty;

    public double Valor { get; set; }

    public string Tipo { get; set; } =
        string.Empty;

    public string Categoria { get; set; } =
        string.Empty;

    public string Data { get; set; } =
        string.Empty;
}


// =========================================================
// META
// =========================================================

public class MetaContexto
{
    public string Nome { get; set; } =
        string.Empty;

    public double ValorMeta { get; set; }

    public double ValorGuardado { get; set; }
}


// =========================================================
// RESPONSE
// =========================================================

public class ChatResponse
{
    public string Resposta { get; set; } =
        string.Empty;
}