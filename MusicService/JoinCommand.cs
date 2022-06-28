using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("join", "Start the vibe session and add JukeBox.")]
    public async Task JoinCommandAsync()
    {
        var embed = new EmbedBuilder().WithColor(102, 196, 166);
        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in a voice channel to join JukeBox.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        if (_audioService.HasPlayer(Context.Guild.Id) && Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            embed.WithAuthor("❌ Vibe Error")
                 .WithTitle("You must be in the same voice channel to join JukeBox.");

            await RespondAsync(embed: embed.Build());
            return;
        }

        try
        {
            await _audioService.JoinAsync<QueuedLavalinkPlayer>(Context.Guild.Id, userVoiceState.VoiceChannel.Id, true);

            embed.WithAuthor($"✅ JukeBox Joined by {Context.User.Username}")
                 .WithTitle($"JukeBox has joined {userVoiceState.VoiceChannel.Name}.")
                 .WithThumbnailUrl(Context.User.GetAvatarUrl());

            await RespondAsync(embed: embed.Build());
        }
        catch (Exception e)
        {
            await RespondAsync($"Failed to join the voice channel: {e.Message}");
        }
    }
}