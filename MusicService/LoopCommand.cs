using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("loop", "Loop JukeBox's current vibe")]
    public async Task LoopCommandAsync()
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
                 .WithTitle("You must be in a voice channel to loop the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to loop the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.NotPlaying)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("JukeBox's vibe is not playing.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var isLooping = jukeBox.LoopMode is not PlayerLoopMode.None;
        jukeBox.LoopMode = isLooping ? PlayerLoopMode.None : PlayerLoopMode.Track;

        if (isLooping)
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