using Discord;
using Discord.Interactions;
using Lavalink4NET.Players;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("resume", "Resume JukeBox's current vibe.")]
    public async Task ResumeCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        if (jukeBox.State is not PlayerState.Paused)
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
