using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("resume", "Run that shizz")]
    public async Task ResumeCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("Im not gonna lie, I'm tryna resume my jack off session rn.");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("Resume typing bro bro");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("I know you want to resume this, its fye on gah, and you aint even here to hear it");
            return;
        }

        var player = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (player.State != PlayerState.Paused)
        {
            await RespondAsync("Bro this shizz already playing u skitzo");
            return;
        }

        await player.ResumeAsync();
        await RespondAsync("We back.");
    }
}
