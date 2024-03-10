using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using JukeBox;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Idle;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using static Discord.GatewayIntents;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<JukeBoxService>()
            .AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace))
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = AllUnprivileged & ~GuildInvites & ~GuildScheduledEvents,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddLavalink()
            .AddInactivityTracking()
            .Configure<IdleInactivityTrackerOptions>(config =>
            {
                config.Timeout = TimeSpan.FromMinutes(30);
            })
            .Configure<UsersInactivityTrackerOptions>(config =>
            {
                config.Timeout = TimeSpan.FromSeconds(1);
            });
    }).Build();

using (var scope = host.Services.CreateScope())
{
    var client = scope.ServiceProvider.GetRequiredService<DiscordSocketClient>();
    var slashCommands = scope.ServiceProvider.GetRequiredService<InteractionService>();
    await scope.ServiceProvider.GetRequiredService<InteractionHandler>().InitializeAsync();

    client.Ready += async () =>
    {
        await slashCommands.RegisterCommandsGloballyAsync();
    };

    await host.RunAsync();
}
