﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class TimeoutCommand : ApplicationCommandModule
    {
        [SlashCommand("timeout", "Send the user to think about their behavior.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public static async Task Timeout(InteractionContext ctx, [Option("user", "User to timeout.")] DiscordUser user, 
            [Choice("60 seconds", 0)]
            [Choice("5 minutes", 1)]
            [Choice("10 minutes", 2)]
            [Choice("30 minutes", 3)]
            [Choice("1 hour", 4)]
            [Choice("1 day", 5)] [Option("time", "Timeout time.")] long time, 
            [Option("reason", "Timeout reason.")] string reason = "No reason provided.")
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like that user is in the server, so I can't time them out."
                }));

                return;
            }

            List<DateTimeOffset> dateTimeOffsets = new() {
                DateTimeOffset.Now.LocalDateTime.AddMinutes(1),
                DateTimeOffset.Now.LocalDateTime.AddMinutes(5),
                DateTimeOffset.Now.LocalDateTime.AddMinutes(10),
                DateTimeOffset.Now.LocalDateTime.AddMinutes(30),
                DateTimeOffset.Now.LocalDateTime.AddHours(1),
                DateTimeOffset.Now.LocalDateTime.AddDays(1)
            };

            List<string> times = new()
            {
                "1 minutes",
                "5 minutes",
                "10 minutes",
                "30 minutes",
                "1 hour",
                "1 day"
            };

            try
            {
                await member.TimeoutAsync(dateTimeOffsets[(int)time], reason);
            }
            catch(UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to timeout **{member.Username}**! Please check the role hierarchy and permissions."
                }));
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to timeout that user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"{member.Username} sent to think about their behavior for {times[(int)time]}! Reason: {reason}"
            }));
        }
    }
}
