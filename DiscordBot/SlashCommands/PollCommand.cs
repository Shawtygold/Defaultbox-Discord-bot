using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System.Collections.ObjectModel;

namespace DiscordBot.SlashCommands
{
    internal class PollCommand : ApplicationCommandModule
    {
        #region [Poll]

        [SlashCommand("poll", "Create poll.")]
        public static async Task Poll(InteractionContext ctx, [Option("question", "Poll question.")] string title, [Option("answers", "Answer options.")] string answers,
            [Choice("5 мин", 5)]
            [Choice("15 мин", 15)]
            [Choice("30 мин", 30)]
            [Choice("1 час", 60)]
            [Choice("2 часа", 120)]
            [Choice("1 день", 1440)]
            [Option("polling_time", "Polling time.") ] long pollTime,
            [Option("separator", "Separator.")] string separator = "|")
        {
            if (!PermissionsManager.CheckPermissionsIn(ctx.Member, ctx.Channel, new() { Permissions.Administrator }) && !ctx.Member.IsOwner)
            {
                await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Insufficient permissions. You need **Administrator** permission for this command."
                });
                return;
            }

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

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.SendMessages, Permissions.AddReactions, Permissions.ReadMessageHistory, Permissions.ManageMessages }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Maybe I'm not allowed to ssend messages, add reactions, read message history or manage messages. Please check the permissions."
                }));
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();

            // разделение единой строки на варианты ответов по спец. символу
            List<string> options = answers.Split(separator).ToList();

            if (options.Count > 9)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Exception! The number of answer choices is exceeded! Max. number - 9"
                }));
                return;
            }

            DiscordEmoji[] optionEmojis =
            {
                DiscordEmoji.FromName(ctx.Client, ":one:"),
                DiscordEmoji.FromName(ctx.Client, ":two:"),
                DiscordEmoji.FromName(ctx.Client, ":three:"),
                DiscordEmoji.FromName(ctx.Client, ":four:"),
                DiscordEmoji.FromName(ctx.Client, ":five:"),
                DiscordEmoji.FromName(ctx.Client, ":six:"),
                DiscordEmoji.FromName(ctx.Client, ":seven:"),
                DiscordEmoji.FromName(ctx.Client, ":eight:"),
                DiscordEmoji.FromName(ctx.Client, ":nine:"),
            };

            string pollDescription = "";

            for (int i = 0; i < options.Count; i++)
            {
                pollDescription += $"{optionEmojis[i]} | {options[i]}\n";
            }

            DiscordMessage pollMessage;
            try
            {
                pollMessage = await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Description = pollDescription,
                    Title = title
                }));
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to send messages! Please check the permissions."
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to send poll message!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please include the following debugging information in the message:\n```{ex}\n```"
                }));
                Logger.Error(ex.ToString());
                return;
            }

            // список с числами, которые характеризуют количество голосов за определенный вариант ответа
            List<int> answerVotes = new();

            for (int i = 0; i < options.Count; i++)
            {
                await pollMessage.CreateReactionAsync(optionEmojis[i]);

                // заполнение списка нулями
                answerVotes.Add(0);
            }

            ReadOnlyCollection<Reaction> reactions = await interactivity.CollectReactionsAsync(pollMessage, TimeSpan.FromSeconds(pollTime));

            int totalVotes = 0;

            // подсчет количества голосов
            for (int i = 0; i < answerVotes.Count; i++)
            {
                for (int j = 0; j < reactions.Count; j++)
                {
                    if (optionEmojis[i] == reactions[j].Emoji)
                    {
                        answerVotes[i] += reactions[j].Total;

                        totalVotes += reactions[j].Total;
                    }
                }
            }

            // нахождение варианта, который победил в опросе
            int maxValue = answerVotes.Max();
            int index = answerVotes.IndexOf(maxValue);
            
            string resultDescription = "";

            for (int i = 0; i < options.Count; i++)
            {
                resultDescription += $"{optionEmojis[i]} | {options[i]} : {answerVotes[i]} Votes\n";
            }

            resultDescription += $"\nResult: **{options[index]}**";

            var resultMessage = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Title = title,
                Color = DiscordColor.Green,
                Description = resultDescription,
                Footer = new() { Text = $"Total votes: {totalVotes}" }
            });

            try
            {
                await ctx.Channel.DeleteMessageAsync(pollMessage);
            }
            catch(UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to manage messages! Please check the permissions."
                }));

                return;
            }
            catch(NotFoundException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. Poll message not found!"
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong when trying to delete a poll message!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please include the following debugging information in the message:\n```{ex}\n```"
                }));
                Logger.Error(ex.ToString());
                return;
            }

            try
            {
                await ctx.Channel.SendMessageAsync(resultMessage);
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to send messages! Please check the permissions."
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to send poll message!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please include the following debugging information in the message:\n```{ex}\n```"
                }));
                Logger.Error(ex.ToString());
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Poll completed!"));
        }

        #endregion
    }
}
