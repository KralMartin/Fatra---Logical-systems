using System;
using System.Xml;
using Microsoft.Xna.Framework;
using CP_Engine.cs.Utilities;
using CP_Engine.SchemeItems;
using System.Collections.Generic;
using System.Text;

namespace CP_Engine.BugItems
{
    /// <summary>
    /// Describes Input or Output of Bug.
    /// Described IOput can be of width x>0, it means it has x SchemeSources.
    /// </summary>
    public class IODescription
    {
        /// <summary>
        /// Text, that shows in bottom bar.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// Text, that shows at IOput.
        /// </summary>
        internal string DisplayText { get; set; }

        /// <summary>
        /// Order number of IOput.
        /// </summary>
        internal int Order { get; set; }

        /// <summary>
        /// This instance refers to IOput in scheme on this coordinates.
        /// </summary>
        internal Point Coords { get; set; }

        /// <summary>
        /// TRUE=Input FALSE=Output
        /// </summary>
        internal bool IsInput { get; private set; }

        /// <summary>
        /// SchemeSources, that are descripted by this instance.
        /// </summary>
        internal List<SchemeSource> SchemeSourcesOnCoords { get; private set; }

        internal IODescription(bool isInput, string title, string desc, int index, Point coords, List<SchemeSource> schemeSourcesOnCoords)
        {
            this.IsInput = isInput;
            this.DisplayText = title;
            this.Description = desc;
            this.Order = index;
            this.Coords = coords;
            this.SchemeSourcesOnCoords = schemeSourcesOnCoords;
        }

        internal string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            if(this.IsInput)
                    sb.Append("Input");
                else
                    sb.Append("Output");

            sb.Append(" (width ");
            sb.Append(this.SchemeSourcesOnCoords.Count + ")");

            if (string.IsNullOrEmpty(this.Description) == false)
            {
                sb.Append(": ");
                sb.Append(this.Description);
            }
            else if (string.IsNullOrEmpty(this.DisplayText) == false)
            {
                sb.Append(": ");
                sb.Append(this.DisplayText);
            }
            return sb.ToString();
        }

        #region XML

        internal IODescription(XmlReader xml)
        {
            this.Coords = new Point(Convert.ToInt32(xml.GetAttribute(XML.Param1)), Convert.ToInt32(xml.GetAttribute(XML.Param2)));
            this.IsInput = Convert.ToBoolean(xml.GetAttribute(XML.Param3));
            this.Order = Convert.ToInt32(xml.GetAttribute(XML.Param4));

            //Default values.
            Description = "";
            DisplayText = "";

            if (xml.IsEmptyElement == false)
            {
                xml.Read();
                if (xml.IsStartElement(XML.Element1))
                {
                    xml.Read();
                    this.DisplayText = xml.Value;
                    xml.Read();
                    xml.ReadEndElement();
                }
                if (xml.IsStartElement(XML.Element2))
                {
                    xml.Read();
                    this.Description = xml.Value;
                    xml.Read();
                    xml.ReadEndElement();
                }
                xml.ReadEndElement();
            }
            else
            {
                xml.Read();
            }
        }

        internal void Save(XmlWriter xml)
        {
            xml.WriteAttributeString(XML.Param1, this.Coords.X.ToString());
            xml.WriteAttributeString(XML.Param2, this.Coords.Y.ToString());
            xml.WriteAttributeString(XML.Param3, this.IsInput.ToString());
            xml.WriteAttributeString(XML.Param4, this.Order.ToString());

            if (string.IsNullOrEmpty(this.DisplayText) == false)
                xml.WriteElementString(XML.Element1, this.DisplayText);
            if (string.IsNullOrEmpty(this.Description) == false)
                xml.WriteElementString(XML.Element2, this.Description);
        }
        #endregion

        /// <summary>
        /// Returns coppy of this instance.
        /// Exclutes SchemeSourcesOnCoords.
        /// </summary>
        /// <returns></returns>
        internal IODescription Export()
        {
            IODescription toReturn = new IODescription(this.IsInput, this.DisplayText, this.Description, this.Order, this.Coords, null);
            return toReturn;
        }

        /// <summary>
        /// Imports settings from provided instance.
        /// Excludes SchemeSourcesOnCoords.
        /// </summary>
        /// <param name="source"></param>
        internal void Inport(IODescription source)
        {
            this.DisplayText = source.DisplayText;
            this.Description = source.Description;
            this.Order = source.Order;
        }
    }
}
