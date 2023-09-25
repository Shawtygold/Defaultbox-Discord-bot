using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;

namespace DiscordBot.SlashCommands
{
    internal class CreateReactionRolesCommand : ApplicationCommandModule
    {
        #region [Fields] 

        private ulong messageId = 0;
        private bool uniqueRole;
        private DiscordMessage message;
        private List<DiscordEmoji> optionEmojis;
        private List<DiscordRole> optionRoles;
        private List<ulong> usersWithRole;
        private InteractionContext ctx;

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
        
        [SlashCommand("create_reaction_roles", "Сreates a role addition when clicking on a reaction in a message.")]
        public async Task CreateReactionRoles(InteractionContext ctx,
            [Option("message_Id", "Id of the message you want to attach a reaction to.")] string messageId,
            [Choice("True", "True")]
            [Choice("False", "False")]
            [Option("unique_role", "Only one role can be selected.")] string uniqueRole,
            [Option("emoji", "Emoji")] DiscordEmoji emoji,
            [Option("role", "Role")] DiscordRole role,
            [Option("emoji2", "Emoji")] DiscordEmoji? emoji2 = null,
            [Option("role2", "Role")] DiscordRole? role2 = null,
            [Option("emoji3", "Emoji")] DiscordEmoji? emoji3 = null,
            [Option("role3", "Role")] DiscordRole? role3 = null,
            [Option("emoji4", "Emoji")] DiscordEmoji? emoji4 = null,
            [Option("role4", "Role")] DiscordRole? role4 = null,
            [Option("emoji5", "Emoji")] DiscordEmoji? emoji5 = null,
            [Option("role5", "Role")] DiscordRole? role5 = null)
        {
            if (!PermissionsManager.CheckPermissionsIn(ctx.Member, ctx.Channel, new() { Permissions.Administrator }) && !ctx.Member.IsOwner)
            {
                await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Insufficient permissions. You need **Administrator** permission for this command."
                },true);
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
                    Description = "I don't have access to this channel! Please, check the permissions."
                }));
                return;
            }

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.AddReactions, Permissions.ManageRoles, Permissions.ManageMessages, Permissions.ReadMessageHistory }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Maybe I'm not allowed to add reactions, manage roles, manage messages or read message history. Please check the permissions."
                }));
                return;
            }

            this.ctx = ctx;
            this.uniqueRole = Convert.ToBoolean(uniqueRole);

            optionEmojis = new();
            optionRoles = new();
            usersWithRole = new();

            DiscordMessage message;
            try
            {
                message = await ctx.Channel.GetMessageAsync(Convert.ToUInt64(messageId));
            }
            catch(UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. Maybe I'm not allowed to read message history. Please check the permissions."
                }));
                return;
            }
            catch(NotFoundException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong while trying to get a message by id. Please check the message Id."
                }));
                return;
            }
            catch(Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to get a message by id.\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
                return;
            }

            this.message = message;
            this.messageId = message.Id;

            await AddReactionAsync(emoji, role, message);
            await AddReactionAsync(emoji2, role2, message);
            await AddReactionAsync(emoji3, role3, message);
            await AddReactionAsync(emoji4, role4, message);
            await AddReactionAsync(emoji5, role5, message);

            ctx.Client.MessageReactionAdded += Client_MessageReactionAdded; 
            ctx.Client.MessageReactionRemoved += Client_MessageReactionRemoved;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Сompleted"
            }));
        }

        #region [Events]

        private async Task Client_MessageReactionRemoved(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionRemoveEventArgs args)
        {
            if (args.Message.Id != messageId) return;

            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(args.User.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like this user is on the server."
                }));

                return;
            }

            if (member.IsBot == true) return;

            if (!optionEmojis.Contains(args.Emoji)) return;

            await RevokeRoleFromUserAsync(member, args.Emoji);

            return;
        }

        private async Task Client_MessageReactionAdded(DiscordClient sender, DSharpPlus.EventArgs.MessageReactionAddEventArgs args)
        {
            if (args.Message.Id != messageId) return;

            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(args.User.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like this user is on the server."
                }));

                return;
            }

            if (member.IsBot == true) return;

            if (!optionEmojis.Contains(args.Emoji)) return;

            if (uniqueRole == true)
            {
                // если роль есть, то добавляю новую роль и удаляю старую
                if (usersWithRole.Contains(member.Id))
                {
                    await GrantRoleToUserAsync(member, args.Emoji);

                    for (int i = 0; i < optionEmojis.Count; i++)
                    {
                        if (optionEmojis[i] != args.Emoji)
                        {
                            try
                            {
                                // удаляем предыдущую реакцию пользователя на сообщении
                                await message.DeleteReactionAsync(optionEmojis[i], member);
                            }
                            catch (UnauthorizedException)
                            {
                                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                                {
                                    Title = "An error occurred",
                                    Color = DiscordColor.Red,
                                    Description = $"Something went wrong. I may not be allowed to delete reactions! Please check the permissions."
                                }));
                                return;
                            }
                            catch(NotFoundException)
                            {
                                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                                {
                                    Title = "An error occurred",
                                    Color = DiscordColor.Red,
                                    Description = $"Hmm, something went wrong when trying to delete the reaction from the message! Message not found!"
                                }));
                                return;
                            }
                            catch (Exception ex)
                            {
                                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                                {
                                    Title = "An error occurred",
                                    Color = DiscordColor.Red,
                                    Description = $"Hmm, something went wrong when trying to delete the reaction from the message!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                                }));
                                return;
                            }                              
                        }
                    }                   
                }
                else
                {
                    await GrantRoleToUserAsync(member, args.Emoji);
                }
            }
            else
            {
                await GrantRoleToUserAsync(member, args.Emoji);
            }

            return;
        }

        #endregion

        #region [Methods]

        private async Task AddReactionAsync(DiscordEmoji? emoji, DiscordRole? role, DiscordMessage message)
        {
            if (emoji == null || role == null) return;

            optionEmojis.Add(emoji);
            optionRoles.Add(role);

            try
            {
                await message.CreateReactionAsync(emoji);
                return;
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to add reactions! Please check the permissions."
                }));
                return;
            }
            catch (NotFoundException)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong when trying to add a reaction to a message! Message not found!"
                }));
                return;
            }
            catch (Exception ex)
            { 
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong when trying to add a reaction to a message!!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
                return;   
            }
        }

        private async Task GrantRoleToUserAsync(DiscordMember member, DiscordEmoji userEmoji)
        {
            for (int i = 0; i < optionEmojis.Count; i++)
            {
                if (optionEmojis[i] == userEmoji)
                {
                    if(member.Roles.Contains(optionRoles[i]))
                        return;

                    try
                    {
                        await member.GrantRoleAsync(optionRoles[i]);
                        usersWithRole.Add(member.Id);
                        return;
                    }
                    catch(UnauthorizedException)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                        {
                            Title = "An error occurred",
                            Color = DiscordColor.Red,
                            Description = $"Something went wrong. I may not be allowed to add a role **{member.Username}**! Please check the permissions."
                        });                       
                        return;
                    }
                    catch (NotFoundException)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                        {
                            Title = "An error occurred",
                            Color = DiscordColor.Red,
                            Description = $"Something went wrong while trying to add a role to this user. Role not found!"
                        });
                        return;
                    }
                    catch (Exception ex)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                        {
                            Title = "An error occurred",
                            Color = DiscordColor.Red,
                            Description = $"Hmm, something went wrong while trying to add a role to this user!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                        });
                        return;
                    }
                }
            }
        }

        private async Task RevokeRoleFromUserAsync(DiscordMember member, DiscordEmoji userEmoji)
        {
            for (int i = 0; i < optionEmojis.Count; i++)
            {
                if (optionEmojis[i] == userEmoji)
                {
                    if (!member.Roles.Contains(optionRoles[i]))
                        return;

                    try
                    {
                        await member.RevokeRoleAsync(optionRoles[i]);
                        usersWithRole.Remove(member.Id);
                        return;
                    }
                    catch (UnauthorizedException)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                        {
                            Title = "An error occurred",
                            Color = DiscordColor.Red,
                            Description = $"Something went wrong. I may not be allowed to remove the role from **{member.Username}**! Please check the permissions."
                        });                   
                        return;
                    }
                    catch(NotFoundException)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                        {
                            Title = "An error occurred",
                            Color = DiscordColor.Red,
                            Description = $"Something went wrong while trying to remove the role from this user. Role not found!"
                        });
                        return;
                    }
                    catch (Exception ex)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                        {
                            Title = "An error occurred",
                            Color = DiscordColor.Red,
                            Description = $"Hmm, something went wrong while trying to remove the role from this user!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                        });
                        return;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
