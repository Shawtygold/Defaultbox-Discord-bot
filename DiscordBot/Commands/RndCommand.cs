using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    internal class RndCommand : BaseCommandModule
    {

        [Command("random")]
        public async Task RandomCommand(CommandContext ctx, int from, int to)
        {
            var random = new Random();
            await ctx.Channel.SendMessageAsync($"Рандомное число: {random.Next(from, to)}");
        }
    }
}
