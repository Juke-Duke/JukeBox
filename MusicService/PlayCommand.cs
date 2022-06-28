using Discord;
using Discord.Interactions;
using Lavalink4NET.Artwork;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("play", "Set JukeBox's current vibe.")]
    public async Task PlayCommandAsync(string track)
    {
        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("❌ You must be in a voice channel to set JukeBox's vibe.");
            return;
        }

        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("❌ JukeBox is not in a vibe session.");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("❌ You must be in the same voice channel to set JukeBox's vibe.");
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        var vibe = await _audioService.GetTrackAsync(track, SearchMode.YouTube);

        if (vibe is null)
        {
            await RespondAsync("❌ No vibe could be found for JukeBox.");
            return;
        }

        var pos = await jukeBox.PlayAsync(vibe);
        // using var artworkService = new ArtworkService();
        // var artworkUri = (await artworkService.ResolveAsync(song))!;

        var embedBuilder = new EmbedBuilder();
        if (pos > 0)
            embedBuilder.WithAuthor($"Vibe Queued");
        else
            embedBuilder.WithAuthor($"Vibe Now Playing");

        var embed = embedBuilder
                    .WithTitle(vibe.Title)
                    .WithThumbnailUrl(Context.User.GetAvatarUrl())
                    .AddField("Channel", vibe.Author, true)
                    .AddField("Duration", vibe.Duration, true)
                    .AddField("Position", pos, true)
                    .WithColor(new Color(102, 196, 166))
                    .Build();

        await RespondAsync(embed: embed);
    }
}