using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class UnbanCommand : ApplicationCommandModule
    {
        #region [Unban]

        [SlashCommand("unban", "Unban a user.")]
        [RequirePermissions(Permissions.BanMembers)]
        public static async Task Unban(InteractionContext ctx,
            [Option("user", "The user to unban.")] DiscordUser userToUnban)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            try
            {
                await ctx.Guild.UnbanMemberAsync(userToUnban);
            }
            catch(UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to unban **{userToUnban.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch(Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to unban that user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"Successfully unbanned **{userToUnban.Username}**!"
            }));
        }

        #endregion
    }
}
