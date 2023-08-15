using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using System.IO;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using System.Linq.Expressions;
using DSharpPlus.EventArgs;
using DiscordBot.Commands;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity.Enums;
using DiscordBot.SlashCommands;

namespace DiscordBot
{
    internal class Bot
    {
        internal DiscordClient Client { get; private set; }
        internal InteractivityExtension Interactivity { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }
        internal SlashCommandsExtension SlashCommands { get; private set; }

        internal async Task RunBotAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
            });

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                EnableDefaultHelp = false,
                StringPrefixes = new string[] { "!" },
                EnableMentionPrefix = true
            });
            Commands.RegisterCommands<CommandModule>();
            Commands.RegisterCommands<RndCommand>();

            SlashCommands = Client.UseSlashCommands();
            SlashCommands.RegisterCommands<CreateReactionRolesCommand>();
            SlashCommands.RegisterCommands<PollCommand>();
            SlashCommands.RegisterCommands<KickCommand>();
            SlashCommands.RegisterCommands<BanCommands>();
            SlashCommands.RegisterCommands<TimeoutCommand>();
            SlashCommands.RegisterCommands<MemberInfoCommand>();
            SlashCommands.RegisterCommands<RoleCommands>();
            

            //Client.MessageCreated += Client_MessageCreated;
            Client.Ready += OnClientReady;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            Console.WriteLine("Bot is ready!");
            return Task.CompletedTask;
        }

        //private Task Client_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs args)
        //{
        //    if (args.Message.ToString().ToLower().Contains("привет"))
        //    {
        //        if (args.Message.Author.Username == "shawtygold")
        //            Client.SendMessageAsync(args.Channel, $"Приветсвую многоуважаемый {args.Author.Username}!");
        //        else if (args.Author.IsBot == false)
        //            Client.SendMessageAsync(args.Channel, $"Привет, {args.Author.Username}");
        //    }

        //    return Task.CompletedTask;
        //}
    }
}
