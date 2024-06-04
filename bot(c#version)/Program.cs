using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

class Program
{
    private DiscordSocketClient _client;
    private StreamWriter _writer; // Declarei o StreamWriter aqui para ficar no escopo global

    static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        string filePath = @"C:\Users\enzo2\Documents\MyProjects\discord-bot\banco_de_dados.txt";

        _writer = new StreamWriter(filePath, true); // O segundo parâmetro "true" indica para adicionar ao arquivo se ele já existir

        #region Discord fundamental settings
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 1000,
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildPresences | GatewayIntents.GuildMembers
        });

        _client.Ready += OnReady;
        _client.MessageReceived += MessageReceivedAsync;

        string token = "";
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Aguarda até que o bot seja desconectado manualmente
        await Task.Delay(-1);
        #endregion
    }

    #region OnReadyMethod
    private async Task OnReady()
    {
        Console.WriteLine("Bot is ready.");

        var generalChannel = _client.GetGuild(1247018046233640981).GetTextChannel(1247249629070753862);
        await generalChannel.SendMessageAsync("Olá! Estou online!");
    }
    #endregion

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.Id != 1247248953485955073)
        {   
            DateTime crntTime = DateTime.Now;
            await _writer.WriteLineAsync($"Mensagem recebida: {message.Author} - {crntTime}");
            Console.WriteLine($"Update no Banco de Dados - {crntTime}");

            await _writer.FlushAsync(); // para gravar instantaneamente os arquivos
        }
    }

    private async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState before, SocketVoiceState after) // Log de canal de voz
    {
        if (before.VoiceChannel == null && after.VoiceChannel != null)
        {
            // O usuário entrou em um canal de voz
            await _writer.WriteLineAsync($"{DateTime.Now}: {user.Username} entrou no canal de voz {after.VoiceChannel.Name}");
        }
        else if (before.VoiceChannel != null && after.VoiceChannel == null)
        {
            // O usuário saiu de um canal de voz
            await _writer.WriteLineAsync($"{DateTime.Now}: {user.Username} saiu do canal de voz {before.VoiceChannel.Name}");
        }
        else if (before.VoiceChannel != null && after.VoiceChannel != null && before.VoiceChannel.Id != after.VoiceChannel.Id)
        {
            // O usuário mudou de um canal de voz para outro
            await _writer.WriteLineAsync($"{DateTime.Now}: {user.Username} mudou do canal de voz {before.VoiceChannel.Name} para {after.VoiceChannel.Name}");
        }

        await _writer.FlushAsync();
    }
}
