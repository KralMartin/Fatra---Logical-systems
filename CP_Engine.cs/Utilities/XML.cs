using System.Xml;

namespace CP_Engine.cs.Utilities
{
    class XML
    {
        public const string Item = "item";
        public const string Param1 = "p1";
        public const string Param2 = "p2";
        public const string Param3 = "p3";
        public const string Param4 = "p4";
        public const string Param5 = "p5";

        public const string Element1 = "e1";
        public const string Element2 = "e2";
        public const string Element3 = "e3";
        public const string Element4 = "e4";
        public const string Element5 = "e5";

        internal static string GetElementValueSimple(XmlReader xml)
        {
            xml.Read();
            string toReturn = xml.Value;
            xml.Read();
            return toReturn;
        }

    }
}
