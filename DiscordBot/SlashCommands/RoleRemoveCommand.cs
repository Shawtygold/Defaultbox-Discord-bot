using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Exceptions;

namespace DiscordBot.SlashCommands
{
    internal class RoleRemoveCommand : ApplicationCommandModule
    {
        #region [Role Remove]

        [SlashCommand("Role_remove", "Remove role from user")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public static async Task RemoveRole(InteractionContext ctx, [Option("User", "User to remove role")] DiscordUser user, [Option("Role", "Role")] DiscordRole role)
        {
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
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't remove the role from him."
                }));

                return;
            }

            if (!member.Roles.Contains(role))
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "User does not have this role!"
                }));

                return;
            }

            try
            {
                await member.RevokeRoleAsync(role);
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to remove role from **{member.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to remove a role from this user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"Removed {role.Mention} from **{member.Username}**"
            }));
        }

        #endregion
    }
}
