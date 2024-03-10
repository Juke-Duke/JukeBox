using Discord;
using Discord.Interactions;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("disconnect", "End the vibe session and disconnect JukeBox.")]
    public async Task DisconnectCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);

        var jukeBox = await GetJukeBoxAsync(embed);

        if (jukeBox is null)
            return;

        await jukeBox.DisconnectAsync();

        embed.WithAuthor($"✅ JukeBox Disconnected by {Context.User.Username}")
             .WithTitle($"JukeBox disconnected from {(Context.User as IVoiceState)!.VoiceChannel.Name}.")
             .WithThumbnailUrl(Context.User.GetAvatarUrl());

        await RespondAsync(embed: embed.Build());
    }
}
