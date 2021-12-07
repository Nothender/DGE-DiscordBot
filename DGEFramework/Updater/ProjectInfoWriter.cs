using DGE.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DGE.Updater
{
    internal static class ProjectUpdateInfoWriter
    {

        /// <summary>
        /// Saves modules version as an XML file to be used by the autoupdater
        /// </summary>
        internal static void CreateXMLFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "UTF-8", null), doc.DocumentElement);
            XmlElement elementModules = doc.CreateElement("modules");
            doc.AppendChild(doc.CreateComment("Do not edit"));
            foreach(DGEModule module in DGEModules.modules)
            {
                XmlElement me = doc.CreateElement("module");
                me.AppendChild(doc.CreateTextNode(module.ToString()));
                elementModules.AppendChild(me);
            }
            doc.AppendChild(elementModules);
            doc.Save(Paths.Get("Application") + "ProjectUpdateInfo.xml");
        }

    }
}
