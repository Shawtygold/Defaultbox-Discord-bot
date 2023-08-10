using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class RoleCreateCommand : ApplicationCommandModule
    {

        /// <summary>
        /// Временно эта команда не будет регистрироваться в боте по причине ненужности этой команды.
        /// </summary>

        #region [Role Create]

        [SlashCommand("role_create", "Creates a role on this server")]
        [RequirePermissions(Permissions.Administrator)]
        public static async Task CreateRole(InteractionContext ctx, 
            [Option("Name", "Role name")] string roleName, 
            [Option("HEXColor", "Role color")] string? color = null, 
            [Choice("True", "True")]
            [Choice("False", "False")]
            [Option("Hoist", "Hoist")]string? hoist = null,
            [Choice("True", "True")]
            [Choice("True", "False")]
            [Option("Mentionable", "Mentionable")]bool? mentionable = null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if(color != null)
                if (!color.StartsWith("#") && color.Length != 7)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Description = "Wrong color specified! Use HEX code for color transfer, for example: #000000"
                    }));
                    return;
                }

            DiscordColor discordColor = new(color);
            
            DiscordRole role;
            try
            {
                role = await ctx.Guild.CreateRoleAsync(roleName, Permissions.None, discordColor, Convert.ToBoolean(hoist), Convert.ToBoolean(mentionable));
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I are not be allowed to create roles! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to create a role!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"Role {role.Mention} has been created!"
            }));
        }

        #endregion
    }
}
