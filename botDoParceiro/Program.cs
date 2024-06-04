using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Net;
using Newtonsoft.Json;

class Program
{
    private DiscordSocketClient _client;
    private Stopwatch _stopwatch; // Declarei o Stopwatch aqui para ficar no escopo global

    static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        _stopwatch = new Stopwatch(); // Inicializa o Stopwatch

        #region Discord fundamental settings
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 1000,
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.GuildPresences |
                             GatewayIntents.GuildMembers
        });

        _client.Log += Log;
        _client.Ready += OnReady;
        _client.InteractionCreated += InteractionCreatedAsync;

        string token = "MTI0NzM5NjY2MjM3NzMxNjM5Mg.G4-_qF.d2NE8F6YIULMGum_Urkxi7VGxKL1FLWXpcY_KE";

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
        #endregion
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    #region OnReadyMethod
    private async Task OnReady()
    {
        Console.WriteLine("Bot was turned on");

        var generalChannel = _client.GetGuild(1202621828300935190)?.GetTextChannel(1202621829911683160);
        if (generalChannel != null)
        {
            await generalChannel.SendMessageAsync("Chegueiii");
        }
        else
        {
            Console.WriteLine("Canal geral não encontrado.");
        }

        var guild = _client.GetGuild(1202621828300935190); // Substitua pelo ID do seu servidor

        var startCommand = new SlashCommandBuilder()
            .WithName("start")
            .WithDescription("Inicia o cronômetro.");

        var stopCommand = new SlashCommandBuilder()
            .WithName("stop")
            .WithDescription("Para o cronômetro e mostra o tempo decorrido.");

        try
        {
            await guild.CreateApplicationCommandAsync(startCommand.Build());
            await guild.CreateApplicationCommandAsync(stopCommand.Build());
            Console.WriteLine("Slash commands '/start' e '/stop' registrados com sucesso.");
        }
        catch (HttpException ex)
        {
            var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
    #endregion

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        if (interaction is SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "start":
                    await HandleStartCommand(command);
                    break;
                case "stop":
                    await HandleStopCommand(command);
                    break;
            }
        }
    }

    private async Task HandleStartCommand(SocketSlashCommand command)
    {
        _stopwatch.Restart();
        await command.RespondAsync("Cronômetro iniciado.");
    }

    private async Task HandleStopCommand(SocketSlashCommand command)
    {
        _stopwatch.Stop();
        await command.RespondAsync($"Cronômetro parado. Tempo decorrido: {_stopwatch.Elapsed}");
    }
}
