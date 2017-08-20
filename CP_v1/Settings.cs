using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CP_v1
{
    class Settings
    {
        private static string SettingsFileName = "settings.xml";

        internal string ProjectsPath { get; set; }


        internal Settings()
        {
            if (File.Exists(SettingsFileName))
                Load();
            else
                CreateDefaultFile();
        }

        private void Load()
        {
            using (XmlReader xml = XmlReader.Create(SettingsFileName))
            {
                while (true)
                {
                    if (xml.IsStartElement(XML.ProjectsPath))
                    {
                        this.ProjectsPath = XML.GetElementValueSimple(xml);
                    }
                    else
                        break;
                }
            }
        }

        private void CreateDefaultFile()
        {
            this.ProjectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KralApp_Saves");
            Save();
        }
                
        private void Save()
        {
            using (XmlWriter xml = XmlWriter.Create(SettingsFileName))
            {
                xml.WriteElementString(XML.ProjectsPath, this.ProjectsPath);
            }

            
        }
    }
}
