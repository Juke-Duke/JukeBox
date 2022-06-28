using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("queue", "Display the vibes in JukeBox's queue.")]
    public async Task QueueCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("JukeBox is not in a vibe session.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in a voice channel to display the vibes");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to display the vibes");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.CurrentTrack is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("There are no vibes in the queue.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        embed.WithTitle($"Vibe Queue")
             .AddField($"Current Vibe: {jukeBox.CurrentTrack!.Title}", $"{jukeBox.CurrentTrack.Author} - {jukeBox.CurrentTrack.Duration}");

        int pos = 1;
        foreach (var vibe in jukeBox.Queue)
            embed.AddField($"[{pos++}]. {vibe.Title}", $"{vibe.Author} - {vibe.Duration}");

        await RespondAsync(embed: embed.Build());
    }
}