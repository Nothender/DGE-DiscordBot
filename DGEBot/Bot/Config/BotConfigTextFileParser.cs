using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using EnderEngine;

namespace DGE.Bot.Config
{
    public class BotConfigTextFileParser : IBotConfigLoader, IBotConfigSaver
    {

        private readonly string file;

        public BotConfigTextFileParser(string file)
        {
            this.file = file;
        }

        public IBotConfig LoadConfig()
        {
            string[] lines = File.ReadAllLines(file);
            return new BotConfig(
                lines[0],                   // Token
                ulong.Parse(lines[2]),       // DebugGuildId (Nullable)
                ulong.Parse(lines[1])       // FeedbackChannelId
                );
        }

        public void SaveConfig(IBotConfig config)
        {
            string[] lines =
                {
                    config.Token,
                    config.DebugGuildId.ToString(),
                    config.FeedbackChannelId.ToString()
                };
            if (!File.Exists(file))
            {
                using (FileStream fs = File.Create(file))
                    fs.Write(
                        Encoding.ASCII.GetBytes(
                            string.Join('\n', lines)));
            }
            else
                File.WriteAllLines(file, lines);
        }
    }
}
