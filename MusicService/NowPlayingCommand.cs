using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("nowplaying", "Whats poppin atm")]
    public async Task NowPlayingCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("Stop playing with me");
            return;
        }

        var player = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (player.State == PlayerState.NotPlaying)
        {
            await RespondAsync("Playing your mother rn");
            return;
        }

        var song = player.CurrentTrack!;

        var embed = new EmbedBuilder()
                   .WithTitle($"Current Heat: {song.Title}")
                   .AddField("Channel", song.Author, true)
                   .AddField("Timestamp", $"{player.Position.Position.ToString(@"hh\:mm\:ss")} / {song.Duration}", true)
                   .AddField("Next Song", player.Queue.FirstOrDefault()?.Title ?? "-", true)
                   .WithColor(new Color(102, 196, 166))
                   .Build();

        await RespondAsync(embed: embed);
        await FollowupAsync(song.Source);
    }
}