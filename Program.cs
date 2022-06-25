using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using JukeBox;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<JukeBoxService>()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildScheduledEvents,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton(new LavalinkNodeOptions());
    }).Build();


using (var scope = host.Services.CreateScope())
{
    var client = scope.ServiceProvider.GetRequiredService<DiscordSocketClient>();
    var slashCommands = scope.ServiceProvider.GetRequiredService<InteractionService>();
    await scope.ServiceProvider.GetRequiredService<InteractionHandler>().InitializeAsync();

    client.Ready += async () =>
    {
        await scope.ServiceProvider.GetRequiredService<IAudioService>().InitializeAsync();
        await slashCommands.RegisterCommandsGloballyAsync();
    };

    await host.RunAsync();
}