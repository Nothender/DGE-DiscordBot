using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DGE.Misc;
using DGE.Misc.BetaTesting;

namespace DGE.Discord.Commands
{
    [Summary("Command module for beta testing versions (here to show points gotten by triggering the most exceptions possible)")]
    public class BetaTestingCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ShowPoints")]
        [Summary("Shows the point count of the current or specified user")]
        public async Task CommandShowPoints(IUser user = null)
        {
            if (user is null)
                await ReplyAsync($"You have {BetaTestingPointsCounter.GetUserPoints(Context.User.Id)} points");
            else
                await ReplyAsync($"The user {user.Username} has {BetaTestingPointsCounter.GetUserPoints(user.Id)} points");
        }

        [Command("ShowPointsLeaderboard")]
        [Summary("Shows leaderboard of the users (most points to least points)")]
        public async Task CommandShowPointsLeaderboard()
        {
            List<UserPointsPair> users = BetaTestingPointsCounter.GetUsersPoints(Context.Client);
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

            await ReplyAsync(embed: embed.Build());
        }

        [Command("ResetPoints")]
        [Summary("Resets the points for every user or just the specified one")]
        [RequireOwner]
        public async Task CommandResetPoints(IUser user = null)
        {
            if (user is null)
            {
                BetaTestingPointsCounter.ResetPoints();
                await ReplyAsync("Points were reset for every users");
            }
            else
            {
                BetaTestingPointsCounter.ResetPoints(user.Id);
                await ReplyAsync($"Points were reset for the user {user.Username}");
            }
        }

    }
}
