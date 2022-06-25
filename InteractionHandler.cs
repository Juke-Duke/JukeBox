using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace JukeBox;
public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _slashCommands;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService slashCommands, IServiceProvider services)
    {
        _client = client;
        _slashCommands = slashCommands;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        _client.InteractionCreated += HandleInteraction;
        await _slashCommands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            var context = new SocketInteractionContext(_client, arg);

            await _slashCommands.ExecuteCommandAsync(context, _services);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}