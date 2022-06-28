using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("disconnect", "End the vibe session and disconnect JukeBox.")]
    public async Task DisconnectCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("❌ JukeBox is not in a vibe session.");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("❌ You must be in a voice channel to disconnect JukeBox.");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("❌ You must be in the same voice channel to disconnect JukeBox.");
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        await jukeBox.DisconnectAsync();
        await RespondAsync($"JukeBox disconnected from {userVoiceState.VoiceChannel.Name}.");
    }
}