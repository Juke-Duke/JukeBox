using Discord;
using Discord.WebSocket;

namespace JukeBox;
public class JukeBoxService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _config;

    public JukeBoxService(DiscordSocketClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += async Task (LogMessage msg) =>
        {
            Console.WriteLine(msg.Message);
            await Task.CompletedTask;
        };

        var token = _config.GetRequiredSection("Token").Value;
        await _client.LoginAsync(TokenType.Bot, token, validateToken: true);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
       await _client.LogoutAsync();
       await _client.StopAsync();
    }
}