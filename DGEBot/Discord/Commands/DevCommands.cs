﻿using Discord.Commands;
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
using System.Linq;
using DGE.Core.OperatingSystem;
using DGE.Updater;
using DGE.Console;

namespace DGE.Discord.Commands
{
    [Summary("Developpers commands")]
    public class DevCommands : DGEModuleBase
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
        public async Task CommandStop(bool bot = false, params string[] s)
        {
#if RELEASE
            if (s is null || s.Length == 0 || s[0] != "RELEASE")
            {
                await ReplyAsync("The bot is running in release mod, to shut it down input the command and add \"Release\" at the end");
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

        /*
        [Command("Updater", RunMode = RunMode.Async)]
        [Alias("Update")]
        [RequireOwner]
        [Summary("Runs the update procedure")]
        public async Task CommandUpdate(params string[] args)
        {
            logCallbackChannel = Context.Channel;
            if (args.Length > 0)
            {
                UpdaterCommands.Execute(args, out bool interactive, LogCallback);
                if (interactive) // If not interactive, the command was already executed
                {
                    string action = args[0].ToLower().Trim();
                    if (action == "i" || action == "install")
                    {
                        await ReplyAsync("This will restart the application, are you sure you want to continue ? (y/n)");
                        IMessage answer = await NextMessageAsync();
                        if(!(answer is null))
                        {
                            string content = answer.Content.ToLower().Trim();
                            if (content == "y" || content == "yes")
                            {
                                await ReplyAsync("Installing new update - Rebooting");
                                AssemblyBot.logger.Log("User is running Interactive install procedure - Rebooting", EnderEngine.Logger.LogLevel.INFO);
                                UpdateManager.StartUpdateScript();
                            }
                            else
                                await ReplyAsync("Canceled interactive install procedure");
                        }
                    }
                }
            }
            else // Run interactive procedure
            {
                IUserMessage message = await ReplyAsync("Running interactive udpate procedure");
                UpdateManager.StartUpdater(LogCallback);
                await message.ModifyAsync(m => m.Content = $"{message.Content}\nFetching latest versions");
                UpdateManager.Fetch("all");
                if (UpdateManager.isUpdateAvailable)
                {
                    await message.ModifyAsync(m => m.Content = $"{message.Content}\nNew version(s) available - Downloading update");
                    UpdateManager.Download("all");
                    if (UpdateManager.isUpdateDownloaded)
                    {
                        await message.ModifyAsync(m => m.Content = $"{message.Content}\nA new update was downloaded : Do you want to restart and update the application ? (y/n)");

                        IMessage answer = await NextMessageAsync();
                        if (!(answer is null))
                        {
                            string content = answer.Content.ToLower().Trim();
                            if (content == "y" || content == "yes")
                            {
                                await message.ModifyAsync(m => m.Content = $"{message.Content}\nRestarting and updating");
                                UpdateManager.StartUpdateScript();
                            }
                        }
                        await message.ModifyAsync(m => m.Content = $"{message.Content}\nCanceled operation - Not applying update and restarting");
                        return;
                    }
                }
                await message.ModifyAsync(m => m.Content = $"{message.Content}\nCanceled - No new version available");

            }
        }*/

        private IMessageChannel logCallbackChannel;

        private void LogCallback(string message, EnderEngine.Logger.LogLevel level)
        {
            EmbedBuilder bembed = new EmbedBuilder();
            bembed.WithTitle($"AU Log - {level}"); //TODO: <- setting to the level string from the logger would be more useful (EE Update ofc)
            // bembed.WithColor() <- the color corresponding to the log level
            bembed.WithDescription(message);
            logCallbackChannel.SendMessageAsync(null, false, bembed.Build());
        }

    }
}
