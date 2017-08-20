using CP_Engine;
using CP_Engine.cs.Utilities;
using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CP_Engine
{
    class BreakPoint
    {
        internal Point Coords { get; private set; }
        internal int ID { get; private set; }
        
        internal BreakPoint(int ID, Point coords)
        {
            this.ID = ID;
            this.Coords = coords;
        }

        public BreakPoint(XmlReader xml)
        {
            Point point = new Point();
            point.X = Convert.ToInt32(xml.GetAttribute(XML.Param1));
            point.Y = Convert.ToInt32(xml.GetAttribute(XML.Param2));
            this.Coords = point;
            xml.Read();
        }

        internal void Check(Scheme scheme, List<PhysScheme> pSchemes, List<BreakPointResult> results)
        {
            Tile tile = scheme.Get_Tile(Coords);
            BreakPointResult result = null;
            foreach (PhysScheme pScheme in pSchemes)
            {
                bool[] oldValue = pScheme.BreakPoints[this.ID];
                bool[] newValue = tile.GetValues(pScheme);
                if (CompareValues(oldValue, newValue) == false)
                {
                    pScheme.BreakPoints[this.ID] = newValue;
                    if (result == null)
                        result = new BreakPointResult(this);

                    BreakPointOccourance occourance = new BreakPointOccourance();
                    occourance.PhysScheme = pScheme;
                    occourance.OldValues = oldValue;
                    occourance.NewValues = newValue;
                    result.BreakOccourances.Add(occourance);
                }
            }
            if (result != null)
                results.Add(result);
        }

        private bool CompareValues(bool[] valA, bool[] valB)
        {
            for (int i = 0; i < valA.Length; i++)
            {
                if (valA[i] != valB[i])
                    return false;
            }
            return true;
        }

        internal void Save(XmlWriter xml)
        {
            xml.WriteAttributeString(XML.Param1, this.Coords.X.ToString());
            xml.WriteAttributeString(XML.Param2, this.Coords.Y.ToString());
        }
    }
}
