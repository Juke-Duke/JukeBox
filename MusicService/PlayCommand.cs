using Discord;
using Discord.Interactions;
using Lavalink4NET.Artwork;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("play", "What we vibin to?")]
    public async Task PlayCommandAsync(string track)
    {
        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("Get in call first buddy");
            return;
        }

        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("Ye bro? Invite me first bozo!");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("In here!!!");
            return;
        }

        var player = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        var song = await _audioService.GetTrackAsync(track, SearchMode.YouTube);

        if (song is null)
        {
            await RespondAsync("Bro english my mans");
            return;
        }

        var pos = await player.PlayAsync(song);
        // using var artworkService = new ArtworkService();
        // var artworkUri = (await artworkService.ResolveAsync(song))!;

        var embedBuilder = new EmbedBuilder();
        if (pos > 0)
            embedBuilder.WithTitle($"Queued up: {song.Title} to the track by DJ {Context.User.Discriminator}");
        else
            embedBuilder.WithTitle($"Now playing: {song.Title} by DJ {Context.User.Username}");

        var embed = embedBuilder
                    .AddField("Channel", song.Author, true)
                    .AddField("Duration", song.Duration, true)
                    .AddField("Position", pos, true)
                    .WithColor(new Color(102, 196, 166))
                    .Build();

        await RespondAsync(embed: embed);
    }
}