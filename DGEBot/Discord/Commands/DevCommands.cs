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

namespace DGE.Discord.Commands
{
    [Summary("Developpers commands")]
    public class DevCommands : ModuleBase<DGECommandContext>
    {
        [Command("TimeCommand")]
        [Summary("Executes a command, and measures the total time taken")]
        public async Task TimeCommand(string command, params string[] args)
        {
            if (command.ToLower() == "timecommand") //Making sure that the user doesn't create an infinite loop
                throw new CommandExecutionException("Cannot time the execution of TimeCommand (bc the current code would cause a stack overflow)");

            Stopwatch stopwatch = new Stopwatch(); //Debug
            stopwatch.Start();

            bool commandExecutionSuccess = await CommandHandler.ExecuteCommand(Context, Context.bot.commandPrefix.Length + "TimeCommand".Length + 1);
            //Have to pay attention cause we do not get information on the previous argpos, so previous commands are not ignored and this can easily fall into an endless recursion cycle

            stopwatch.Stop();
            if (commandExecutionSuccess)
                await ReplyAsync($"{LogPrefixes.DGE_DEBUG}Execution of the `{command}` command took {stopwatch.Elapsed.TotalSeconds * 1000}ms");
            else
                await ReplyAsync($"{LogPrefixes.DGE_DEBUG}Failed execution the `{command}` command, failing took {stopwatch.Elapsed.TotalSeconds * 1000}ms");
        }

        [Command("ClearReports")]
        [Summary("Removes feedback report messages and files")]
        [RequireOwner]
        public async Task CommandClearReports()
        {
            UserFeedbackHandler.ClearReports();
            await ReplyAsync(LogPrefixes.DGE_LOG + "Cleared every reports");
        }

        [Command("DeleteReport")]
        [Summary("Deletes the report with the associated Id from the file and removes its message")]
        [RequireOwner]
        public async Task CommandRemoveReport(string reportId)
        {
            try
            {
                UserFeedbackHandler.DeleteReport(reportId);
            }
            catch (KeyNotFoundException e)
            {
                throw new CommandExecutionException(e.Message, e);
            }
            await ReplyAsync($"{LogPrefixes.DGE_LOG} Deleted report {reportId}");
        }

    }
}
