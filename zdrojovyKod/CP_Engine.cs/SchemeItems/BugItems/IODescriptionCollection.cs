using CP_Engine.cs.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CP_Engine.BugItems
{
    /// <summary>
    /// Collction of IODescription.
    /// </summary>
    class IODescriptionCollection
    {
        List<IODescription> items;

        internal IODescriptionCollection()
        {
            items = new List<IODescription>();
        }

        /// <summary>
        /// When IOput was removed from scheme, call this function.
        /// Remove IODescription on provided coords. 
        /// </summary>
        /// <param name="coords"></param>
        internal void Remove(Point coords)
        {
            //Remove IODescription on provided coords. 
            IODescription toRemove = Get_Description(coords);
            items.Remove(toRemove);
            //Repair order of remaining IOputs.
            if (toRemove.IsInput)
                ReorderList(Get_Inputs());
            else
                ReorderList(Get_Outputs());
        }

        /// <summary>
        /// Returns IODescription on provided coords.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal IODescription Get_Description(Point coords)
        {
            foreach (IODescription item in items)
            {
                if (item.Coords == coords)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Return ordered list of Outputs.
        /// </summary>
        /// <returns></returns>
        internal List<IODescription> Get_Outputs()
        {
            return GetItems(false);
        }

        /// <summary>
        /// Return ordered list of Inputs.
        /// </summary>
        /// <returns></returns>
        internal List<IODescription> Get_Inputs()
        {
            return GetItems(true);
        }

        /// <summary>
        /// Returns ordered list of Inputs or Outputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private List<IODescription> GetItems(bool inputs)
        {
            List<IODescription> toReturn = new List<IODescription>();
            foreach (IODescription item in items)
            {
                if (item.IsInput == inputs)
                    toReturn.Add(item);
            }
            toReturn = toReturn.OrderBy(x => x.Order).ToList();
            return toReturn;
        }

        /// <summary>
        /// Adds IODescription into this collection.
        /// </summary>
        /// <param name="value"></param>
        internal void Add(IODescription value)
        {
            items.Add(value);
        }

        /// <summary>
        /// Sets IODescription.Order property of provided IODescriptions list in order.
        /// </summary>
        /// <param name="itemList"></param>
        private void ReorderList(List<IODescription> itemList)
        {
            itemList = itemList.OrderBy(x => x.Order).ToList();
            int index = 0;
            foreach (IODescription item in itemList)
                item.Order = index++;
        }

        /// <summary>
        /// Imports settings from provided IODescriptionCollection.
        /// </summary>
        /// <param name="source"></param>
        internal void Inport(IODescriptionCollection source, Point offset)
        {
            foreach (IODescription desc in source.items)
            {
                IODescription target = Get_Description(desc.Coords + offset);
                target.Inport(desc);
            }
        }

        /// <summary>
        /// Creates coppy of this instance.
        /// </summary>
        /// <returns></returns>
        internal IODescriptionCollection Export()
        {
            IODescriptionCollection toReturn = new IODescriptionCollection();
            foreach (IODescription desc in items)
                toReturn.Add(desc.Export());
            return toReturn;
        }

        #region XML
        internal void Save(XmlWriter xml)
        {
            foreach (IODescription item in items)
            {
                xml.WriteStartElement(XML.Item);
                item.Save(xml);
                xml.WriteEndElement();
            }
        }

        internal void Load(XmlReader xml)
        {
            while (true)
            {
                if (xml.IsStartElement(XML.Item))
                {
                    IODescription item = new IODescription(xml);
                    items.Add(item);
                }
                else
                    break;
            }
        }
        #endregion
    }
}
