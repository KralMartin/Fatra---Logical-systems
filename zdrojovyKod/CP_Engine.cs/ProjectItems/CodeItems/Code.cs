using CP_Engine.cs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CP_Engine.SchemeItems;

namespace CP_Engine
{
    public class Code
    {
        public string Title { get; set; }
        public string CodeText { get; set; }

        public Code() { }

        internal Code(XmlReader xml)
        {
            this.Title = xml.GetAttribute(XML.Param1);
            xml.Read();
            if (xml.HasValue)
            {
                this.CodeText = xml.Value;
                xml.Read();
                xml.ReadEndElement();
            }
        }

        internal void Save(XmlWriter xml)
        {
            xml.WriteAttributeString(XML.Param1, Title);
            xml.WriteValue(this.CodeText);
        }
        
        internal List<string> GetCleanText()
        {
            List<string> toReturn = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool ignore = false;
            for (int i = 0; i < CodeText.Length; i++)
            {
                char c = CodeText[i];
                if (c == (char)10)
                {
                    ignore = false;
                    toReturn.Add(sb.ToString());
                    sb.Clear();
                }
                else if (ignore)
                    continue;
                else if (c == '-' && CodeText[i + 1] == '-')
                {
                    ignore = true;
                    i++;
                    continue;
                }
                else
                    sb.Append(c);
            }
            if (sb.Length > 0)
                toReturn.Add(sb.ToString());
            return toReturn;
        }        
    }
}
