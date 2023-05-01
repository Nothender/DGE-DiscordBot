using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DGE.Misc;
using DGE.Misc.BetaTesting;
using DGE.Application;
using Discord.WebSocket;
using Discord.Interactions;
using System.ComponentModel;

namespace DGE.Discord.Commands
{
    [Description("Command module for beta testing versions (here to show points gotten by triggering the most exceptions possible)")]
    public class BetaTestingCommands : DGEModuleBase
    {
        [SlashCommand("ShowPoints", "Shows the point count of the current or specified user")]
        public async Task CommandShowPoints(IUser user = null)
        {
            if (user is null)
                await RespondAsync($"You have {BetaTestingPointsCounter.GetUserPoints(Context.User.Id)} points");
            else
                await RespondAsync($"The user {user.Username} has {BetaTestingPointsCounter.GetUserPoints(user.Id)} points");
        }

        [SlashCommand("ShowPointsLeaderboard", "Shows leaderboard of the users (most points to least points)")]
        public async Task CommandShowPointsLeaderboard()
        {
            List<UserPointsPair> users = BetaTestingPointsCounter.GetUsersPoints(Context.Client as DiscordSocketClient);
            users.Sort((x, y) => y.points.CompareTo(x.points));

            string leaderboard = "";
            for (int i = 0; i < users.Count; i++)
            {
                UserPointsPair u = users[i];
                leaderboard += $"{i}. {u.user.Mention}\nwith {u.points} points\n\n";
            }

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithDescription(leaderboard);
            embed.WithTitle("Leaderboard - Points for most errors triggered");

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("ResetPoints", "Resets the points for every user or just the specified one")]
        [RequireOwner]
        public async Task CommandResetPoints(IUser user = null)
        {
            if (user is null)
            {
                BetaTestingPointsCounter.ResetPoints();
                await RespondAsync("Points were reset for every users");
            }
            else
            {
                BetaTestingPointsCounter.ResetPoints(user.Id);
                await RespondAsync($"Points were reset for the user {user.Username}");
            }
        }

        [SlashCommand("StartApp", "Stops the app of specified id")]
        [RequireOwner]
        public async Task CommandStartApp(int id)
        {
            Application.IApplication app = ApplicationManager.Get(id);
            app.Start();
            await RespondAsync($"Started app {id} of type {app.GetType().Name}");
        }

        [SlashCommand("StopApp", "Stops the app of specified id")]
        [RequireOwner]
        public async Task CommandStopApp(int id)
        {
            Application.IApplication app = ApplicationManager.Get(id);
            app.Stop();
            await RespondAsync($"Stopped app {id} of type {app.GetType().Name}");
        }

        [SlashCommand("UselessCommand", "Is useless, no i swear, don't believe me ? well then try it.")]
        public async Task CommandUselessCommand()
        {
            await Task.Run(() => { });
        }

    }
}
