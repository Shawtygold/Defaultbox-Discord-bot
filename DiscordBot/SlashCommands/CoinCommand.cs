using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.SlashCommands
{
    internal class CoinCommand : ApplicationCommandModule
    {
        [SlashCommand("coin", "Flip a coin.")]
        public static async Task Coin(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            DiscordMember bot;
            try
            {
                bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Could not find myself on the server. Please try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
                return;
            }

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.AccessChannels }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "I don't have access to this channel! Please check the permissions."
                }));
                return;
            }

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.SendMessages }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Maybe I'm not allowed to send messages. Please check the permissions."
                }));
                return;
            }

            string[] sidesСoin = new[] { "Head", "Tail" };
            Random rnd = new();

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"You got **{sidesСoin[rnd.Next(0, sidesСoin.Length)]}**"
            }));
        }
    }
}
