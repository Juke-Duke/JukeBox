using Discord;
using Discord.Interactions;
using Lavalink4NET.Artwork;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("play", "Set JukeBox's current vibe.")]
    public async Task PlayCommandAsync([Summary("vibe", "The name or youtube link of the media to vibe to.")] string vibe)
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in a voice channel to set JukeBox's vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (_audioService.HasPlayer(Context.Guild.Id) && Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to set the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var setVibe = await _audioService.GetTrackAsync(vibe, SearchMode.YouTube);

        if (setVibe is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("No vibe could be found for JukeBox.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            try
            {
                await _audioService.JoinAsync<QueuedLavalinkPlayer>(Context.Guild.Id, userVoiceState.VoiceChannel.Id, true);
            }
            catch (Exception e)
            {
                await RespondAsync($"Failed to join the voice channel: {e.Message}");
                return;
            }
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        var pos = await jukeBox.PlayAsync(setVibe);
        // using var artworkService = new ArtworkService();
        // var artworkUri = (await artworkService.ResolveAsync(song))!;

        if (pos > 0)
            embed.WithAuthor($"✅ Vibe Queued by {Context.User.Username}");
        else
            embed.WithAuthor($"✅ Vibe Set by {Context.User.Username}");

        embed.WithTitle(setVibe.Title)
             .WithThumbnailUrl(GetYoutubeThumbnailUrl(setVibe.Uri))
             .AddField("Channel", setVibe.Author, true)
             .AddField("Duration", setVibe.Duration.ToString("d':'hh':'mm':'ss"), true)
             .AddField("Position", pos, true);

        await RespondAsync(embed: embed.Build());
    }

    private static string GetYoutubeThumbnailUrl(Uri? videoUrl)
    {
        string fallbackThumbnailUrl = "";
        if (videoUrl == null) return fallbackThumbnailUrl;

        List<string> paths = videoUrl.LocalPath         // path: "/shorts/videoId" or "/watch"
            .Split("/")                                 // ["", "shorts", "videoId"] or ["", "watch"]
            .Where(path => path.Length != 0).ToList();  // ["shorts", "videoId"] or ["watch"]
        // should be [ "shorts", "videoId" ] or ["watch"]

        string videoId;
        if (paths[0] == "watch") videoId = ParseWatchId(videoUrl);
        else if (paths[0] == "shorts") videoId = ParseShortsId(videoUrl);
        else return fallbackThumbnailUrl;

        return $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";
    }

    // URL should be /watch?v=videoId

    private static string ParseWatchId(Uri videoUrl)
    {
        string fallbackThumbnailUrl = "";
        try
        {
            // will be in format: `?v=videoId&key=value&...`
            string urlQuery = videoUrl.Query[1..]; // skip the '?'
            string[] queries = videoUrl.Query[1..].Split("&"); // parse to array of `key=value`
            string videoKey = "v";
            string videoQuery = queries.First(query => query.Split("=")[0] == videoKey);

            if (videoKey == null) return fallbackThumbnailUrl;
            string videoId = videoQuery.Split('=')[1];

            return videoId;
        }
        catch
        {
            return fallbackThumbnailUrl; // failed parse
        }
    }

    // URL should be /shorts/videoId
    private static string ParseShortsId(Uri videoUrl)
    {
        string fallbackThumbnailUrl = "";
        try
        {
            string[] paths = videoUrl.Segments; // should be [ "/", "shorts/", "videoId" ]
            return paths.Last();
        }
        catch
        {
            return fallbackThumbnailUrl;
        }
    }
}

