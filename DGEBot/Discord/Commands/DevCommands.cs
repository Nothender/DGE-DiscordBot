using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DGE.Exceptions;
using DGE.Discord.Handlers;
using DGE.Core;
using DGE.UI.Feedback;
using DGE.Bot;
using Discord;
using Discord.Addons.Interactive;
using Discord.Addons;
using System.Linq;
using DGE.Core.OperatingSystem;

namespace DGE.Discord.Commands
{
    [Summary("Developpers commands")]
    public class DevCommands : DGEInteractiveBase
    {
        [Command("TimeCommand")]
        [Summary("Executes a command, and measures the total time taken")]
        public async Task CommandTimeCommand(string command, params string[] args)
        {
            if (command.ToLower() == "timecommand") //Making sure that the user doesn't create an infinite loop
                throw new CommandExecutionException("Cannot time the execution of TimeCommand (bc the current code would cause a stack overflow)");

            Stopwatch stopwatch = new Stopwatch(); //Debug
            stopwatch.Start();

            bool commandExecutionSuccess = await CommandHandler.ExecuteCommand(Context, Context.bot.commandPrefix.Length + "TimeCommand".Length + 1);
            //Have to pay attention cause we do not get information on the previous argpos, so previous commands are not ignored and this can easily fall into an endless recursion cycle

            stopwatch.Stop();
            if (commandExecutionSuccess)
                await ReplyAsync($"{LogPrefixes.DGE_DEBUG}Execution of the `{command}` command took {stopwatch.Elapsed.TotalMilliseconds}ms");
            else
                await ReplyAsync($"{LogPrefixes.DGE_DEBUG}Failed execution the `{command}` command, failing took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        [Command("ClearReports")]
        [Summary("Removes feedback report messages and files")]
        [RequireOwner]
        public async Task CommandClearReports()
        {
            UserFeedbackHandler.ClearReports(Context.bot.feedbackChannel);
            await ReplyAsync(LogPrefixes.DGE_LOG + "Cleared every reports");
        }

        [Command("DeleteReport")]
        [Summary("Deletes the report with the associated Id from the file and removes its message")]
        [RequireOwner]
        public async Task CommandRemoveReport(string reportId)
        {
            try
            {
                UserFeedbackHandler.DeleteReport(reportId, Context.bot.feedbackChannel);
            }
            catch (KeyNotFoundException e)
            {
                throw new CommandExecutionException(e.Message, e);
            }
            await ReplyAsync($"{LogPrefixes.DGE_LOG} Deleted report {reportId}");
        }

        [Command("Stop", RunMode = RunMode.Async)]
        [Alias("Shutdown", "Quit", "Exit", "STFU", "Shut")]
        [RequireOwner]
        [Summary("Stops the app bot if bot is true, else it shutdowns the entire framework")]
        public async Task CommandStop(bool bot = false)
        {
#if RELEASE
            Random r = new Random();
            string code = r.Next(10, 99).ToString();
            for (int i = 0; i < 6; i++)
            {
                code += '-' + r.Next(10, 99).ToString();
            }
            await ReplyAsync($"Are you sure ? Write code {code.Replace("-", "")} by placing a '-' every 2 numbers");
            IMessage message = await NextMessageAsync();
            if(message is null)
            {
                await ReplyAsync(new DiscordInteractiveTimeoutException(DiscordBot.interactiveTimeoutSeconds).Message.Replace(CommandHandler.AvoidBugReportErrorTag, ""));
                return;
            }
            if(message.Content.Trim() != code)
            {
                await ReplyAsync("Wrong code");
                return;
            }
#endif
            if (bot)
            {
                await ReplyAsync("Shutting down bot");
                Context.bot.Stop();
                return;
            }
            await ReplyAsync("Shutting down everything");
            Main.Stop();
        }

        [Command("Reboot")]
        [Alias("Restart")]
        [RequireOwner]
        [Summary("Reboots the bot if true, and the entire framework if false")]
        public async Task CommandReboot(bool bot = true)
        {
            if (bot)
            {
                await ReplyAsync("Rebooting bot");
                _ = Task.Run(() => //Dont want to await call otherwise it creates a bug
                {
                    Context.bot.Stop();
                    Context.bot.Start();
                });
                return;
            }
            await ReplyAsync("Rebooting Entire framework");
            Scripts.RunApp.CreateProcess(Process.GetCurrentProcess().MainModule.FileName);
            Main.OnStopped += (s, e) => Scripts.RunApp.Run();
            Main.Stop();
        }

    }
}
