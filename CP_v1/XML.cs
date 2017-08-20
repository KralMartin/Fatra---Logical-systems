using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CP_v1
{
    class XML
    {
        internal static string ProjectsPath = "ProjectsPath";

        internal static string GetElementValueSimple(XmlReader xml)
        {
            xml.Read();
            string toReturn = xml.Value;
            xml.Read();
            return toReturn;
        }
    }
}
