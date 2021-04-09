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
using DiscordGameEngine.Core;
using DiscordGameEngine.Rendering;

namespace DiscordGameEngine.UI.Commands
{
    [Summary("Commands adding literaly nothing useful")]
    public class FunCommands : ModuleBase<SocketCommandContext>
    {

        private static Random random = new Random();

        [Command("Send42s")]
        [RequireUserPermission(ChannelPermission.ManageChannels)] //Bc it can be very chiant
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

        [Command("Count")]
        [RequireUserPermission(ChannelPermission.ManageChannels)] //Bc it can be very chiant
        [Summary("Counts from 0 to MaxCount (default 42) a step can be specified (default 1)")]
        public async Task Count(int maxCount = 42, int step = 1)
        {
            _ = Task.Run(async () =>
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
                    await Context.Channel.SendMessageAsync(message);
                    watch.Stop();

                    System.Threading.Thread.Sleep(Math.Max(210 - (int)watch.ElapsedMilliseconds, 0));
                    message = new string("");
                }
            }).ConfigureAwait(false);
            await ReplyAsync("Started counting");
        }

        [Command("PingRandom")]
        [Summary("Pings a random person")]
        public async Task PingRandomPerson()
        {
            await ReplyAsync("Haha ping " + Context.Guild.Users.ElementAt(random.Next(0, Context.Guild.Users.Count)).Mention);
        }

    }
}
