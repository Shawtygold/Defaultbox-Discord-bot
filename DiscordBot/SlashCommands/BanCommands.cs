using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.SlashCommands
{
    internal class BanCommands : ApplicationCommandModule
    {
        #region [Ban]

        [SlashCommand("ban", "Ban a user.")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public static async Task Ban(InteractionContext ctx,
            [Option("user", "The user to ban.")] DiscordUser user,
            [Option("reason", "The reason for the ban.")][MaximumLength(1500)] string reason = "No reason provided.",
            [Choice("1 День", 1)]
            [Choice("1 Неделя", 7)]
            [Option("deletedays", "Number of days of message history to delete.")] long deleteDays = 0)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't ban him."
                }));

                return;
            }

            try
            {
                await ctx.Guild.BanMemberAsync(member, (int)deleteDays, reason);
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to ban **{member.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to ban that user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"**{user.Username}** has been banned from this server. Reason: {reason}"
            }));
        }

        #endregion

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
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to unban **{userToUnban.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
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
