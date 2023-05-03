using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using JukeBox;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Tracking;

using static Discord.GatewayIntents;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<JukeBoxService>()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = AllUnprivileged & ~GuildInvites & ~GuildScheduledEvents,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<InactivityTrackingService>()
            .AddSingleton(new InactivityTrackingOptions
            {
                TrackInactivity = true,
                DisconnectDelay = TimeSpan.FromMinutes(30)
            })
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
        scope.ServiceProvider.GetRequiredService<InactivityTrackingService>();
        await slashCommands.RegisterCommandsGloballyAsync();
    };

    await host.RunAsync();
}
