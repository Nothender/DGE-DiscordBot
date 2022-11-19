using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DGE.Discord.Config
{
    public class ConfigTextFileParser : IConfigLoader, IConfigSaver
    {

        private readonly string file;

        public ConfigTextFileParser(string file)
        {
            this.file = file;
        }

        public IConfig LoadConfig()
        {
            string[] lines = File.ReadAllLines(file);
            return new Config(
                lines[0],                   // Token
                lines[2],                   // FeedbackChannelId
                ulong.Parse(lines[1])       // Prefix
                );
        }

        public void SaveConfig(IConfig config)
        {
            string[] lines = 
                { 
                    config.Token, 
                    config.FeedbackChannelId.ToString(), 
                    config.Prefix 
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
