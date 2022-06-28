using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("resume", "Resume JukeBox's current vibe.")]
    public async Task ResumeCommandAsync()
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
                 .WithTitle("You must be in a voice channel to resume the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to resume the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State != PlayerState.Paused)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("JukeBox's vibe is not paused.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        await jukeBox.ResumeAsync();

        embed.WithAuthor($"✅ Vibe Resumed by {Context.User.Username}")
             .WithTitle($"JukeBox's vibe resumed.")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}
