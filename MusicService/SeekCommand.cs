using Discord;
using Discord.Interactions;
using Lavalink4NET.Players;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("seek", "Seek a timestamp in JukeBox's current vibe.")]
    public async Task SeekCommandAsync([Summary("timeStamp", "The timestamp of the vibe to seek to in 00:00:00 format.")] string timeStamp)
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

        if (!TimeSpan.TryParse(timeStamp, out var validTimeStamp))
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Invalid timestamp.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (validTimeStamp == TimeSpan.Zero)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Invalid timestamp.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (validTimeStamp > jukeBox.CurrentTrack!.Duration)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You are seeking off the grid.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        await jukeBox.SeekAsync(validTimeStamp);

        embed.WithAuthor($"✅ Vibe Seeked to {validTimeStamp.ToString("d':'hh':'mm':'ss")} by {Context.User.Username}")
             .WithTitle(jukeBox.CurrentTrack!.Title)
             .AddField("Current Vibe", jukeBox.CurrentTrack?.Title ?? "-")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}
