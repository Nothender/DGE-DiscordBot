using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using EnderEngine;
using System.Xml;
using System.Xml.Linq;

namespace DGE.Bot.Config
{
    public class BotConfigXMLFileParser : IBotConfigLoader, IBotConfigSaver
    {

        private readonly string file;

        public BotConfigXMLFileParser(string file)
        {
            this.file = file;
        }

        public IBotConfig LoadConfig()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNode config = doc.DocumentElement;

            string configType = config.Attributes["type"]?.InnerText;
            if (configType != "XMLDiscordBotConfig")
                throw new Exception("Config does not seem to be of type `XMLDiscordBotConfig`");
            
            string botName, token;
            ulong debugGuildID, feedbackChannelID;
            try
            {
                botName = config.SelectSingleNode("botName").InnerText;
                token = config.SelectSingleNode("token").InnerText;
                debugGuildID = ulong.Parse(config.SelectSingleNode("debugGuildID").InnerText);
                feedbackChannelID = ulong.Parse(config.SelectSingleNode("feedbackChannel").InnerText);
            }
            catch (Exception e)
            {
                throw new Exception("One of the attributes being parsed was not the right type", e);
            }

            // Gathering command modules
            XmlNodeList xmlModules = config.SelectSingleNode("commandModules").ChildNodes;
            List<ICommandModuleConfig> commandModules = new List<ICommandModuleConfig>();

            if (xmlModules.Count > 0)
            {
                foreach (XmlNode node in xmlModules)
                {
                    try
                    {
                        commandModules.Add(new CommandModuleConfig(
                            node.SelectSingleNode("assemblyQualifiedName").InnerText,
                            node.SelectSingleNode("name").InnerText,
                            bool.Parse(node.SelectSingleNode("debugOnly").InnerText)
                        )) ;
                    }
                    catch(Exception e)
                    {
                        throw new Exception($"Exception occured when trying to parse command module config number {commandModules.Count + 1}", e);
                    }
                }
            }
            return new BotConfig(botName, token, debugGuildID, feedbackChannelID, commandModules.ToArray());
        }

        public void SaveConfig(IBotConfig config)
        {
            XElement[] modules = new XElement[config.Modules.Length];
            ICommandModuleConfig mcfg;
            for (int i = 0; i < config.Modules.Length; i++)
            {
                mcfg = config.Modules[i];
                modules[i] = new XElement("commandModule",
                    new XElement("name", mcfg.ModuleName),
                    new XElement("assemblyQualifiedName", mcfg.AssemblyQualifiedName),
                    new XElement("debugOnly", mcfg.DebugOnly)
                    );
            }

            XElement xconfig = new XElement(
                "config",
                new XAttribute("type", "XMLDiscordBotConfig"),
                    new XElement("botName", config.BotName),
                    new XElement("token", config.Token),
                    new XElement("debugGuildID", config.DebugGuildId),
                    new XElement("feedbackChannel", config.FeedbackChannelId),
                    new XElement("commandModules", modules)
                );

            using (var f = File.CreateText(file))
                f.Write(xconfig.ToString());
        }
    }
}
