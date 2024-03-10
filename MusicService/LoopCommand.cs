using Discord;
using Discord.Interactions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("loop", "Loop JukeBox's current vibe")]
    public async Task LoopCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        if (jukeBox.State is PlayerState.NotPlaying)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("There is no vibe currently.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        jukeBox.RepeatMode = jukeBox.RepeatMode is TrackRepeatMode.Track
            ? TrackRepeatMode.None
            : TrackRepeatMode.Track;

        if (jukeBox.RepeatMode is TrackRepeatMode.Track)
        {
            embed.WithAuthor($"🔁 Vibe Looped by {Context.User.Username}")
                 .WithTitle($"JukeBox's vibe is now looping.")
                 .WithThumbnailUrl(Context.User.GetAvatarUrl());
        }
        else
        {
            embed.WithAuthor($"🔁 Vibe Stopped Looping by {Context.User.Username}")
                 .WithTitle($"JukeBox's vibe is no longer looping.")
                 .WithThumbnailUrl(Context.User.GetAvatarUrl());
        }

        await RespondAsync(embed: embed.Build());
    }
}
