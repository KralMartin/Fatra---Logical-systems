using System;
using System.Xml;
using CP_Engine.cs.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CP_Engine.MapItems
{
    struct TileData
    {
        internal int Type { get; set; }
        internal int Offset { get; set; }
        internal int HorzWidth { get; set; }
        internal int VertWidth { get; set; }
        internal int ColorID { get; set; }

        internal TileData(int type)
        {
            this.Type = type;
            this.Offset = 0;
            this.ColorID = 0;
            HorzWidth = 0;
            VertWidth = 0;
        }

        /// <summary>
        /// TRUE: other Instances share same values for Type, Offset, HorzWidth, VertWidth properties.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal bool MyEquals(TileData other)
        {
            if (this.Type != other.Type)
                return false;
            if (this.Offset != other.Offset)
                return false;
            if (this.HorzWidth != other.HorzWidth)
                return false;
            if (this.VertWidth != other.VertWidth)
                return false;
            return true;
        }

        /// <summary>
        /// Returns wire-width on provided side.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        internal int GetWidth(int side)
        {
            if (side == Sides.Top || side == Sides.Bot)
                return VertWidth;
            return HorzWidth;
        }

        /// <summary>
        /// Repairs tile data.
        /// </summary>
        internal void Repair()
        {
            TileInfoItem info = TilesInfo.GetItem(this.Type);

            //When 4 way tile, do not have same horz and vert width, it cannot be connected.
            if (TilesInfo.IsType12(this.Type) && HorzWidth != VertWidth)
                HorzWidth = HorzWidth;

            //When tile do not uses side, its width has to be set to 0.
            if (info.UsesHorizontal() == false)
                HorzWidth = 0;
            if (info.UsesVertical() == false)
                VertWidth = 0;

            //When tile is not composed, offset has to be set to 0.
            RepairOffset(info);

            //Tile type 7(x shaped without connected vires) Offset has to be set to value of one of its vire-widths. 
            if (TilesInfo.IsType7(this.Type))
            {
                if (info.OffsetHorizontal)
                    this.Offset = this.VertWidth;
                else
                    this.Offset = this.HorzWidth;
            }
        }
        
        /// <summary>
        /// Repair Offset value.
        /// </summary>
        /// <param name="info"></param>
        private void RepairOffset(TileInfoItem info)
        {
            if (info.IsComposed() == false)
            {
                Offset = 0;
                return;
            }
            //Vire width of side, that is using offset.
            int widthUsingOffset = 0;
            //Vire width of side, that is NOT using offset.
            int staticWidth = 0;
            if (info.OffsetHorizontal)
            {
                widthUsingOffset = this.HorzWidth;
                staticWidth = this.VertWidth;
            }
            else
            {
                widthUsingOffset = this.VertWidth;
                staticWidth = this.HorzWidth;
            }
            //Offset has it's max and min value.
            if (this.Offset + widthUsingOffset <= 0)
                this.Offset = 1 - widthUsingOffset;
            else if (this.Offset >= staticWidth)
                this.Offset = staticWidth - 1;
        }

        #region XML
        internal void Save(XmlWriter xml, int row, int col)
        {
            xml.WriteAttributeString(XML.Element1, row.ToString());
            xml.WriteAttributeString(XML.Element2, col.ToString());

            xml.WriteAttributeString(XML.Param1, this.Type.ToString());
            xml.WriteAttributeString(XML.Param2, this.VertWidth.ToString());
            xml.WriteAttributeString(XML.Param3, this.HorzWidth.ToString());
            xml.WriteAttributeString(XML.Param4, this.Offset.ToString());
            xml.WriteAttributeString(XML.Param5, this.ColorID.ToString());
        }

        internal Point Load(XmlReader xml)
        {
            Type = Convert.ToInt32(xml.GetAttribute(XML.Param1));
            VertWidth = Convert.ToInt32(xml.GetAttribute(XML.Param2));
            HorzWidth = Convert.ToInt32(xml.GetAttribute(XML.Param3));
            Offset = Convert.ToInt32(xml.GetAttribute(XML.Param4));
            ColorID = Convert.ToInt32(xml.GetAttribute(XML.Param5));
            return new Point(Convert.ToInt32(xml.GetAttribute(XML.Element2)), Convert.ToInt32(xml.GetAttribute(XML.Element1)));
        }
        #endregion        
    }
}
