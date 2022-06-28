using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("pause", "Pause JukeBox's current vibe.")]
    public async Task PauseCommandAsync()
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
                 .WithTitle("You must be in a voice channel to pause the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to pause the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (jukeBox.State == PlayerState.Paused)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Vibe is already paused.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        await jukeBox.PauseAsync();

        embed.WithAuthor($"✅ Vibe Paused by {Context.User.Username}")
             .WithTitle($"JukeBox's vibe paused.")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}
