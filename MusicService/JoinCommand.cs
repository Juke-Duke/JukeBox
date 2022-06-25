using System.Security.Cryptography.X509Certificates;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Player;

namespace JukeBox.MusicService;
public partial class MusicSlashCommands
{
    [SlashCommand("join", "Start the vibe session!")]
    public async Task JoinCommandAsync()
    {
        var userVoiceState = (Context.User as IVoiceState)!;

        if (userVoiceState.VoiceChannel is null)
        {
            await RespondAsync("Get in call first buddy");
            return;
        }

        if (_audioService.HasPlayer(Context.Guild.Id) && Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
        {
            await RespondAsync("Bro you vibing alone? Hop in the vibe session and make some friends");
            return;
        }

        try
        {
            var player = await _audioService.JoinAsync<QueuedLavalinkPlayer>(Context.Guild.Id, userVoiceState.VoiceChannel.Id, true);

            await RespondAsync($"Its litty in {userVoiceState.VoiceChannel.Name}");
        }
        catch (Exception e)
        {
            await RespondAsync($"Failed to join the voice channel: {e.Message}");
        }
    }
}