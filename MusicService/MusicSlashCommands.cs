using Discord.Interactions;
using Lavalink4NET;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    public MusicSlashCommands(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("ping", "Check JukeBox's vibe pulse.")]
    public async Task PingCommand()
    {
        await RespondAsync($"Pong {Context.User.Mention}!");
    }
}