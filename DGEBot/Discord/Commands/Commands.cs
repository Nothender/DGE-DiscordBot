using DGE.Exceptions;
using DGE.UI.Feedback;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Color = Discord.Color;

namespace DGE.Discord.Commands
{
    [Summary("Base commands")]
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        [Summary("Replies with \"Pong\"")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        [Command("Pong")]
        [Summary("Replies with \"Ping\"")]
        public async Task Pong()
        {
            await ReplyAsync("Ping");
        }

        [Command("42")]
        [Summary("Replies with \"42\"")]
        public async Task Reply42()
        {
            await ReplyAsync("42");
        }

        [Command("PingLatency")]
        [Alias("PingL")]
        [Summary("Shows the latency from the bot to the gateway server")]
        public async Task PingLatency()
        {
            await ReplyAsync($"Pong : the latency is {Context.Client.Latency}ms");
        }

        [Command("Stop")]
        [Alias("Shutdown", "Quit", "Exit", "STFU", "Shut")]
        [RequireOwner]
        [Summary("Stops the bot")]
        public async Task Stop()
        {
            await ReplyAsync("Shutting DGE - FW and Apps");
            Main.Stop();
        }

        [Command("SendFeedback")]
        [Alias("ReportBug")]
        [Summary("Sends a feedback report, you can enter this command like that : {prefix}SendFeedback\n\"Summary\"\n\"Description\"\n\"additional info 1\" \"additional info 2\" etc...")]
        public async Task SendFeedbackCommand(string summary, string description, params string[] additionnalInfo)
        {
            FeedbackInfo feedbackInfo = new FeedbackInfo
                ($"Sent by discord user @{Context.User.Username} (id : {Context.User.Id}), in the [{Context.Guild.Name}] discord server, from the #{Context.Channel.Name} channel",
                summary,
                description,
                additionnalInfo);

            UserFeedbackHandler.SendFeedback(feedbackInfo);
            await ReplyAsync("Thanks for sending feedback");
        }

        [Command("Help")]
        [Summary("Replies with all existing command, or the description and parameters of the specified command or module")]
        public async Task Help(string valueName = null)
        {
            //TODO: Clean-up this command a bit (methods/delegates/actions, text and names) gl hf
            //Code dirty af
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(Color.Blue);

            if (valueName is null)
            {
                embed.WithAuthor("Modules :");

                DiscordCommandManager.commands.Modules.ToList().ForEach(m => embed.AddField(
                    m.Name,
                    m.Summary is null ? "No summary" : m.Summary,
                    true));
            }
            else
            {
                valueName = valueName.ToLower();

                List<ModuleInfo> modules = DiscordCommandManager.commands.Modules.ToList();
                List<CommandInfo> commands = DiscordCommandManager.commands.Commands.ToList();

                if (modules.Any(m => m.Name.ToLower() == valueName))
                {
                    ModuleInfo module = modules.Find(m => m.Name.ToLower() == valueName);
                    module.Commands.ToList().ForEach(c =>
                    {
                        embed.AddField($"{c.Name} {(c.Aliases.Count == 1 ? "" : $"({string.Join(", ", c.Aliases.ToArray(), 1, c.Aliases.Count - 1)})")}",
                            $"{(c.Summary is null ? "No summary" : c.Summary)}" +
                            (c.Parameters.Count == 0 ? "" : $"\nparams : {(string.Join(", ", c.Parameters.Select(p => $"{p.Type.Name} {p.Name}")))}"));
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
                        $"\nremarks : {(command.Remarks is null ? "No remarks" : command.Remarks)}");
                }
                else
                {
                    throw new CommandExecutionException($"Couldn't find any Module or Command name matching for `{valueName}`");
                }
            }

            await ReplyAsync(null, false, embed.Build());
        }
    }
}