using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DiscordBot.SlashCommands
{
    internal class MemberInfoCommand : ApplicationCommandModule
    {
        #region [MemberInfo]

        [SlashCommand("members_info", "Shows some interesting information about a member")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task MemberInfo(InteractionContext ctx, [Option("user", "user")] DiscordUser user)
        {
            try
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                DiscordMember member = (DiscordMember)user;

                Optional<DiscordColor> color;

                if(member.BannerColor == null)
                {
                    color = DiscordColor.Blurple;
                }
                else
                {
                    color = (Optional<DiscordColor>)member.BannerColor;
                }

                Console.WriteLine(member.Nickname);
                string roles = "";

                IEnumerable<DiscordRole> discordRoles = member.Roles;
                discordRoles = discordRoles.Reverse();

                if (!discordRoles.Any())
                {
                    roles = "No roles";
                }
                else
                {
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

                await ctx.Client.SendMessageAsync(ctx.Channel, new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = color,
                    Title = member.Username,
                    ImageUrl = member.AvatarUrl,
                    Description = $"**Roles:** \n{roles}\n\n**Display name:** {member.DisplayName}\n\n**Created at:** {member.CreationTimestamp.LocalDateTime}\n\n**Joined at:** {member.JoinedAt.LocalDateTime}"

                }));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Success!"));
            }
            catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"{DateTime.Now} | {ex.Message}."); Console.ResetColor(); }
        }

        #endregion
    }
}
