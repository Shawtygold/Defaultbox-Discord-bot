using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DiscordBot.SlashCommands
{
    internal class BanCommand : ApplicationCommandModule
    {
        #region [Ban]

        [SlashCommand("ban", "Bans a user")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Ban(InteractionContext ctx, [Option("User", "User to ban")] DiscordUser user, [Option("Reason", "Reason for banning a user from this server")] string reason,
            [Choice("1 День", 1)]
            [Choice("1 Неделя", 7)]
            [Option("deletedays", "Number of days of message history to delete")] long deleteDays = 0)
        {
            try
            {
                await ctx.Guild.BanMemberAsync(user.Id, (int)deleteDays);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{user.Username} has been banned from this server. Reason: {reason}"));
            }
            catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"{DateTime.Now} | {ex.Message}."); Console.ResetColor(); }
        }

        #endregion

    }
}
