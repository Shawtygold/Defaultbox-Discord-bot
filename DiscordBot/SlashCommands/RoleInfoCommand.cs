using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using System.Data;

namespace DiscordBot.SlashCommands
{
    internal class RoleInfoCommand : ApplicationCommandModule
    {
        #region [Role Info]

        [SlashCommand("role_info", "Check info for a role ")]
        public static async Task RoleInfo(InteractionContext ctx, [Option("Role", "Role")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            int countMembersWithRole = 0;

            IEnumerable<DiscordMember> members;

            try
            {
                members = await ctx.Guild.GetAllMembersAsync();
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to get the members of this server!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            foreach (var member in members)
            {
                if (member.Roles.Any(r => r.Name == role.Name))
                    countMembersWithRole++;
            }

            var result = DateTimeOffset.Now - role.CreationTimestamp;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Title = "Role info",
                Color = role.Color,
                Description = $"Name: {role.Name}\nMembers: {countMembersWithRole}\nColor: {role.Color}\nCreated: {(int)result.TotalDays} days ago",
                Footer = new() { Text = $"Id: {role.Id}" }
            }));
        }

        #endregion
    }
}
