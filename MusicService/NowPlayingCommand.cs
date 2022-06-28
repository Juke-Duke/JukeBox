using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("nowplaying", "Display JukeBox's current vibe.")]
    public async Task NowPlayingCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("❌ JukeBox is not in a vibe session.");
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.NotPlaying)
        {
            await RespondAsync("❌ JukeBox has no vibe currently.");
            return;
        }

        var vibe = jukeBox.CurrentTrack!;

        var embed = new EmbedBuilder()
                   .WithAuthor("Current Vibe")
                   .WithTitle(vibe.Title)
                   .AddField("Channel", vibe.Author, true)
                   .AddField("Timestamp", $"{jukeBox.Position.Position.ToString(@"hh\:mm\:ss")} / {vibe.Duration.ToString(@"hh\:mm\:ss")}", true)
                   .AddField("Next Vibe", jukeBox.Queue.FirstOrDefault()?.Title ?? "-", true)
                   .WithColor(new Color(102, 196, 166))
                   .Build();

        await RespondAsync(vibe.Source, embed: embed);
    }
}