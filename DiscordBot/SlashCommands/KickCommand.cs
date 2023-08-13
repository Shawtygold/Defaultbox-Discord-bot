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
    internal class KickCommand : ApplicationCommandModule
    {
        #region [Kick]

        [SlashCommand("kick", "Kick user from this server.")]
        [SlashRequirePermissions(Permissions.KickMembers)]
        public static async Task Kick(InteractionContext ctx,
            [Option("user", "The user to kick.")] DiscordUser user,
            [Option("reason", "The reason for the kick.")][MaximumLength(1500)] string reason = "No reason provided.")
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
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't kick him."
                }));

                return;
            }

            try
            {
                await member.RemoveAsync();
            }
            catch(UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to kick **{member.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to kick that user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder() 
            { 
                Color = DiscordColor.Green,
                Description = $"**{member.Username}** has been kicked from this server. Reason: {reason}"
            }));
        }

        #endregion
    }
}
