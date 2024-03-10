using Discord;
using Discord.Interactions;
using Lavalink4NET.Players;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("queue", "Display the vibes in JukeBox's queue.")]
    public async Task QueueCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        if (jukeBox.CurrentTrack is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("There are no vibes in the queue.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        embed.WithTitle($"Vibe Queue")
             .AddField($"Current Vibe: {jukeBox.CurrentTrack!.Title}", $"{jukeBox.CurrentTrack.Author} - {jukeBox.CurrentTrack.Duration.ToString("d':'hh':'mm':'ss")}");

        int pos = 1;
        foreach (var vibe in jukeBox.Queue)
            embed.AddField($"[{pos++}]. {vibe.Track?.Title}", $"{vibe.Track?.Author} - {vibe.Track?.Duration.ToString("d':'hh':'mm':'ss")}");

        await RespondAsync(embed: embed.Build());
    }
}
