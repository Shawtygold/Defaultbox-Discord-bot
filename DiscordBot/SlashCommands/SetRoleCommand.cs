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
    internal class SetRoleCommand : ApplicationCommandModule
    {
        #region [SetRole]

        [SlashCommand("Set_role", "Set role to user")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task SetRole(InteractionContext ctx, [Option("user", "User to be assigned the role")] DiscordUser user, [Option("role", "role5")] DiscordRole role)
        {
            DiscordMember member = (DiscordMember)user;

            if(member == null)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Member not found"));
                return;
            }

            if (member.Roles.Contains(role))
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("The user already has this role!"));
                return;
            }

            await member.GrantRoleAsync(role);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"{member.Username} is now \"{role.Name}\""));
        }

        #endregion
    }
}
