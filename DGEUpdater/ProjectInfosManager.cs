using System;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace DGE
{
    public class ProjectInfosManager //Does not implement IVersion but DGEVersion directly for simplicity reasons (Could change in the future)
    {
        public ProjectInfo[] projectInfos;

        public ProjectInfosManager(string projectInfoFile, string projectConfigFile)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(projectInfoFile);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/modules/module");

            List<DGEVersion> modules = new List<DGEVersion>(nodes.Count);

            foreach(XmlNode node in nodes) //Loading module versions
            {
                modules.Add(DGEVersion.FromString(node.InnerText));
            }
            
            xmlDoc.Load(projectConfigFile);
            nodes = xmlDoc.DocumentElement.SelectNodes("/project/repos");

            projectInfos = new ProjectInfo[nodes.Count];

            for (int i = 0; i < nodes.Count; i++) //Loading Different projects, their URL, and calculating their Version using the modules they use.
            {
                XmlNode node = nodes[i];

                ProjectInfo p = projectInfos[i];

                p.DownloadLatestGet = node.SelectSingleNode("dl-latest").InnerText;
                p.VersionLatestGet = node.SelectSingleNode("v-latest").InnerText;
                p.FetcherOption = node.SelectSingleNode("options").InnerText;

                DGEVersion projectVersion = new DGEVersion("0.0.0.0");

                foreach(XmlNode module in node.SelectNodes("module"))
                {
                    string moduleName = module.InnerText;
                    DGEVersion moduleVersion = modules.Find((m) => m.name == moduleName);
                    if(!(moduleVersion is null)) projectVersion += moduleVersion;
                }

                p.Version = projectVersion;
            }

        }

    }
}
