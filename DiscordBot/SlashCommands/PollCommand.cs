using DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Collections.ObjectModel;

namespace DiscordBot.SlashCommands
{
    internal class PollCommand : ApplicationCommandModule
    {
        #region [Poll]

        [SlashCommand("poll", "Create poll.")]
        public static async Task Poll(InteractionContext ctx,
            [Option("question", "Poll question.")] string question, 
            [Choice("5 мин", 5)]
            [Choice("15 мин", 15)]
            [Choice("30 мин", 30)]
            [Choice("1 час", 60)]
            [Choice("2 часа", 120)]
            [Choice("1 день", 1440)]
            [Option("polling_time", "Polling time.") ] long pollTime,
            [Option("choice_a", "answer option1")] string? choiceA = null,
            [Option("choice_b", "answer option2")] string? choiceB = null,
            [Option("choice_c", "answer option3")] string? choiceC = null,
            [Option("choice_d", "answer option4")] string? choiceD = null,
            [Option("choice_e", "answer option5")] string? choiceE = null,
            [Option("choice_f", "answer option6")] string? choiceF = null,
            [Option("choice_g", "answer option7")] string? choiceG = null,
            [Option("choice_h", "answer option8")] string? choiceH = null,
            [Option("choice_i", "answer option9")] string? choiceI = null,
            [Option("choice_j", "answer option10")] string? choiceJ = null,
            [Option("choice_k", "answer option11")] string? choiceK = null,
            [Option("choice_l", "answer option12")] string? choiceL = null,
            [Option("choice_m", "answer option13")] string? choiceM = null,
            [Option("choice_n", "answer option14")] string? choiceN = null,
            [Option("choice_o", "answer option15")] string? choiceO = null,
            [Option("choice_p", "answer option16")] string? choiceP = null,
            [Option("choice_q", "answer option17")] string? choiceQ = null,
            [Option("choice_r", "answer_option18")] string? choiceR = null,
            [Option("choice_s", "answer_option19")] string? choiceS = null,
            [Option("choice_t", "answer_option20")] string? choiceT = null,
            [Option("choice_u", "answer_option21")] string? choiceU = null,
            [Option("choice_v", "answer_option22")] string? choiceV = null,
            [Option("choice_w", "answer_option23")] string? choiceW = null
            )
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
                    Description = "I don't have access to this channel! Please, check the permissions."
                }));
                return;
            }

            if (!PermissionsManager.CheckPermissionsIn(bot, ctx.Channel, new() { Permissions.SendMessages, Permissions.AddReactions, Permissions.ReadMessageHistory, Permissions.ManageMessages, Permissions.EmbedLinks }))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = "Maybe I'm not allowed to ssend messages, add reactions, read message history or manage messages. Please check the permissions."
                }));
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();

            List<string> notNullChoices = new();
            List<string?> choices = new() { choiceA, choiceB, choiceC, choiceD, choiceE, choiceF, choiceG, choiceH, choiceI, choiceJ, choiceK, choiceL, choiceM, choiceN, choiceO, choiceP, choiceQ, choiceR, choiceS, choiceT, choiceU, choiceV, choiceW, /*choiceX,*/ /*choiceY, choiceZ*/ };
            
            for (int i = 0; i < choices.Count; i++)
            {
                if (choices[i] != null)
                {
                    notNullChoices.Add(choices[i]);
                }
            }

            DiscordEmoji[] optionEmojis =
            {
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_a:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_b:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_c:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_d:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_e:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_f:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_g:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_h:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_i:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_j:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_k:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_l:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_m:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_n:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_o:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_p:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_q:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_r:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_s:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_t:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_u:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_v:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_w:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_x:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_y:"),
                DiscordEmoji.FromName(ctx.Client, ":regional_indicator_z:")
            };

            string pollDescription = "";

            for (int i = 0; i < notNullChoices.Count; i++)
            {
                pollDescription += $"{optionEmojis[i]} | {notNullChoices[i]}\n";
            }

            DiscordMessage pollMessage;
            try
            {
                pollMessage = await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Description = pollDescription,
                    Title = question
                }));
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to send messages or embed links! Please check the permissions."
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to send poll message!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
                return;
            }

            // список с числами, которые характеризуют количество голосов за определенный вариант ответа
            List<int> answerVotes = new();

            for (int i = 0; i < notNullChoices.Count; i++)
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

            for (int i = 0; i < notNullChoices.Count; i++)
            {
                resultDescription += $"{optionEmojis[i]} | {notNullChoices[i]} : {answerVotes[i]} Votes\n";
            }

            resultDescription += $"\nResult: **{notNullChoices[index]}**";

            var resultMessage = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Title = question,
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
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to manage messages! Please check the permissions."
                }));

                return;
            }
            catch(NotFoundException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. Poll message not found!"
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong when trying to delete a poll message!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
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
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. I may not be allowed to send messages! Please check the permissions."
                }));
                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Title = "An error occurred",
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to send poll message!\n\nThis was Discord's response:\n> {ex.Message}\n\nPlease try again or contact [support team](https://t.me/Shawtygoldq)."
                }));
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Poll completed!"));
        }

        #endregion
    }
}
