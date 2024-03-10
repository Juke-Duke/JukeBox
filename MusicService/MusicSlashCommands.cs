using Discord;
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    public MusicSlashCommands(IAudioService audioService)
        => _audioService = audioService;

    [SlashCommand("ping", "Check JukeBox's vibe pulse.")]
    public async Task PingCommand()
        => await RespondAsync($"Pulse received, its vibe hours {Context.User.Mention}!");


    private async ValueTask<QueuedLavalinkPlayer?> GetJukeBoxAsync(EmbedBuilder embed)
    {
        var channelBehavior = !_audioService.Players.HasPlayer(Context.Guild.Id)
            ? PlayerChannelBehavior.Join
            : PlayerChannelBehavior.None;

        var result = await _audioService.Players
            .RetrieveAsync(
                Context,
                playerFactory: PlayerFactory.Queued,
                new PlayerRetrieveOptions(ChannelBehavior: channelBehavior)
            )
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            _ = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => embed
                    .WithAuthor("❌ Vibe Error")
                    .WithTitle("You must be in a voice channel to set JukeBox's vibe."),
                
                PlayerRetrieveStatus.VoiceChannelMismatch => embed
                    .WithAuthor("❌ Vibe Error")
                    .WithTitle("You must be in the same voice channel to set the vibe."),

                PlayerRetrieveStatus.BotNotConnected => embed
                    .WithAuthor("❌ Vibe Error")
                    .WithTitle("JukeBox is currently not connected."),
                
                _ => embed
                    .WithAuthor("❌ Vibe Error")
                    .WithTitle("An unknown error occurred while setting the vibe.")
            };

            await RespondAsync(embed: embed.Build());
            return null;
        }

        return result.Player;
    }
}
