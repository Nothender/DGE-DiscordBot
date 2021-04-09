using Discord;
using Discord.Commands;
using DiscordGameEngine.Core;
using DiscordGameEngine.Misc.BetaTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGameEngine.Services
{
    public static class CommandHandler
    {

        /// <summary>
        /// A tag used by the CommandExecutionException, it is used to make the error not reported as a bug if the command throws an error
        /// </summary>
        internal static readonly string AvoidBugReportErrorTag = "[DGE-KECE-IBR]"; //DiscordGameEngine-KnownExecutionCommandException-IgnoreBugReporting

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static async Task<bool> ExecuteCommand(ICommandContext context, int argPos)
        {
            IResult execution = await DiscordGameEngineBot._commands.ExecuteAsync(context, argPos, DiscordGameEngineBot._services);
            if (!execution.IsSuccess)
            {
                await LogManager.LogDebug(new LogMessage(LogSeverity.Warning, "Commands", execution.ErrorReason));
                bool ignoreExceptionReport = execution.ErrorReason.Contains(AvoidBugReportErrorTag);
                if (execution.Error == CommandError.Exception && !ignoreExceptionReport)
                    //When there is "[DGE]" in the command execution error, no bug reports will be created (creating a way to the automatic bug reporting) -- Something that may need fixing, poorly coded, not really intuitive
                {
                    string commandExecuted = context.Message.Content.Split(' ', 2)[0].Remove(0, DiscordGameEngineBot.commandPrefix.Length);
                    bool reportWasAlreadySent = UI.Feedback.UserFeedbackHandler.SendFeedback(new UI.Feedback.FeedbackInfo(
                        $"(Automatic [DGE] report) - An exception occured while executing the command `{commandExecuted}`",
                        $"Exception message : \"{execution.ErrorReason}\"",
                        $"User message : \"{context.Message.Content}\"",
                        new string[] { $"Discord user : @{context.User.Username}(id: {context.User.Id})",
                            $"Discord server : {context.Guild.Name}",
                            $"Discord channel : #{context.Channel.Name}"},
                        new string[] { commandExecuted, execution.ErrorReason }));
                    string bugReportLine = reportWasAlreadySent ? "This bug was already reported, and is being fixed" : "A BugReport was automatically sent to the developpers (+1 point)";
                    await context.Channel.SendMessageAsync($"{LogManager.DGE_ERROR}Command execution failed - {execution.ErrorReason}\n*{bugReportLine}*");

                    //Temp error triggering point system
                    if (!reportWasAlreadySent)
                        BetaTestingPointsCounter.GivePointsToUser(context.User.Id);
                }
                else
                {
                    string errorReason = execution.ErrorReason;
                    if (ignoreExceptionReport)
                        errorReason = errorReason.Replace(AvoidBugReportErrorTag, "");
                    await context.Channel.SendMessageAsync($"{LogManager.DGE_ERROR}Command execution failed - {errorReason}\n*See help using `{DiscordGameEngineBot.commandPrefix}help`*");
                }
                return false; //The command execution had problems
            }
            return true; //Could execute the command
        }

    }
}
