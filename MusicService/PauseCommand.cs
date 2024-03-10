using Discord;
using Discord.Interactions;
using Lavalink4NET.Players;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("pause", "Pause JukeBox's current vibe.")]
    public async Task PauseCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        if (jukeBox.State is PlayerState.Paused)
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
