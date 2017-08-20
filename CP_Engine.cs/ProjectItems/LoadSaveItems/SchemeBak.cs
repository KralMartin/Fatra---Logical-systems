using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using CP_Engine.cs.Utilities;
using CP_Engine.MapItems;

namespace CP_Engine.LoadSaveItems
{
    class SchemeBak
    {
        //Bak of Scheme.TielsCountX,Scheme.TilesCountY.
        int tilesCountX, tilesCountY;
        
        //Bak of Scheme.Tiles.
        internal TileData[,] Tiles { get; private set; }

        /// <summary>
        /// Bak of Scheme.PlacedBugs
        /// </summary>
        internal List<PlacedBug> PlacedBugs { get; private set; }

        /// <summary>
        /// Bak of Scheme.Bug.
        /// </summary>
        internal Bug BugBak { get; private set; }

        internal List<BreakPoint> BreakPointsBak { get; private set; }

        internal SchemeBak()
        {
            this.PlacedBugs = new List<PlacedBug>();
            this.BreakPointsBak = new List<BreakPoint>();
        }

        /// <summary>
        /// Creates Bak of provided scheme.
        /// </summary>
        /// <param name="scheme"></param>
        internal void CreateFromScheme(Scheme scheme)
        {
            //Store Bug settings.
            this.BugBak = scheme.Bug.Export();

            //Store scheme grid size.
            this.tilesCountX = scheme.TilesCountX;
            this.tilesCountY = scheme.TilesCountY;

            //Store scheme tiles.
            Tile[,] source = scheme.Get_Tiles();
            this.Tiles = new TileData[scheme.TilesCountX, scheme.TilesCountY];
            for (int row = 0; row < scheme.TilesCountY; row++)
            {
                for (int col = 0; col < scheme.TilesCountX; col++)
                {
                    Point coords = new Point(col, row);
                    TileData data = scheme.Get_TileData(coords);
                    //Ignore tiles of BugTileType.
                    if (TilesInfo.IsBugType(data.Type) == false)
                        this.Tiles[col, row] = data;
                }
            }
            //Store breakPoints.
            this.BreakPointsBak= scheme.BreakPoints.Export();

            //Store placed bugs.
            List<PlacedBug> pBugs = scheme.PlacedBugs.GetItems();
            foreach (PlacedBug pBug in pBugs)
                this.PlacedBugs.Add(pBug.Bak());
        }

        /// <summary>
        /// Fills Scheme using data from this instance.
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="scheme">Scheme, that will be filled with data.</param>
        /// <param name="bugIdRemap">Remap of Bug ID.</param>
        internal void FillScheme(WorkPlace workplace, Scheme scheme, Dictionary<int, int> bugIdRemap)
        {
            FillScheme_Private(workplace, scheme, bugIdRemap, 0, 0, new Point());
        }

        internal void FillScheme(WorkPlace workplace, Scheme scheme, int expandByX, int expandByY, Point offset)
        {
            FillScheme_Private(workplace, scheme, new Dictionary<int, int>(), expandByX, expandByY, offset);
        }

        /// <summary>
        /// Fills Scheme using data from this instance.
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="scheme">Scheme, that will be filled with data.</param>
        /// <param name="bugIdRemap">Remap of Bug ID.</param>
        /// <param name="expandByX">Make new scheme bigger by this amount of tiles.</param>
        /// <param name="expandByY">Make new scheme bigger by this amount of tiles.</param>
        /// <param name="offset"></param>
        private void FillScheme_Private(WorkPlace workplace, Scheme scheme, Dictionary<int, int> bugIdRemap, int expandByX, int expandByY, Point offset)
        {
            Repair repair = new Repair(workplace, null);

            //Set tiles. 
            Point coords;
            scheme.SetSizeAndReset(this.tilesCountX + expandByX, this.tilesCountY + expandByY);
            for (int row = 0; row < this.tilesCountY; row++)
            {
                for (int col = 0; col < this.tilesCountX; col++)
                {
                    if (TilesInfo.IsType0(Tiles[col, row].Type) == false || Tiles[col, row].ColorID > 0)
                    {
                        coords = new Point(col, row) + offset;
                        scheme.Set_TileData(coords, Tiles[col, row]);
                    }
                    else
                    {
                        TileData d = Tiles[col, row];
                    }
                }
            }
            //Create breakPoints.
            scheme.BreakPoints.Import(this.BreakPointsBak);

            //Create PBugs.
            foreach (PlacedBug current in this.PlacedBugs)
            {
                //Get remmaped Bug id.
                //ID might not be remaped, because it is SpecialBug(SpecialBugs are not stored into files).
                int newBugID = current.BugID;
                if (bugIdRemap.ContainsKey(newBugID))
                    newBugID = bugIdRemap[newBugID];
                //Get Bug.
                Bug bug = workplace.Project.Bugs.GetItem(newBugID);
                //Create PBug.
                PlacedBug pbug = workplace.BugAssistant.Insert(bug, current.Coords + offset, scheme, repair);
                pbug.Description = current.Description;
                pbug.Order = current.Order;
                pbug.Number = current.Number;
            }
            //ReOrder items
            scheme.PlacedBugs.ReOrder();
            //Set Bug properties.
            scheme.Bug.Inport(this.BugBak, offset);
        }
        
