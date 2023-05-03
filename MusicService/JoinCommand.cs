using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("join", "Start the vibe session and add JukeBox.")]
    public async Task JoinCommandAsync()
    {
        await RespondAsync("/join is depracted, use /play and JukeBox will join in with the good vibes");
    }
}
