using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEnginePlus.Programs
{
    public class RenameConfusionProgram : ProgramModule
    {

        //This is a simple and stupid program, that was really badly programmed, and will not be improved

        public RenameConfusionProgram(ProgramData programData) : base(programData) { }

        public static string code;

        public RenameConfusionProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id);
            AddInteraction("ConfusionRename", ConfusionRename);
            AddInteraction("Ban", Ban);
            code = NextCode();
        }

        public async void Ban(SocketUserMessage message)
        {
            if (!message.Content.Contains(code))
                return;
            code = NextCode();

            if (message.MentionedUsers.Count == 1)
            {
                SocketGuildChannel channel = message.Channel as SocketGuildChannel;
                IEnumerator<SocketUser> userIterator = message.MentionedUsers.GetEnumerator();
                userIterator.Reset();
                while(userIterator.MoveNext())
                    await channel.Guild.AddBanAsync(userIterator.Current.Id);
            }
        }

        public async void ConfusionRename(SocketUserMessage message)
        {
            if (!message.Content.Contains(code))
                return;
            code = NextCode();
            
            string[] names = { "I", "He", "Me", "They", "Who", "What", "No one", "Some one", "Him", "Them", "42" };

            await message.Channel.TriggerTypingAsync();
            SocketGuildChannel channel = message.Channel as SocketGuildChannel;
            int i = 0;
            foreach(var user in channel.Users)
            {
                try { await user.ModifyAsync(p => p.Nickname = names[i]); }
                catch { }
                i++;
                if (i >= names.Length)
                    i = -1;
            }
        }

        public static string NextCode()
        {
            Random random = new Random();
            string res = "";
            for (int i = 0; i < 42; i++)
            {
                string value = $"{random.Next()}";
                res += value[random.Next(0, value.Length - 1)];
                
                /*
                byte[] bytes = new byte[2];
                random.NextBytes(bytes);
                string c = string.Join("", Encoding.Unicode.GetChars(bytes));
                DiscordGameEngine.DGEMain.DGELoggerProgram.Log($"{c.Length}", EnderEngine.Logger.LogLevel.DEBUG, EnderEngine.Logger.LogMethod.TO_CONSOLE);
                res += c;
                */
            }
            DiscordGameEngine.DGEMain.DGELoggerProgram.Log($"New code : {res}", EnderEngine.Logger.LogLevel.INFO, EnderEngine.Logger.LogMethod.TO_CONSOLE);
            return res;
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage)
        {
        }

        protected override List<object> GetData()
        {
            return new List<object>();
        }

        protected override void LoadData(List<object> data)
        {
        }
    }
}
