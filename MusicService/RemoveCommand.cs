using Discord;
using Discord.Interactions;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("remove", "Remove a vibe from JukeBox's queue.")]
    public async Task RemoveCommandAsync([Summary("position", "The position of the vibe to remove from queue.")] int position)
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

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

        await queue.RemoveAtAsync(position - 1);

        embed.WithAuthor($"✅ Vibe Removed by {Context.User.Username}")
             .WithTitle(vibe.Track?.Title)
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}
