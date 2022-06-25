using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("disconnect", "You can kick me, but you know where you'll end up? Back to me.")]
    public async Task DisconnectCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("Man im dead, HAHA");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("Come here and kick me then daddy");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("So close, but nah");
            return;
        }

        var player = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        await player.DisconnectAsync();
        await RespondAsync("Man fudge this, imma catch you at work");
    }
}