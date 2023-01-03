using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Tracking;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    public MusicSlashCommands(IAudioService audioService)
        => _audioService = audioService;

    [SlashCommand("ping", "Check JukeBox's vibe pulse.")]
    public async Task PingCommand()
        => await RespondAsync($"Pulse received, its vibe hours {Context.User.Mention}!");
}