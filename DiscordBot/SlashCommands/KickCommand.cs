using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;

namespace DiscordBot.SlashCommands
{
    internal class KickCommand : ApplicationCommandModule
    {
        #region [Kick]

        [SlashCommand("kick", "Kick user from this server.")]
        public static async Task Kick(InteractionContext ctx,
            [Option("user", "The user to kick.")] DiscordUser user,
            [Option("reason", "The reason for the kick.")][MaximumLength(1500)] string reason = "No reason provided.")
        {
            if (!PermissionsManager.CheckPermissionsIn(ctx.Member, ctx.Channel, new() { Permissions.Administrator }) && !ctx.Member.IsOwner)
            {
                await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Insufficient permissions. You need **Administrator** permission for this command."
                }, true);
                return;
            }

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

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.KickMembers }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Maybe I'm not allowed to kick members. Please check the permissions."
                }));
                return;
            }

            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
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
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to kick **{member.Username}**! Please check the role hierarchy and permissions."
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to kick that user!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder() 
            { 
                Title = "Complete",
                Color = DiscordColor.Green,
                Description = $"**{member.Username}** has been kicked from this server. Reason: {reason}"
            }));
        }

        #endregion
    }
}
