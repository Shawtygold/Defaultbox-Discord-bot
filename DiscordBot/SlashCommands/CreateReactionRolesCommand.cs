using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class CreateReactionRolesCommand : ApplicationCommandModule
    {
        #region [Fields] 

        private ulong messageId = 0;
        private long uniqueRole = 0;
        private DiscordMessage message;
        private List<DiscordEmoji> optionEmojis;
        private List<DiscordRole> optionRoles;
        private List<ulong> usersWithRole;

        #endregion

        #region [Constructor]

        public CreateReactionRolesCommand()
        {
            optionEmojis = new();
            optionRoles = new();
            usersWithRole = new();
        }

        #endregion

        #region [Reaction Roles]
        
        [SlashCommand("create_reaction_roles", "Сreates a role addition when clicking on a reaction in a message")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task CreateReactionRoles(InteractionContext ctx,
            [Option("Message_Id", "Id of the message you want to attach a reaction to")] string messageId,
            [Choice("True", 0)]
            [Choice("False", 1)]
            [Option("Unique_role", "Only one role can be selected")] long uniqueRole,
            [Option("Emoji", "Emoji")] DiscordEmoji emoji,
            [Option("Role", "Role")] DiscordRole role,
            [Option("Emoji2", "Emoji")] DiscordEmoji? emoji2 = null,
            [Option("Role2", "Role")] DiscordRole? role2 = null,
            [Option("Emoji3", "Emoji")] DiscordEmoji? emoji3 = null,
            [Option("Role3", "Role")] DiscordRole? role3 = null,
            [Option("Emoji4", "Emoji")] DiscordEmoji? emoji4 = null,
            [Option("Role4", "Role")] DiscordRole? role4 = null,
            [Option("Emoji5", "Emoji")] DiscordEmoji? emoji5 = null,
            [Option("Role5", "Role")] DiscordRole? role5 = null)
        {
            // бот думает...
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            this.uniqueRole = uniqueRole;

            optionEmojis = new();
            optionRoles = new();
            usersWithRole = new();

            DiscordMessage message = await ctx.Channel.GetMessageAsync((ulong)Convert.ToInt64(messageId));

            if (message == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Message not found!"));
                return;
            }

            this.message = message;
            this.messageId = message.Id;

            var task1 = AddReactionAsync(emoji, role, message);
            var task2 = AddReactionAsync(emoji2, role2, message);
            var task3 = AddReactionAsync(emoji3, role3, message);
            var task4 = AddReactionAsync(emoji4, role4, message);
            var task5 = AddReactionAsync(emoji5, role5, message);

            await task1;
            await task2;
            await task3;
            await task4;
            await task5;

            ctx.Client.MessageReactionAdded += Client_MessageReactionAdded; 
            ctx.Client.MessageReactionRemoved += Client_MessageReactionRemoved;      

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Сompleted!"));
        }

        #region [Events]

        private Task Client_MessageReactionRemoved(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionRemoveEventArgs args)
        {
            if (args.Message.Id != messageId) return Task.CompletedTask;

            DiscordMember user = (DiscordMember)args.User;

            if (user.IsBot == true) return Task.CompletedTask;

            //if (optionEmojis.Any(e => e == args.Emoji) == false) return Task.CompletedTask;
            if (!optionEmojis.Contains(args.Emoji)) return Task.CompletedTask;

            RevokeRoleFromUser(user, args.Emoji);

            return Task.CompletedTask;
        }

        private Task Client_MessageReactionAdded(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionAddEventArgs args)
        {
            if (args.Message.Id != messageId) return Task.CompletedTask;

            DiscordMember user = (DiscordMember)args.User;

            if (user.IsBot == true) return Task.CompletedTask;

            if (!optionEmojis.Contains(args.Emoji)) return Task.CompletedTask;

            // if uniqueRole == true
            if (uniqueRole == 0)
            {
                // если реакция и роль есть, то удаляю старую реакцию и роль и добавляю новую реакцию и роль
                if (usersWithRole.Contains(user.Id))
                {
                    for (int i = 0; i < optionEmojis.Count; i++)
                    {
                        if (optionEmojis[i] != args.Emoji)
                        {
                            // удаляем предыдущую реакцию пользователя на сообщении
                            message.DeleteReactionAsync(optionEmojis[i], user);

                            // после удаления реакции пользователя сработает событие MessageReactionRemoved                                
                        }
                    }

                    GrantRoleToUser(user, args.Emoji);
                }
                else
                {
                    GrantRoleToUser(user, args.Emoji);
                }
            }
            else
            {
                GrantRoleToUser(user, args.Emoji);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region [Methods]

        private async Task AddReactionAsync(DiscordEmoji? emoji, DiscordRole? role, DiscordMessage message)
        {
            if (emoji != null && role != null)
            {
                optionEmojis.Add(emoji);
                optionRoles.Add(role);

                await message.CreateReactionAsync(emoji);
            }
        }

        private void GrantRoleToUser(DiscordMember user, DiscordEmoji userEmoji)
        {
            for (int i = 0; i < optionEmojis.Count; i++)
            {
                if (userEmoji == optionEmojis[i])
                {
                    user.GrantRoleAsync(optionRoles[i]);
                    usersWithRole.Add(user.Id);
                    return;
                }
            }
        }

        private void RevokeRoleFromUser(DiscordMember user, DiscordEmoji userEmoji)
        {
            for (int i = 0; i < optionEmojis.Count; i++)
            {
                if (optionEmojis[i] == userEmoji)
                {
                    user.RevokeRoleAsync(optionRoles[i]).Wait();
                    usersWithRole.Remove(user.Id);
                    return;
                }
            }
        }

        #endregion

        #endregion
    }
}
