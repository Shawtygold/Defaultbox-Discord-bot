using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DiscordBot.SlashCommands
{
    internal class CoinCommand : ApplicationCommandModule
    {
        [SlashCommand("coin", "Flip a coin.")]
        public static async Task Coin(InteractionContext ctx)
        {
            string[] sidesСoin = new[] { "Head", "Tail" };
            Random rnd = new();

            await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"You got **{sidesСoin[rnd.Next(0, sidesСoin.Length)]}**"
            });
        }
    }
}
