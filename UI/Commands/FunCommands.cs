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

            int maxCount = int.Parse(args[0]);
            int step = int.Parse(args[1]);

            while (number < maxCount)
            {
                watch.Start();
                while ((message + number + '\n').Length < 2000 && !(number > maxCount))
                {
                    message += number.ToString() + "\n";
                    number += step;
                }
                watch.Stop();

                await Context.Channel.SendMessageAsync(message);
                System.Threading.Thread.Sleep(200 - (int) watch.ElapsedMilliseconds);
                message = new string("");
            }
        }
    }
}
