using System;
using System.Collections.Generic;
using EnderEngine;
using Discord.WebSocket;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Discord.Commands;
using System.Linq;
using DGE.ProgramModules;
using DGE.Bot;
using DGE.Core;
using Discord;

namespace DGE.ProgramModules
{
    public class CountingProgram : ProgramModule
    {
        static CountingProgram()
        {
            SetDescription(typeof(CountingProgram), "Has a counter that is originally 0, the point is to increment it by step (custom variable given by the user 1 or -1)");
        }

        public CountingProgram(ProgramData programData, IBot bot) : base(programData, bot) { }

        private class CountAndDirection
        {
            public int currentCount;
            public int countingDirection;

            public CountAndDirection(int count, int countDirection) 
            {
                currentCount = count;
                countingDirection = countDirection;
            }

            public void Increment(int value)
            {
                currentCount += value;
            }

            public void SetDirection(int value)
            {
                countingDirection = value;
            }

            public void SetCount(int value)
            {
                currentCount = value;
            }

        }

        private static readonly Dictionary<ulong, CountAndDirection> channelsCount = new Dictionary<ulong, CountAndDirection>();

        private readonly ulong mainChannelId;

        public CountingProgram(SocketCommandContext context) : base(context)
        {
            mainChannelId = context.Channel.Id;

            if (!channelsCount.ContainsKey(mainChannelId))
                channelsCount.Add(mainChannelId, new CountAndDirection(0, 1));


            AddChannel(context.Channel.Id, context.Client);
            AddInteraction("SetDirection", SetDirectionCallback);
            AddInteraction("GetCount", GetCurrentCountCallback);
        }

        private void GetCurrentCountCallback(SocketUserMessage umessage)
        {
            umessage.Channel.SendMessageAsync($"The current count is : {channelsCount[mainChannelId].currentCount} and the direction is {channelsCount[mainChannelId].countingDirection}");
        }

        private void SetDirectionCallback(SocketUserMessage umessage)
        {
            char dirChar = umessage.Content.Split()[1][0];
            if (dirChar == '-')
                channelsCount[mainChannelId].SetDirection(-1);
            else if (dirChar == '+')
                channelsCount[mainChannelId].SetDirection(1);
            else
                umessage.Channel.SendMessageAsync("This is no valid sign");
            foreach (ISocketMessageChannel channel in interactionChannels)
                channel.SendMessageAsync("The sign counting direction is " + channelsCount[mainChannelId].countingDirection);
        }

        private class ChannelsCountJSONFormat
        {
            public Dictionary<ulong, CountAndDirection> channelsCount;

            public ChannelsCountJSONFormat(Dictionary<ulong, CountAndDirection> channelsCount)
            {
                this.channelsCount = channelsCount;
            }
        }

        private static double Evaluate(string expression)
        {
            expression = expression.Replace("=", "");
            //Todo: add powers, sqrts, etc...., Optimize using string builder
            string XMLPathExpression = "number(" + new Regex(@"([\+\-\*])")
                .Replace(expression, " ${1} ")
                .Replace("/", " div ")
                .Replace("%", " mod ")
                .Replace("e", "2.718285")
                .Replace("pi", "3.141593")
                + ')';

            return (double)new XPathDocument(new StringReader("<r/>")).CreateNavigator().Evaluate(XMLPathExpression);
        }

        private static Tuple<double, int> GetFirstNumberAndStopIndexFromString(string s)
        {
            string numberStr = "";
            int i;
            for (i = 0; i < s.Length; i++)
            {
                if (int.TryParse(s[i] + "", out _) || s[i] == '.')
                    numberStr += s[i];
                else
                    break;
            }
            return Tuple.Create(double.Parse(numberStr), i);
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage)
        {

            if (umessage.Content.Length < 1)
                return;

            int evaluatedNumber;

            if (int.TryParse(umessage.Content[0] + "", out _))
                evaluatedNumber = (int)GetFirstNumberAndStopIndexFromString(umessage.Content).Item1;
            else if (umessage.Content[0] == '=' || umessage.Content[0] == '-' && umessage.Content.Length > 1)
            {
                try
                {
                    evaluatedNumber = (int)Evaluate(umessage.Content);
                }
                catch
                {
                    umessage.Channel.SendMessageAsync(LogPrefixes.DGE_ERROR + "Could not evaluate expression");
                    return;
                }
            }
            else
                return;

            if (evaluatedNumber == channelsCount[mainChannelId].currentCount + channelsCount[mainChannelId].countingDirection)
            {
                channelsCount[mainChannelId].Increment(channelsCount[mainChannelId].countingDirection);
                umessage.AddReactionAsync(new Emoji("✔️"));
                foreach (ISocketMessageChannel channel in interactionChannels)
                {
                    if (channel.Id != umessage.Channel.Id)
                        channel.SendMessageAsync($"✔️ The user {umessage.Author.Username}#{umessage.Author.Discriminator} brought up the count to : {evaluatedNumber}, in another channel");
                }
            }
            else
            {
                umessage.AddReactionAsync(new Emoji("❌"));
                umessage.Channel.SendMessageAsync($"Wrong number {evaluatedNumber}, the correct number was : {channelsCount[mainChannelId].currentCount + channelsCount[mainChannelId].countingDirection}. Starting back from  0");
                foreach (ISocketMessageChannel channel in interactionChannels)
                {
                    if (channel.Id != umessage.Channel.Id)
                        channel.SendMessageAsync($"❌ The user {umessage.Author.Username}#{umessage.Author.Discriminator} in another channel broke the record with the number : {evaluatedNumber}, the correct number was : {channelsCount[mainChannelId].currentCount + channelsCount[mainChannelId].countingDirection}. Starting back from  0");
                }
                channelsCount[mainChannelId].SetCount(0);
            }

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
