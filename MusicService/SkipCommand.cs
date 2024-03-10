using Discord;
using Discord.Interactions;
using Lavalink4NET.Players.Queued;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("skip", "Skip JukeBox's current vibe.")]
    public async Task SkipCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        if (jukeBox.CurrentTrack is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Jukebox has no vibe currently.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var skippedVibe = jukeBox.CurrentTrack!;

        jukeBox.RepeatMode = TrackRepeatMode.None;

        await jukeBox.SkipAsync();

        embed.WithAuthor($"✅ Vibe Skipped by {Context.User.Username}")
             .WithTitle(skippedVibe.Title)
             .AddField("Current Vibe", jukeBox.CurrentTrack?.Title ?? "-")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}
