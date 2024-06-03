using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

class Program
{
    private DiscordSocketClient _client;

    static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 1000,
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildPresences | GatewayIntents.GuildMembers
        });

        _client.Ready += OnReady;

        string token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"); // Acessa o token com variável de ambiente
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Aguarda até que o bot seja desconectado manualmente
        await Task.Delay(-1);
    }

    private async Task OnReady()
    {
        Console.WriteLine("Bot is ready.");

        var generalChannel = _client.GetGuild(1247018046233640981).GetTextChannel(1247249629070753862);
        await generalChannel.SendMessageAsync("Olá! Estou online!");
    }
}