        #region XML
        internal void Save(XmlWriter xml)
        {
            //Save tiles.
            xml.WriteStartElement(XML.Element1);
            xml.WriteAttributeString(XML.Param1, this.tilesCountX.ToString());
            xml.WriteAttributeString(XML.Param2, this.tilesCountY.ToString());

            for (int row = 0; row < this.tilesCountY; row++)
            {
                for (int col = 0; col < this.tilesCountX; col++)
                {
                    if (TilesInfo.IsType0( Tiles[col, row].Type) == false || Tiles[col,row].ColorID > 0)
                    {
                        //Ignore empty tiles with no color.
                        xml.WriteStartElement(XML.Item);
                        Tiles[col, row].Save(xml, row, col);
                        xml.WriteEndElement();
                    }
                }
            }
            xml.WriteEndElement();

            //Save BreakPoints.
            if (BreakPointsBak.Count > 0)
            {
                xml.WriteStartElement(XML.Element5);
                foreach (BreakPoint bp in this.BreakPointsBak)
                {
                    xml.WriteStartElement(XML.Item);
                    bp.Save(xml);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            //Save Placed Bugs.
            if (this.PlacedBugs.Count > 0)
            {
                xml.WriteStartElement(XML.Element2);
                foreach (PlacedBug pBugBak in this.PlacedBugs)
                {
                    xml.WriteStartElement(XML.Item);
                    pBugBak.Save(xml);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }

            //Save Bug properties.
            xml.WriteStartElement(XML.Element3);
            xml.WriteAttributeString(XML.Param1, BugBak.ID.ToString());
            xml.WriteAttributeString(XML.Param2, BugBak.ColorID.ToString());
            if (string.IsNullOrEmpty(BugBak.Title) == false)
                xml.WriteElementString(XML.Element1, BugBak.Title);
            if (string.IsNullOrEmpty(BugBak.Description) == false)
                xml.WriteElementString(XML.Element2, BugBak.Description);
            if (string.IsNullOrEmpty(BugBak.DisplayTextContertor.Text) == false)
                xml.WriteElementString(XML.Element3, BugBak.DisplayTextContertor.Text);

            //Save Bug IODescription
            xml.WriteStartElement(XML.Element4);
            BugBak.IODecription.Save(xml);
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        internal void Load(XmlReader xml)
        {
            //Read tiles.
            if (xml.IsStartElement(XML.Element1))
            {
                this.tilesCountX = Convert.ToInt32(xml.GetAttribute(XML.Param1));
                this.tilesCountY = Convert.ToInt32(xml.GetAttribute(XML.Param2));
                this.Tiles = new TileData[tilesCountX, tilesCountY];

                xml.Read();
                while (xml.IsStartElement(XML.Item))
                {
                    TileData data = new TileData();
                    Point coords = data.Load(xml);
                    Tiles[coords.X, coords.Y] = data;
                    xml.Read();
                }
                xml.ReadEndElement();
            }
            //Read BreakPoints.
            if (xml.IsStartElement(XML.Element5))
            {
                xml.Read();
                while (true)
                {
                    if (xml.IsStartElement(XML.Item))
                    {
                        this.BreakPointsBak.Add(new BreakPoint(xml));
                    }
                    else
                        break;
                }
                xml.ReadEndElement();
            }
            //Read Placed Bugs.
            if (xml.IsStartElement(XML.Element2))
            {
                xml.Read();
                while (true)
                {
                    if (xml.IsStartElement(XML.Item))
                    {
                        this.PlacedBugs.Add(new PlacedBug(xml));
                    }
                    else
                        break;
                }
                xml.ReadEndElement();
            }
            //Read Bug properties.
            if (xml.IsStartElement(XML.Element3))
            {
                this.BugBak = new Bug(Convert.ToInt32(xml.GetAttribute(XML.Param1)));
                this.BugBak.ColorID = Convert.ToInt32(xml.GetAttribute(XML.Param2));
                xml.Read();
                //Bug.Title
                if (xml.IsStartElement(XML.Element1))
                {
                    xml.Read();
                    this.BugBak.Title = xml.Value;
                    xml.Read();
                    xml.ReadEndElement();
                }
                //Bug.Description
                if (xml.IsStartElement(XML.Element2))
                {
                    xml.Read();
                    this.BugBak.Description = xml.Value;
                    xml.Read();
                    xml.ReadEndElement();
                }
                //Bug.DisplayText
                if (xml.IsStartElement(XML.Element3))
                {
                    xml.Read();
                    this.BugBak.DisplayTextContertor.Text = xml.Value;
                    xml.Read();
                    xml.ReadEndElement();
                }
                if (xml.IsStartElement(XML.Element4))
                {
                    xml.Read();
                    BugBak.IODecription.Load(xml);
                    xml.ReadEndElement();
                }

                xml.ReadEndElement();
            }
        }

        #endregion
    }
}
