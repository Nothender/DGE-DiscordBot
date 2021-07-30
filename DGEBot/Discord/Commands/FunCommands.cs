using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using DGE.Discord;
using Discord.WebSocket;

namespace DiscordGameEngine.UI.Commands
{
    [Summary("Commands adding literaly nothing useful")]
    public class FunCommands : DGEModuleBase
    {

        private static Random random = new Random();

        [Command("Send42s", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageChannels, Group = "fun.spam42")] //Bc it can be very chiant
        [RequireOwner(Group = "fun.spam42")]
        [Summary("Sends 42 continually in the channel")]
        public async Task SendMsgsFast()
        {
            string str = "42\n";
            List<string> strs = new List<string>(666);
            for (int i = 0; i < 666; i++) strs.Add(str);
            str = string.Concat(strs.ToArray());
            

            for (int i = 0; i < 100; i++)
            {
                await ReplyAsync(str);
                System.Threading.Thread.Sleep(200);
            }
        }

        [Command("Count", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageChannels, Group = "fun.count")] //Bc it can be very chiant
        [RequireOwner(Group = "fun.count")]
        [Summary("Counts from 0 to MaxCount (default 42) a step can be specified (default 1)")]
        public async Task Count(int maxCount = 42, int step = 1)
        {
            int number = 0;
            string message = new string(""); //string builder may be of better use

            Stopwatch watch = new Stopwatch();

            step = Math.Abs(step) * (maxCount != 0 ? maxCount / Math.Abs(maxCount) : 1); //Checking the step to know if we will fall into an infinite loop by having step and maxCount of different signs, then fixing it
            step = step != 0 ? step : maxCount / Math.Abs(maxCount); //If the step is 0, then we put it (sign of maxcount) * 1

            //TODO: improve counting speed (CPU usage)
            while (Math.Abs(number) <= Math.Abs(maxCount))
            {
                watch.Restart();
                while ((message + number + '\n').Length < 2000 && !(Math.Abs(number) > Math.Abs(maxCount)))
                {
                    message += number.ToString() + "\n";
                    number += step;
                }
                await ReplyAsync(message);
                watch.Stop();

                System.Threading.Thread.Sleep(Math.Max(210 - (int)watch.ElapsedMilliseconds, 0));
                message = new string("");
            }
        }

        [Command("PingRandom")]
        [Summary("Pings a random person")]
        [RequireUserPermission(ChannelPermission.MentionEveryone, Group = "fun.pingr")]
        [RequireOwner(Group = "fun.pingr")]
        public async Task PingRandomPerson()
        {
            await ReplyAsync("Haha ping " + (Context.Guild as SocketGuild).Users.ElementAt(random.Next(0, (Context.Guild as SocketGuild).Users.Count)).Mention);
        }

    }
}
