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
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.NotPlaying)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Ther is no vibe currently.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var vibe = jukeBox.CurrentTrack!;

        embed = new EmbedBuilder()
                   .WithAuthor("Current Vibe")
                   .WithTitle(vibe.Title)
                   .AddField("Channel", vibe.Author, true)
                   .AddField("Timestamp", $"{jukeBox.Position.Position.ToString(@"hh\:mm\:ss")} / {vibe.Duration.ToString(@"hh\:mm\:ss")}", true)
                   .AddField("Next Vibe", jukeBox.Queue.FirstOrDefault()?.Title ?? "-", true);

        await RespondAsync(vibe.Source, embed: embed.Build());
    }
}