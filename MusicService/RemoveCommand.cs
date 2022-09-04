using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("remove", "Remove a vibe from JukeBox's queue.")]
    public async Task RemoveCommandAsync([Summary("position", "The position of the vibe to remove from queue.")] int position)
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
                 .WithTitle("You must be in a voice channel to remove the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to remove the vibe.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var jukeBox = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

        if (position == 0)
        {
            await jukeBox.StopAsync();

            embed.WithAuthor($"✅ Vibe Stopped by {Context.User.Username}")
                 .WithTitle($"JukeBox's vibe stopped.");

            await RespondAsync(embed: embed.Build());
            return;
        }
        else if (position < 0 || position > jukeBox.Queue.Count)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("Vibe position is off the grid");

            await RespondAsync(embed: embed.Build());
            return;
        }

        var queue = jukeBox.Queue;
        var vibe = queue[position - 1];

        queue.RemoveAt(position - 1);

        embed.WithAuthor($"✅ Vibe Removed by {Context.User.Username}")
             .WithTitle(vibe.Title)
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}