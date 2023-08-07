using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class RemoveRoleCommand : ApplicationCommandModule
    {
        [SlashCommand("remove_role", "Remove role from user")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task RemoveRole(InteractionContext ctx, [Option("user", "user to remove role")] DiscordUser user, [Option("role", "role")] DiscordRole role)
        {
            DiscordMember member = (DiscordMember)user;

            if(user == null)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Member not found!"));
                return;
            }

            if (!member.Roles.Contains(role))
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("User does not have this role!"));
                return;
            }

            await member.RevokeRoleAsync(role);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"{member.Username} is no longer \"{role.Name}\""));
        }
    }
}
