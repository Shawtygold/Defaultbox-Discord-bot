using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Exceptions;

namespace DiscordBot.SlashCommands
{
    internal class RoleAddCommand : ApplicationCommandModule
    {
        #region [Role Add]

        [SlashCommand("Role_add", "Add role to user")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public static async Task AddRole(InteractionContext ctx, [Option("User", "User to be assigned the role")] DiscordUser user, [Option("Role", "Role")] DiscordRole role)
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
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't add a role to it."
                }));

                return;
            }

            if (member.Roles.Contains(role))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "The user already has this role!"
                }));

                return;
            }

            try
            {
                await member.GrantRoleAsync(role);
            }
            catch (UnauthorizedException)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to add the role **{member.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to add a role to this user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"Added {role.Mention} to **{member.Username}**"
            }));
        }

        #endregion
    }
}
