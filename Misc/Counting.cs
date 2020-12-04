using DiscordGameEngine.Core;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DiscordGameEngine.Misc
{
    public static class Counting
    {

        private static Dictionary<ulong, int> ChannelsCount = new Dictionary<ulong, int>();
        private static string JSONFileName = "ChannelsCountingCounts";

        public static Task<bool> AddCountingChannel(ISocketMessageChannel channel, int count = 0)
        {
            if (ChannelsCount.ContainsKey(channel.Id))
                return Task.FromResult(false);
            ChannelListener.AddListenedChannel(channel.Id, MessageRecieved);
            ChannelsCount.Add(channel.Id, count);
            return Task.FromResult(true);
        }

        internal static void Start()
        {
            Program.OnShutdown += Shutdown;
            LoadFromJSON();
        }

        private static void Shutdown(object sender, EventArgs e)
        {
            SaveToJSON();
        }

        public static int GetCurrentChannelCount(ulong channelID)
        {
            return ChannelsCount[channelID];
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
            try
            {
                File.WriteAllText(Core.Core.pathToSavedData + "\\" + JSONFileName + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(new ChannelsCountJSONFormat(ChannelsCount)));
            }
            catch (Exception e)
            {
                Program.DGELogger.Log(e.Message, EnderEngine.Logger.LogLevel.ERROR);
            }
        }

        private static void LoadFromJSON()
        {
            if (!File.Exists(Core.Core.pathToSavedData + "\\" + JSONFileName + ".json"))
            {
                Program.DGELogger.Log("The file \"" + Core.Core.pathToSavedData + "\\" + JSONFileName + ".json" + "\" doesn't not exist, loading info from file failed", EnderEngine.Logger.LogLevel.WARN);
                return;
            }
            StreamReader stream = new StreamReader(Core.Core.pathToSavedData + "\\" + JSONFileName + ".json");
            try
            {
                ChannelsCount = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<ulong, int>>(stream.ReadToEnd());
            }
            catch (Exception e)
            {
                Program.DGELogger.Log(e.Message, EnderEngine.Logger.LogLevel.ERROR);
            }
            if (stream != null) stream.Close();
        }

        private static void MessageRecieved(ulong channelID, SocketUserMessage message)
        {
            int currentCount = ChannelsCount[channelID];

            if (message.Content.Length < 1)
                return;

            int evaluatedNumber = 0;

            if (int.TryParse(message.Content[0] + "", out _))
                evaluatedNumber = (int)GetFirstNumberAndStopIndexFromString(message.Content).Item1;
            else if (message.Content[0] == '=')
            {
                try
                {
                    evaluatedNumber = (int)Evaluate(message.Content);
                }
                catch
                {
                    message.Channel.SendMessageAsync(DiscordGameEngine.Core.LogManager.DGE_ERROR + "Could not evaluate expression");
                    return;
                }
            }
            else
                return;

            if (evaluatedNumber == currentCount + 1)
            {
                ChannelsCount[channelID] =currentCount + 1;
                message.AddReactionAsync(new Discord.Emoji("✔️"));
            }
            else
            {
                ChannelsCount[channelID] = 0;
                message.AddReactionAsync(new Discord.Emoji("❌"));
                message.Channel.SendMessageAsync(string.Format("Wrong number {0}, the correct number was : {1}. Count is now back to 0", evaluatedNumber, currentCount + 1));
            }

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
    }
}
