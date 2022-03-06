using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DGE.Config
{
    public class ConfigTextFileLoader : IConfigLoader
    {

        private readonly string file;

        public ConfigTextFileLoader(string file)
        {
            this.file = file;
        }

        public IConfig LoadConfig()
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                return new Config(
                    lines[0],
                    lines[2], 
                    ulong.Parse(lines[1])
                    );
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't parse config : {ex.Message}");
            }
        }

        public void SaveConfig(IConfig config)
        {
            string[] lines = 
                { 
                    config.Token, 
                    config.FeedbackChannelId.ToString(), 
                    config.Prefix 
                };
            File.WriteAllLines(file, lines);
        }
    }
}
