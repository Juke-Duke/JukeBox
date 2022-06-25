using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("skip", "Go next lol")]
    public async Task SkipCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("Skip what? I'm not vibing");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("Skip into call first");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("Atleast come here and listen to it damn");
            return;
        }

        var player = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (player.State == PlayerState.NotPlaying)
        {
            await RespondAsync("I got nothing chief, yall music taste is bad");
            return;
        }

        await player.SkipAsync();
        await RespondAsync("Neeeeeeeeeeeeeext");
    }
}
