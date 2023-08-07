using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class KickCommand : ApplicationCommandModule
    {
        #region [Kick]

        [SlashCommand("kick", "Kick user from this server")]
        [SlashRequirePermissions(Permissions.KickMembers)]
        public async Task Kick(InteractionContext ctx, [Option("User", "user to kick")] DiscordUser user, [Option("Reason", "Reason for removing a user from this server")] string reason)
        {
            try
            {
                DiscordMember member = (DiscordMember)user;

                if (member == null)
                {
                    await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Member not found!"));
                    return;
                }

                await member.RemoveAsync();

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{member.Username} has been kicked from this server. Reason: {reason}"));
            }
            catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"{DateTime.Now} | {ex.Message}."); Console.ResetColor(); }
        }

        #endregion
    }
}
