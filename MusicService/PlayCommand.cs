using Discord;
using Discord.Interactions;
using Lavalink4NET.Rest.Entities.Tracks;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("play", "Set JukeBox's current vibe.", runMode: RunMode.Async)]
    public async Task PlayCommandAsync([Summary("vibe", "The name or youtube link of the media to vibe to.")] string vibe)
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        var track = await _audioService.Tracks
            .LoadTrackAsync(vibe, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        if (track is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("No vibe could be found for JukeBox.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var pos = await jukeBox.PlayAsync(track);

        if (pos > 0)
            embed.WithAuthor($"✅ Vibe Queued by {Context.User.Username}");
        else
            embed.WithAuthor($"✅ Vibe Set by {Context.User.Username}");

        embed.WithTitle(track.Title)
             .WithThumbnailUrl(GetYoutubeThumbnailUrl(track.Uri) ?? Context.User.GetAvatarUrl())
             .AddField("Channel", track.Author, true)
             .AddField("Duration", track.Duration.ToString("d':'hh':'mm':'ss"), true)
             .AddField("Position", pos, true);

        await RespondAsync(embed: embed.Build());
    }

    private static string? GetYoutubeThumbnailUrl(Uri? videoUrl)
    {
        if (videoUrl is null)
            return null;

        var paths = videoUrl.LocalPath          // path: "/shorts/videoId" or "/watch"
            .Split("/")                         // ["", "shorts", "videoId"] or ["", "watch"]
            .Where(path => path.Length != 0)    // ["shorts", "videoId"] or ["watch"]
            .ToList();
        // should be [ "shorts", "videoId" ] or ["watch"]

        var videoId = paths[0] switch
        {
            "watch" => ParseWatchId(videoUrl),
            "shorts" => ParseShortsId(videoUrl),
            _ => null
        };

        if (videoId is null)
            return null;

        return $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";
    }

    // URL should be /watch?v=videoId
    private static string? ParseWatchId(Uri videoUrl)
    {
        try
        {
            // will be in format: `?v=videoId&key=value&...`
            var urlQuery = videoUrl.Query[1..]; // skip the '?'
            var queries = urlQuery.Split("&"); // parse to array of `key=value`
            var videoKey = "v";
            var videoQuery = queries.First(query => query.Split("=")[0] == videoKey);
            var videoId = videoQuery.Split('=')[1];

            return videoId;
        }
        catch
        {
            return null; // failed parse
        }
    }

    // URL should be /shorts/videoId
    private static string? ParseShortsId(Uri videoUrl)
    {
        var paths = videoUrl.Segments; // should be [ "/", "shorts/", "videoId" ]

        if (paths.Length == 0)
            return null;

        return paths.Last();
    }
}

