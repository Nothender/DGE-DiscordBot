using System;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace DGE
{
    public class ProjectUpdateInfo //Does not implement IVersion but DGEVersion directly for simplicity reasons (Could change in the future)
    {

        public string[] ProjectDlLatest; //The urls / website options from / with which we can download the latest updates
        public string[] ProjectVLatest; //The urls / website options from / with which we can fetch the latest version tag
        public string[] FetcherOptions; //The options to create the fetcher (ex : fetcher type, login info)
        public DGEVersion[] ProjectVersions; //The Global/Project versions

        public ProjectUpdateInfo(string projectInfoFile, string projectConfigFile)
        {
            List<DGEVersion> modules = new List<DGEVersion>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(projectInfoFile);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/modules/module");

            foreach(XmlNode node in nodes) //Loading module versions
            {
                modules.Add(DGEVersion.FromString(node.InnerText));
            }
            
            xmlDoc.Load(projectConfigFile);
            nodes = xmlDoc.DocumentElement.SelectNodes("/project/repos");

            ProjectDlLatest = new string[0] { };
            ProjectVLatest = new string[0] { };
            FetcherOptions = new string[0] { };
            ProjectVersions = new DGEVersion[0] { };

            for (int i = 0; i < nodes.Count; i++) //Loading Different projects, their URL, and calculating their Version using the modules they use.
            {
                XmlNode node = nodes[i];

                ProjectDlLatest = ProjectDlLatest.Append(node.SelectSingleNode("dl-latest").InnerText).ToArray();
                ProjectVLatest = ProjectVLatest.Append(node.SelectSingleNode("v-latest").InnerText).ToArray();
                FetcherOptions = FetcherOptions.Append(node.SelectSingleNode("options").InnerText).ToArray();

                DGEVersion projectVersion = new DGEVersion("0.0.0.0");

                foreach(XmlNode module in node.SelectNodes("module"))
                {
                    string moduleName = module.InnerText;
                    DGEVersion moduleVersion = modules.Find((m) => m.name == moduleName);
                    if(!(moduleVersion is null)) projectVersion += moduleVersion;
                }

                ProjectVersions = ProjectVersions.Append(projectVersion).ToArray();
            }

        }

    }
}
