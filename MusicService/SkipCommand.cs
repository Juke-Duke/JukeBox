using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("skip", "Skip JukeBox's current vibe.")]
    public async Task SkipCommandAsync()
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
                 .WithTitle("You must be in a voice channel to skip the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to skip the vibe.");

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

        var skippedVibe = jukeBox.CurrentTrack!;

        jukeBox.LoopMode = PlayerLoopMode.None;

        await jukeBox.SkipAsync();

        embed.WithAuthor($"✅ Vibe Skipped by {Context.User.Username}")
             .WithTitle(skippedVibe.Title)
             .AddField("Current Vibe", jukeBox.CurrentTrack?.Title ?? "-")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}