using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
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
    internal class PollCommand : ApplicationCommandModule
    {
        #region [Poll]

        [SlashCommand("poll", "Create poll")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task Poll(InteractionContext ctx, [Option("Question", "Poll question")] string title, [Option("Answers", "Answer options")] string answers,
            [Choice("5 мин", 5)]
            [Choice("15 мин", 15)]
            [Choice("30 мин", 30)]
            [Choice("1 час", 60)]
            [Choice("2 часа", 120)]
            [Choice("1 день", 1440)]
            [Option("Polling_time", "Polling time") ] long pollTime,

            [Choice("|", "|")]
            [Choice(";", ";")]
            [Option("Separator", "Separator")] string separator = "|")
        {
            try
            {
                Console.WriteLine($"{DateTime.Now} | {ctx.Client.CurrentUser.Username} создает опрос: \"{title}\".");

                // бот думает...
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                var interactivity = ctx.Client.GetInteractivity();

                // разделение единой строки на варианты ответов по спец. символу ";"
                List<string> options = answers.Split(separator).ToList();

                if (options.Count > 9)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Exception! The number of answer choices is exceeded! Max. number - 9"));
                    return;
                }

                // все возможные вариации эмоджи
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

                // сообщение опроса
                var pollMessage = await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Description = pollDescription,
                    Title = string.Join(" ", title)
                }));

                // список с числами, которые характеризуют количество голосов за определенный вариант ответа
                List<int> answerVotes = new();

                for (int i = 0; i < options.Count; i++)
                {
                    await pollMessage.CreateReactionAsync(optionEmojis[i]);

                    // заполнение списка нулями
                    answerVotes.Add(0);
                }

                var reactions = await interactivity.CollectReactionsAsync(pollMessage, TimeSpan.FromSeconds(pollTime));

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

                string resultDescription = "";

                for (int i = 0; i < options.Count; i++)
                {
                    resultDescription += $"{optionEmojis[i]} | {options[i]} : {answerVotes[i]} Votes\n";
                }

                resultDescription += $"\n**Total votes: {totalVotes}**";

                // result сообщение
                var resultMessage = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Green,
                    Description = resultDescription,
                    Title = string.Join(" ", title)
                });

                await ctx.Channel.DeleteMessageAsync(pollMessage);
                await ctx.Channel.SendMessageAsync(resultMessage);

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Poll completed!"));

                Console.WriteLine($"{DateTime.Now} | Poll: \"{title}\" - completed! Total votes: {totalVotes}. The poll was valid for {pollTime} minutes.");
            }
            catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"{DateTime.Now} | {ex.Message}."); Console.ResetColor(); }
        }

        #endregion
    }
}
