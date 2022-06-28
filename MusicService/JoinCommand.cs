using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("join", "Start the vibe session and add JukeBox.")]
    public async Task JoinCommandAsync()
    {
        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("❌ You must be in a voice channel to run JukeBox.");
            return;
        }

        if (_audioService.HasPlayer(Context.Guild.Id) && Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("❌ You must be in the same voice channel to run JukeBox.");
            return;
        }

        try
        {
            var jukeBox = await _audioService.JoinAsync<QueuedLavalinkPlayer>(Context.Guild.Id, userVoiceState.VoiceChannel.Id, true);

            await RespondAsync($"JukeBox connected to {userVoiceState.VoiceChannel.Name}.");
        }
        catch (Exception e)
        {
            await RespondAsync($"Failed to join the voice channel: {e.Message}");
        }
    }
}