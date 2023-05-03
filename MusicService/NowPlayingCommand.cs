using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("nowplaying", "Display JukeBox's current vibe.")]
    public async Task NowPlayingCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("JukeBox is not in a vibe session.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.NotPlaying)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("There is no vibe currently.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var vibe = jukeBox.CurrentTrack!;

        embed.WithAuthor($"Current Vibe {(jukeBox.IsLooping ? "🔁" : "")}")
             .WithTitle(vibe.Title)
             .AddField("Channel", vibe.Author, true)
             .AddField("Timestamp", $"{jukeBox.Position.Position:d':'hh':'mm':'ss} / {vibe.Duration:d':'hh':'mm':'ss}", true)
             .AddField("Next Vibe", jukeBox.Queue.FirstOrDefault()?.Title ?? "-", true);

        await RespondAsync(embed: embed.Build());
        await FollowupAsync(vibe.Source);
    }
}
