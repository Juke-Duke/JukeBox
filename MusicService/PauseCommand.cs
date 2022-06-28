using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("pause", "Pause JukeBox's current vibe.")]
    public async Task PauseCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("❌ JukeBox is not in a vibe session.");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("❌ You must be in a voice channel to pause JukeBox.");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("❌ You must be in the same voice channel to pause JukeBox.");
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.Paused)
        {
            await RespondAsync("❌ JukeBox is already paused.");
            return;
        }

        await jukeBox.PauseAsync();
        await RespondAsync("JukeBox's vibe paused.");
    }
}
