using CP_Engine.cs.ProjectItems.CodeItems;
using CP_Engine.cs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CP_Engine
{
    public class Function
    {
        public string Name { get; set; }
        public string FunctionCode { get; set; }
        public string Description { get; set; }

        List<Parameter> innerParameters;
        internal List<Parameter> Parameters { get; private set; }
        internal string FunctionName { get; set; }
        internal int TotalBits { get; set; }

        public Function() { }

        public Function(XmlReader xml)
        {
            this.Name = xml.GetAttribute(XML.Param1);
            this.FunctionCode = xml.GetAttribute(XML.Param2);
            xml.Read();
            if (xml.HasValue)
            {
                this.Description = xml.Value;
                xml.Read();
                xml.ReadEndElement();
            }
        }

        internal void Save(XmlWriter xml)
        {
            xml.WriteAttributeString(XML.Param1, Name);
            xml.WriteAttributeString(XML.Param2, FunctionCode);
            xml.WriteValue(this.Description);
        }

        internal Parameter GetParameter(char paramName)
        {
            foreach (Parameter param in this.Parameters)
            {
                if (param.Name == paramName)
                    return param;
            }
            return null;
        }

        public void UpdateParameters()
        {
            Parameters = new List<Parameter>();
            Dictionary<char, Parameter> parameterDict = new Dictionary<char, Parameter>();
            string lowerVariant = Name.ToLowerInvariant();
            int functionNameIndex = lowerVariant.IndexOf(' ');
            if (functionNameIndex < 0)
            {
                this.FunctionName = Name;
            }
            else
            {
                FunctionName = lowerVariant.Substring(0, functionNameIndex);

                string parameterString = lowerVariant.Substring(functionNameIndex + 1, Name.Length - functionNameIndex - 1);
                string[] paramStringArray = parameterString.Split(',');
                for (int i = 0; i < paramStringArray.Length; i++)
                {
                    paramStringArray[i] = paramStringArray[i].Trim();
                    if (paramStringArray[i].Length != 1 || paramStringArray[i][0] < 'a' || paramStringArray[i][0] > 'z')
                        throw new Exception("Function parameter has to be only 'a' - 'z' character!");
                    Parameter p = new Parameter(paramStringArray[i][0], Parameters.Count);
                    Parameters.Add(p);
                    parameterDict.Add(paramStringArray[i][0], p);
                }
            }
            //FunctionCode
            TotalBits = 0;
            innerParameters = new List<Parameter>();
            lowerVariant = FunctionCode.ToLowerInvariant().Replace(" ", "");

            char lastChar = (char)1;
            for (int i = 0; i < lowerVariant.Length; i++)
            {
                char c = lowerVariant[i];
                if (c == '+')
                {
                    i++;
                    string value = lowerVariant[i].ToString();
                    parameterDict[lastChar].Adjust += Convert.ToInt16(value);
                }
                else if (c == '-')
                {
                    i++;
                    string value = lowerVariant[i].ToString();
                    parameterDict[lastChar].Adjust -= Convert.ToInt16(value);
                }
                else
                {
                    if (c != '0' && c != '1')
                    {
                        if (parameterDict.ContainsKey(c) == false)
                            throw new Exception(string.Format("There is no parameter '{0}' in function text definition!", c));
                        TotalBits++;
                        lastChar = c;
                        parameterDict[c].NumberLenght++;
                    }
                }
            }

        }
    }
}
