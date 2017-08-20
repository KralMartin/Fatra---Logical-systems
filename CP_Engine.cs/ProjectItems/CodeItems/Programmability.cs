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
    public class Programmability
    {
        public List<Code> CodeItems { get; private set; }
        public List<Function> FunctionItems { get; private set; }
        public int InstructionWidth { get; private set; }

        internal Programmability()
        {
            CodeItems = new List<Code>();
            FunctionItems = new List<Function>();
            InstructionWidth = 8;
        }

        internal void Save(XmlWriter xml)
        {
            xml.WriteAttributeString(XML.Param1, "Random shit");
            if (CodeItems.Count > 0)
            {
                xml.WriteStartElement(XML.Element1);
                foreach (Code code in CodeItems)
                {
                    xml.WriteStartElement(XML.Item);
                    code.Save(xml);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            if (FunctionItems.Count > 0)
            {
                xml.WriteStartElement(XML.Element2);
                foreach (Function fun in FunctionItems)
                {
                    xml.WriteStartElement(XML.Item);
                    fun.Save(xml);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
        }

        internal void Load(XmlReader xml)
        {
            string str = xml.GetAttribute(XML.Param1);
            if (xml.IsEmptyElement == false)
            {
                xml.Read();
                if (xml.IsStartElement(XML.Element1))
                {
                    xml.Read();
                    while (true)
                    {
                        if (xml.IsStartElement(XML.Item))
                        {
                            Code code = new Code(xml);
                            this.CodeItems.Add(code);
                        }
                        else
                            break;
                    }
                    xml.ReadEndElement();
                }
                if (xml.IsStartElement(XML.Element2))
                {
                    xml.Read();
                    while (true)
                    {
                        if (xml.IsStartElement(XML.Item))
                        {
                            Function fun = new Function(xml);
                            this.FunctionItems.Add(fun);
                        }
                        else
                            break;
                    }
                    xml.ReadEndElement();
                }
                xml.ReadEndElement();
            }
            else
                xml.Read();
        }

        public bool RecompileFunctions()
        {
            try
            {
                foreach (Function fun in FunctionItems)
                    fun.UpdateParameters();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetNoteText()
        {
            return "Note that value on Xth row and Yth position in row goes to memory cell on adress = X*"
                       + InstructionWidth + "+" + InstructionWidth + " - Y\n\n";
        }

        public List<bool> Translate(Code code)
        {
            try
            {
                Compilator compilator = new Compilator(this);
                return compilator.Translate(code);
            }
            catch
            {
                return null;
            }
        }

        internal Queue<bool> Rotate(List<bool> bits)
        {
            Queue<bool> toReturn = new Queue<bool>();
            Stack<bool> bits8 = new Stack<bool>();
            foreach (bool value in bits)
            {
                bits8.Push(value);

                if (bits8.Count >= this.InstructionWidth)
                {
                    while (bits8.Count > 0)
                        toReturn.Enqueue(bits8.Pop());
                }
            }
            return toReturn;
        }
    }
}
