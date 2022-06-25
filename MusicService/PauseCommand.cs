using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    // Checks to see if the bot is in a voice channel
    // Checks to see if the user is in a voice channel
    // Checks to see if the bot is in the same voice channel as the user
    // Checks to see if the bot is already paused
    // Pauses if the bot is playing

    [SlashCommand("pause", "Pause.")]
    public async Task PauseCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("I'm off the grid right now bro");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("Come in here and make me");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("You cant even hear me");
            return;
        }

        var player = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (player.State == PlayerState.Paused)
        {
            await RespondAsync("I am paused tho?");
            return;
        }

        await player.PauseAsync();
        await RespondAsync("Pause.");
    }
}
