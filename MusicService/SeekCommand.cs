using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("seek", "Seek a timestamp in JukeBox's current vibe.")]
    public async Task SeekCommandAsync([Summary("timeStamp", "The timestamp of the vibe to seek to in 00:00:00 format.")] string timeStamp)
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
                 .WithTitle("You must be in a voice channel to seek through JukeBox's vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to seek through the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.NotPlaying)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Jukebox has no vibe currently.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        TimeSpan.TryParse(timeStamp, out var validTimeStamp);

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

        await jukeBox.SeekPositionAsync(validTimeStamp);

        embed.WithAuthor($"✅ Vibe Seeked to {validTimeStamp} by {Context.User.Username}")
             .WithTitle(jukeBox.CurrentTrack!.Title)
             .AddField("Current Vibe", jukeBox.CurrentTrack?.Title ?? "-")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}