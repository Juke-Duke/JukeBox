using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("skip", "Skip JukeBox's current vibe.")]
    public async Task SkipCommandAsync()
    {
        if (!_audioService.HasPlayer(Context.Guild.Id))
        {
            await RespondAsync("❌ JukeBox is not in a vibe session.");
            return;
        }

        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("❌ You must be in a voice channel to skip JukeBox's vibe.");
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("❌ You must be in the same voice channel to skip JukeBox's vibe.");
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.NotPlaying)
        {
            await RespondAsync("❌ JukeBox has no vibe currently.");
            return;
        }

        var skipEmbed = new EmbedBuilder()
                    .WithAuthor("Vibe Skipped")
                    .WithTitle(jukeBox.CurrentTrack!.Title)
                    .WithColor(new Color(102, 196, 166))
                    .Build();

        await jukeBox.SkipAsync();
        await RespondAsync(embed: skipEmbed);

        if (jukeBox.State != PlayerState.NotPlaying)
        {
            var nowEmbed = new EmbedBuilder()
                    .WithAuthor("Vibe Now Playing")
                    .WithTitle(jukeBox.CurrentTrack!.Title)
                    .AddField("Channel", jukeBox.CurrentTrack!.Author, true)
                    .AddField("Timestamp", $"{jukeBox.Position.Position.ToString(@"hh\:mm\:ss")} / {jukeBox.CurrentTrack!.Duration.ToString(@"hh\:mm\:ss")}", true)
                    .AddField("Next Vibe", jukeBox.Queue.FirstOrDefault()?.Title ?? "-", true)
                    .WithColor(new Color(102, 196, 166))
                    .Build();

            await FollowupAsync(embed: nowEmbed);
        }
    }
}