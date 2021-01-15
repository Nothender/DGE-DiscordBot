using DiscordGameEngine;
using DiscordGameEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Discord.Commands;
using System.Globalization;

namespace DiscordGameEnginePlus.Programs
{
    public class CountingProgram : ProgramModule
    {
        private int currentCount = 0;
        private int countingDirection = 1;
        private static string JSONFileName = "ChannelsCountingCounts";

        public CountingProgram(SocketCommandContext context) : base(context)
        {
            DGEMain.OnShutdown += Shutdown;
            LoadFromJSON();

            AddChannel(context.Channel.Id);
            AddInteraction("SetDirection", SetDirectionCallback);
            AddInteraction("GetCount", GetCurrentCountCallback);
        }

        private void GetCurrentCountCallback(SocketUserMessage umessage)
        {
            umessage.Channel.SendMessageAsync($"The current count is : {currentCount} and the direction is {countingDirection}");
        }

        private void SetDirectionCallback(SocketUserMessage umessage)
        {
            char dirChar = umessage.Content.Split()[1][0];
            if (dirChar == '-')
                countingDirection = -1;
            else if (dirChar == '+')
                countingDirection = 1;
            else
                umessage.Channel.SendMessageAsync("This is no valid sign");
            foreach (ISocketMessageChannel channel in interactionChannels)
                channel.SendMessageAsync("The sign counting direction is " + countingDirection);
        }

        private static void Shutdown(object sender, EventArgs e)
        {
            SaveToJSON();
        }

        private class ChannelsCountJSONFormat
        {
            public Dictionary<ulong, int> channelsCount;

            public ChannelsCountJSONFormat(Dictionary<ulong, int> channelsCount)
            {
                this.channelsCount = channelsCount;
            }
        }


        /// <summary>
        /// Saves the current count associated to channels in a json file
        /// </summary>
        public static void SaveToJSON()
        {
            /*try
            {
                File.WriteAllText(Core.pathToSavedData + "\\" + JSONFileName + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(new ChannelsCountJSONFormat(ChannelsCount)));
            }
            catch (Exception e)
            {
                DGEMain.DGELoggerProgram.Log(e.Message, Logger.LogLevel.ERROR);
            }*/
        }

        private static void LoadFromJSON()
        {
            /*if (!File.Exists(Core.pathToSavedData + "\\" + JSONFileName + ".json"))
            {
                DGEMain.DGELoggerProgram.Log("The file \"" + Core.pathToSavedData + "\\" + JSONFileName + ".json" + "\" doesn't not exist, loading info from file failed", EnderEngine.Logger.LogLevel.WARN);
                return;
            }
            StreamReader stream = new StreamReader(Core.pathToSavedData + "\\" + JSONFileName + ".json");
            try
            {
                ChannelsCount = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<ulong, int>>(stream.ReadToEnd());
            }
            catch (Exception e)
            {
                DGEMain.DGELoggerProgram.Log(e.Message, EnderEngine.Logger.LogLevel.ERROR);
            }
            if (stream != null) stream.Close();*/
        }

        private static double Evaluate(string expression)
        {
            expression = expression.Replace("=", "");
            //Todo: add powers, sqrts, etc....
            string XMLPathExpression = "number(" + new Regex(@"([\+\-\*])")
                .Replace(expression, " ${1} ")
                .Replace("/", " div ")
                .Replace("%", " mod ")
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
                    umessage.Channel.SendMessageAsync(DiscordGameEngine.Core.LogManager.DGE_ERROR + "Could not evaluate expression");
                    return;
                }
            }
            else
                return;

            if (evaluatedNumber == currentCount + countingDirection)
            {
                currentCount += countingDirection;
                umessage.AddReactionAsync(new Discord.Emoji("✔️"));
                foreach (ISocketMessageChannel channel in interactionChannels)
                {
                    if (channel.Id != umessage.Channel.Id)
                        channel.SendMessageAsync($"✔️ The user {umessage.Author.Username}#{umessage.Author.Discriminator} brought up the count to : {evaluatedNumber}, in another channel");
                }
            }
            else
            {
                umessage.AddReactionAsync(new Discord.Emoji("❌"));
                umessage.Channel.SendMessageAsync($"Wrong number {evaluatedNumber}, the correct number was : {currentCount + countingDirection}. Starting back from  0");
                foreach (ISocketMessageChannel channel in interactionChannels)
                {
                    if (channel.Id != umessage.Channel.Id)
                        channel.SendMessageAsync($"❌ The user {umessage.Author.Username}#{umessage.Author.Discriminator} in another channel broke the record with the number : {evaluatedNumber}, the correct number was : {currentCount + countingDirection}. Starting back from  0");
                }
                currentCount = 0;
            }

        }
    }
}
