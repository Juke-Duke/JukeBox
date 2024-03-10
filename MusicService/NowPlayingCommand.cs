using Discord;
using Discord.Interactions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("nowplaying", "Display JukeBox's current vibe.")]
    public async Task NowPlayingCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        if (jukeBox.CurrentTrack is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("There is no vibe currently.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var vibe = jukeBox.CurrentTrack!;
        var isLooping = jukeBox.RepeatMode is TrackRepeatMode.Track;

        embed.WithAuthor($"Current Vibe {(isLooping ? "🔁" : "")}")
             .WithTitle(vibe.Title)
             .AddField("Channel", vibe.Author, true)
             .AddField("Timestamp", $"{jukeBox.Position?.Position:d':'hh':'mm':'ss} / {vibe.Duration:d':'hh':'mm':'ss}", true)
             .AddField("Next Vibe", jukeBox.Queue.Peek()?.Track?.Title ?? "-", true);

        await RespondAsync(embed: embed.Build());
        await FollowupAsync(vibe.Uri!.ToString());
    }
}
