using DiscordBot.Config;
using DiscordBot.Models;
using DiscordBot.SlashCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace DiscordBot
{
    internal class Bot
    {
        internal static DiscordClient Client { get; private set; }
        internal InteractivityExtension Interactivity { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }
        internal SlashCommandsExtension SlashCommands { get; private set; }

        internal async Task RunBotAsync()
        {
            JSONReader jsonReader = new();
            await jsonReader.ReadJson();

            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = jsonReader.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
            });

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis
            });

            SlashCommands = Client.UseSlashCommands();
            SlashCommands.RegisterCommands<CreateReactionRolesCommand>();
            SlashCommands.RegisterCommands<PollCommand>();
            SlashCommands.RegisterCommands<KickCommand>();
            SlashCommands.RegisterCommands<BanCommands>();
            SlashCommands.RegisterCommands<TimeoutCommand>();
            SlashCommands.RegisterCommands<MemberInfoCommand>();
            SlashCommands.RegisterCommands<RoleCommands>();           

            Client.Ready += OnClientReady;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            Logger.Info("Bot is ready!");
            return Task.CompletedTask;
        }
    }
}
