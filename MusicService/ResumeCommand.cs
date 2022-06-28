using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("resume", "Resume JukeBox's current vibe.")]
    public async Task ResumeCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("❌ JukeBox is not in a vibe session.");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("❌ You must be in a voice channel to resume JukeBox's vibe.");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("❌ You must be in the same voice channel to resume JukeBox's vibe.");
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State != PlayerState.Paused)
        {
            await RespondAsync("❌ JukeBox's vibe is not paused.");
            return;
        }

        await jukeBox.ResumeAsync();
        await RespondAsync("JukeBox's vibe resumed.");
    }
}
