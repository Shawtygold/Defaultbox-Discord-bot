using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class TimeoutCommand : ApplicationCommandModule
    {
        [SlashCommand("timeout", "Send the user to think about their behavior")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task Timeout(InteractionContext ctx, [Option("user", "user to timeout")] DiscordUser user, 
            [Choice("60 seconds", 0)]
            [Choice("5 minutes", 1)]
            [Choice("10 minutes", 2)]
            [Choice("30 minutes", 3)]
            [Choice("1 hour", 4)]
            [Choice("1 day", 5)] [Option("time", "time to timeout")] long time, [Option("Reason", "Timeout reason")] string reason)
        {
            DiscordMember member = await ctx.Guild.GetMemberAsync(user.Id);

            if(member == null)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Member not found!"));
                return;
            }

            List<DateTimeOffset> dateTimeOffsets = new() {
                DateTimeOffset.Now.AddMinutes(1),
                DateTimeOffset.Now.AddMinutes(5),
                DateTimeOffset.Now.AddMinutes(10),
                DateTimeOffset.Now.AddMinutes(30),
                DateTimeOffset.Now.AddHours(1),
                DateTimeOffset.Now.AddDays(1) 
            };

            List<string> times = new()
            {
                "1 minutes",
                "5 minutes",
                "10 minutes",
                "30 minutes",
                "1 hour",
                "1 day"
            };
            
            await member.TimeoutAsync(dateTimeOffsets[(int)time], reason);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"{member.Username} sent to think about their behavior for {times[(int)time]}! Reason: {reason}"));
        }
    }
}
