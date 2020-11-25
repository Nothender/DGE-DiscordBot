using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordGameEngine.Core;
using DiscordGameEngine.Misc;
using DiscordGameEngine.Rendering;

namespace DiscordGameEngine.UI.Commands
{
    public class FunCommands : ModuleBase<SocketCommandContext>
    {

        [Command("sendMsgsFast")]
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

        [Command("count")]
        public async Task Count(params string[] args)
        {
            int number = 0;
            string message = new string("");

            Stopwatch watch = new Stopwatch();

            int maxCount = args.Length > 0 ? int.Parse(args[0]) : 42;
            int step = Math.Abs(args.Length > 1 ? int.Parse(args[1]) : 1) * (maxCount != 0 ? maxCount/Math.Abs(maxCount) : 1);
            step = step != 0 ? step : 1;

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
        }

        [Command("beginCounting")]
        public async Task BeginCounting()
        {
            if (await Counting.AddCountingChannel(Context.Channel))
                await ReplyAsync("Began counting in this channel, starting count is 0");
            else
                await ReplyAsync("This channel already is a counting channel, current count : " + Counting.GetCurrentChannelCount(Context.Channel.Id));
        }

        [Command("getCurrentCount")]
        public async Task GetCurrentCount()
        {
            await ReplyAsync("The current count in this channel is : " + Counting.GetCurrentChannelCount(Context.Channel.Id));
        }

        [Command("saveChannel")]
        public async Task SaveChannel()
        {
            Counting.SaveToJSON();
            await ReplyAsync("saved");
        }


    }
}
