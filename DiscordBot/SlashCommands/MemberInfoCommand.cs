using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;

namespace DiscordBot.SlashCommands
{
    internal class MemberInfoCommand : ApplicationCommandModule
    {
        #region [MemberInfo]

        [SlashCommand("member_info", "Shows some interesting information about a member.")]
        public static async Task MemberInfo(InteractionContext ctx, [Option("user", "User information about which you want to get.")] DiscordUser user)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            DiscordMember bot;
            try
            {
                bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
            }
            catch (ServerErrorException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Server Error Exception. Please, try again or contact the developer."
                }));
                return;
            }

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.AccessChannels }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "I don't have access to this channel! Please, check the permissions."
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
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't get information about him."
                }));
                return;
            }

            Optional<DiscordColor> color;

            if(member.BannerColor == null)
                color = DiscordColor.Blurple;
            else
                color = (Optional<DiscordColor>)member.BannerColor;

            string roles = "";

            IEnumerable<DiscordRole> discordRoles = member.Roles;

            if (!discordRoles.Any())
                    roles = "No roles";
            else
            {
                discordRoles = discordRoles.Reverse();

                int index = 0;
                foreach (var role in discordRoles)
                {
                    if (index % 2 == 0 && index != 0)
                    {
                        roles += "\n";
                    }
                    roles += $" {role.Mention}";
                    index++;
                }
            }

            var embed = new DiscordEmbedBuilder()
            {
                Color = color,
                Title = "Member info",
                ImageUrl = member.AvatarUrl,
                Footer = new() { Text = $"Id: {member.Id}" }
            };

            embed.AddField("Name", $"{member.Username}");
            embed.AddField("Roles", $"{roles}");
            embed.AddField("Created at", $"{member.CreationTimestamp.LocalDateTime}");
            embed.AddField("Joined At", $"{member.JoinedAt.LocalDateTime}");

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }

        #endregion
    }
}
