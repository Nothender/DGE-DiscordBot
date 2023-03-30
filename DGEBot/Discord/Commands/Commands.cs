using DGE.Core;
using DGE.Discord.Handlers;
using DGE.Exceptions;
using DGE.UI.Feedback;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Color = Discord.Color;

namespace DGE.Discord.Commands
{
    public class Commands : DGEModuleBase
    {
        [SlashCommand("ping", "Replies with pong")]
        public Task Ping()
        {
            AssemblyBot.logger.Log("Pinged", EnderEngine.Logger.LogLevel.DEBUG);
            RespondAsync("Pong").Wait();
            return Task.CompletedTask;
        }

        /*
        [SlashCommand("pong", "Replies with \"Ping\"")]
        public async Task Pong()
        {
            await RespondAsync("Ping");
        }

        [SlashCommand("fortytwo", "Replies with \"42\"")]
        public async Task Reply42()
        {
            await RespondAsync("42");
        }

        [SlashCommand("pinglatency", "Shows the latency from the bot to the gateway server")]
        public async Task PingLatency()
        {
            await RespondAsync($"Pong : the latency is {(Context.Client as DiscordSocketClient).Latency}ms");
        }

        [SlashCommand("sendfeedback", "Sends a feedback report")]
        public async Task SendFeedbackCommand(string summary, string description, string additionnalInfo)
        {
            FeedbackInfo feedbackInfo = new FeedbackInfo
                ($"Sent by discord user @{Context.User.Username} (id : {Context.User.Id}), in the [{Context.Guild.Name}] discord server, from the #{Context.Channel.Name} channel",
                summary,
                description,
                new string[] { additionnalInfo });

            //UserFeedbackHandler.SendFeedback(feedbackInfo, Context.bot.feedbackChannel);
            await RespondAsync("Thanks for sending feedback");
        }

        [SlashCommand("help", "Replies with the command's description")]
        public async Task CommandHelp(string valueName = null)
        {
            // TODO: Fix

            //TODO: Clean-up this command a bit (methods/delegates/actions, text and names) gl hf
            //Code dirty af
            //TODO: make the embed look cleaner too
            /*
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(Color.Blue);

            if (valueName is null)
            {
                embed.WithAuthor("Modules :");

                Context.bot.commandsService.Modules.ToList().ForEach(m => embed.AddField(
                    m.Name,
                    m.Summary is null ? "No summary" : m.Summary,
                    false));
            }
            else
            {
                valueName = valueName.ToLower();

                List<ModuleInfo> modules = Context.bot.commandsService.Modules.ToList();
                List<CommandInfo> commands = Context.bot.commandsService.Commands.ToList();

                if (modules.Any(m => m.Name.ToLower() == valueName))
                {
                    ModuleInfo module = modules.Find(m => m.Name.ToLower() == valueName);
                    module.Commands.ToList().ForEach(c =>
                    {
                        embed.AddField($"{c.Name} {(c.Aliases.Count == 1 ? "" : $"({string.Join(", ", c.Aliases.ToArray(), 1, c.Aliases.Count - 1)})")}",
                            $"{(c.Summary is null ? "No summary" : c.Summary)}" +
                            (c.Parameters.Count == 0 ? "" : $"\nparams : {(string.Join(", ", c.Parameters.Select(p => $"{p.Type.Name} {p.Name}")))}"), false);
                    });
                    embed.WithAuthor($"{module.Name} module :");
                    embed.WithDescription(module.Summary is null ? "No summary" : module.Summary);
                }
                else if (commands.Any(c => c.Aliases.Contains(valueName)))
                {
                    CommandInfo command = commands.Find(c => c.Aliases.Contains(valueName));
                    embed.AddField($"{command.Name} {(command.Aliases.Count == 1 ? "" : $"({string.Join(", ", command.Aliases.ToArray(), 1, command.Aliases.Count - 1)})")} command :",
                        $"{(command.Summary is null ? "No summary" : command.Summary)}" +
                        (command.Parameters.Count == 0 ? "" : $"\nparams : {(string.Join(", ", command.Parameters.Select(p => $"{p.Type.Name} {p.Name}")))}") +
                        $"\nremarks : {(command.Remarks is null ? "No remarks" : command.Remarks)}", false);
                }
                else
                {
                    throw new CommandExecutionException($"Couldn't find any Module or Command name matching for `{valueName}`");
                }
            }

            await RespondAsync($"Use `{Context.bot.commandPrefix}HelpAll` to list all commands from every module", embed: embed.Build());
            
        }

        [SlashCommand("helpall", "Replies with a description of every command")]
        public async Task CommandHelpAll()
        {
            //TODO: Fix
            /*
            for (int i = 0; i < Context.bot.commandsService.Commands.Count(); i += 25) //Creates an embed for each 25 commands and sends it (cannot create more than 25 fields in one embed)
            {

                EmbedBuilder embed = new EmbedBuilder();

                for (int j = i; j < i + 25; j++)
                {
                    if (j >= Context.bot.commandsService.Commands.Count())
                        break;

                    CommandInfo<IParameterInfo> c = Context.bot.commandsService.Commands.ElementAt(j);

                    embed.AddField($"{c.Name} {(c.Aliases.Count == 1 ? "" : $"({string.Join(", ", c.Aliases.ToArray(), 1, c.Aliases.Count - 1)})")} [{c.Module.Name}]",
                        $"{(c.Summary is null ? "No summary" : c.Summary)}" +
                        (c.Parameters.Count == 0 ? "" : $"\nparams : {(string.Join(", ", c.Parameters.Select(p => $"{p.Type.Name} {p.Name}")))}"), false);
                }

                await RespondAsync(embed: embed.Build());
            }
        }*/
        
    }
}