using CP_Engine.BugItems;
using CP_Engine.cs.Utilities;
using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace CP_Engine
{
    /// <summary>
    /// Describes additional information about Bug placed within another Scheme.
    /// </summary>
    class PlacedBug
    {
        /// <summary>
        /// Order number of PBug. 
        /// When value is smaller than 0, PBug is not in order.
        /// </summary>
        internal int Order { get; set; }
        
        /// <summary>
        /// Order number of PBug. 
        /// When value is smaller than 0, PBug is not in order.
        /// </summary>
        internal int Number { get; set; }

        /// <summary>
        /// Description, shown in bottom bar.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        internal int ID { get; private set; }

        /// <summary>
        /// Coords of top left tile of this PBug.
        /// </summary>
        internal Point Coords { get; private set; }

        /// <summary>
        /// This variabile is used only to preserve information in bak-state(while getting loaded or saved...).
        /// </summary>
        internal int BugID { get; private set; }

        /// <summary>
        /// Bug, that is described by this instance.
        /// </summary>
        internal Bug Bug { get; private set; }

        /// <summary>
        /// Scheme, in which is Bug placed.
        /// </summary>
        internal Scheme ParentScheme { get; private set; }

        /// <summary>
        /// OuterSchemeSources that are part of this PBug.
        /// KEY: InnerSchemeSource Identifier, VALUE: SSouce
        /// </summary>
        internal Dictionary<int, SchemeSource> SchemeSources { get; private set; }

        //inputs and outputs variabiles together contains same SSources as Dictionary SchemeSources
        CollectionOfOuterSchemeSourceCollection inputs;
        CollectionOfOuterSchemeSourceCollection outputs;

        internal PlacedBug(int id, Point coords, Bug bug, Scheme parentScheme)
        {
            this.ID = id;
            this.Bug = bug;
            this.Coords = coords;
            this.ParentScheme = parentScheme;
            this.Order = -1;
            this.BugID = bug.ID;
        }

        #region XML

        internal PlacedBug(XmlReader xml)
        {
            int x = Convert.ToInt32(xml.GetAttribute(XML.Param2));
            int y = Convert.ToInt32(xml.GetAttribute(XML.Param3));

            this.BugID = Convert.ToInt32(xml.GetAttribute(XML.Param1));
            this.Coords = new Point(x, y);
            this.Order = Convert.ToInt32(xml.GetAttribute(XML.Param4));
            string str = xml.GetAttribute(XML.Param5);
            if(str!=null)
                this.Number = Convert.ToInt32(str);

            if (xml.IsEmptyElement == false)
            {
                xml.Read();
                this.Description = xml.Value;
                xml.Read();
                xml.ReadEndElement();
            }
            else
                xml.Read();
        }

        internal PlacedBug Bak()
        {
            PlacedBug toReturn = new PlacedBug(this.ID, this.Coords, this.Bug, this.ParentScheme);
            toReturn.Description = this.Description;
            toReturn.Order = this.Order;
            toReturn.Number = this.Number;
            return toReturn;
        }

        internal void Save(XmlWriter xml)
        {
            xml.WriteAttributeString(XML.Param2, this.Coords.X.ToString());
            xml.WriteAttributeString(XML.Param3, this.Coords.Y.ToString());
            xml.WriteAttributeString(XML.Param1, this.BugID.ToString());
            xml.WriteAttributeString(XML.Param4, this.Order.ToString());
            xml.WriteAttributeString(XML.Param5, this.Number.ToString());
            xml.WriteValue(this.Description);
        }

        #endregion


        internal string GetDescription(Point coords)
        {
            //Bug description.
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Bug.Title);
            if (string.IsNullOrEmpty(this.Description) == false)
            {
                sb.Append(":\n");
                sb.Append(this.Description);
            }
            else if (string.IsNullOrEmpty(this.Bug.Description) == false)
            {
                sb.Append(":\n");
                sb.Append(this.Bug.Description);
            }

            //Input/Output description.
            //Get OuterSchemeSource.
            int index = coords.X - this.Coords.X;
            OuterSchemeSource sSource;
            if (coords.Y == this.Coords.Y)
                sSource = this.inputs.GetSchemeSource(0, index);
            else
                sSource = this.outputs.GetSchemeSource(0, index);

            //Get InnerSchemeSource description.
            if (sSource != null)
            {
                IODescription desc = sSource.GetInnerSourceDescription();
                sb.Append("\n");
                sb.Append(desc.GetDescription());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Not Safe (You have to provide coords, that intersects with this PBug)!
        /// Returns wire width on provided tile and its side.
        /// </summary>
        /// <param name="coords">Coors of tile.</param>
        /// <param name="side">Side of tile.</param>
        /// <returns></returns>
        internal int GetWireWidth(Point coords, int side)
        {
            //IOput is only on top or bottom side of PBUg.
            if (Sides.IsHorizontal(side))
                return -1;
            int index = coords.X - this.Coords.X;
            if (this.Coords.Y == coords.Y)
                return this.inputs.GetVireWidth(index);
            else if (this.Coords.Y + 1 == coords.Y)
                return this.outputs.GetVireWidth(index);
            else
                throw new Exception("zly index");
        }

        /// <summary>
        /// Return current count of tiles from left to right side fo bug.
        /// Calling this.GetBugWidth do not have to be same as calling this.Bug.GetBugWidth.
        /// </summary>
        /// <returns></returns>
        internal int GetBugWidth()
        {
            return Math.Max(2, Math.Max(this.inputs.GetItemsLenght(), this.outputs.GetItemsLenght()));
        }

        /// <summary>
        /// Adds OuterSchemeSources into provided path.
        /// </summary>
        /// <param name="path">Path, that will be filled with data.</param>
        /// <param name="exactPosition">Position from where to fill path.</param>
        /// <param name="side">Side of tile.</param>
        internal void AddToPath(SchemePath path, ExactGridPosition exactPosition, int side)
        {
            if (Sides.IsHorizontal(side))
                return;
            int index = exactPosition.Coords.X - this.Coords.X;
            if (index < 0)
                return;
            if (side == Sides.Top)
            {
                //Get SSource on provided coords and floor.
                SchemeSource sSource = this.inputs.GetSchemeSource(exactPosition.Floor, index);
                if (sSource != null)
                {
                    //Insert it into path.
                    path.Outputs.Add(sSource);
                    sSource.IsOutputIn = path;
                }
            }
            else
            {
                SchemeSource sSource = this.outputs.GetSchemeSource(exactPosition.Floor, index);
                if (sSource != null)
                {
                    path.Inputs.Add(sSource);
                    sSource.IsInputIn = path;
                }
            }
        }

        /// <summary>
        /// Destroys paths on provided coords.
        /// </summary>
        /// <param name="coords">Coords where to destroy paths</param>
        /// <param name="side">Side of tile.</param>
        internal void DestroyPaths(Point coords, int side)
        {
            if (Sides.IsHorizontal(side))
                return;
            int index = coords.X - this.Coords.X;
            if (index < 0)
                return;
            this.inputs.DestroyPaths(index, true, this.ParentScheme);
            this.outputs.DestroyPaths(index, false, this.ParentScheme);
        }

        /// <summary>
        /// Create OuterSchemeSources.
        /// Initialization function. 
        /// It is not part of constructor, because you can create PBug only for Bak purposes.
        /// </summary>
        internal void CreateSchemeSources()
        {
            SchemeSources = new Dictionary<int, SchemeSource>();
            inputs = new CollectionOfOuterSchemeSourceCollection(this, true);
            outputs = new CollectionOfOuterSchemeSourceCollection(this, false);
        }

        /// <summary>
        /// Destroys all paths, that contain any of  OuterSchemeSources of this instance.
        /// </summary>
        internal void RemoveSchemeSources()
        {
            inputs.DestroySchemeSources(this, true);
            outputs.DestroySchemeSources(this, false);
        }

        /// <summary>
        /// Creates phys representation of this instance.
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="parentPScheme">Parent scheme, that will be filled with data.</param>
        internal void CreatePhysRepre(WorkPlace workplace, PhysScheme parentPScheme)
        {
            if (this.Bug.Scheme != null)
            {
                //User created Bug (scheme is not null).
                //Create new PScheme.
                PhysScheme pScheme = new PhysScheme(this);
                //Acknowledge Project SchemeStructure, that new scheme was created.
                workplace.Project.SchemeStructure.PSchemeCreated(pScheme);
                //Parent scheme might be null, when this is TopScheme.
                if (parentPScheme != null)
                    parentPScheme.Children.Add(this.ID, pScheme);
                //Set parent scheme.
                pScheme.ParentPScheme = parentPScheme;
                //Fill newly created pScheme with data.
                this.Bug.Scheme.Sources.CreatePhysRepresentation(pScheme);
                this.Bug.Scheme.Paths.CreatePhysRepresentation(workplace, pScheme);
                this.Bug.Scheme.PlacedBugs.CreatePhysRepresentation(workplace, pScheme);
                this.Bug.Scheme.BreakPoints.CreatePhysRepresentation(workplace, pScheme);
            }
            else
            {
                //Special Bug (scheme is null).
                SpecialPhysScheme pScheme = new SpecialPhysScheme(this, parentPScheme);
                this.Bug.SpecialPhysSchemeCreated(pScheme, workplace.Simulation);
                parentPScheme.SpecialChildren.Add(this.ID, pScheme);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workplace"></param>
        internal void RemoveAllPhysRepres(WorkPlace workplace)
        {
            if (this.Bug.Scheme != null)
            {
                //User created Bug (scheme is not null).
                if (this.ParentScheme != null)
                {
                    //Parent scheme is not null, this is not TopScheme.
                    //Get PSchemes of parent scheme.
                    List<PhysScheme> parentPSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.ParentScheme);
                    List<PhysScheme> toRemove = new List<PhysScheme>();
                    foreach (PhysScheme parentPScheme in parentPSchemes)
                    {
                        //Get PScheme represented by this PBug, it PBug parent scheme.
                        PhysScheme pScheme = parentPScheme.Children[this.ID];
                        //It will be later removed.
                        toRemove.Add(pScheme);
                        //Remove it from parent.
                        parentPScheme.Children.Remove(this.ID);
                    }
                    //Remove PSchemes from project SchemeStructure.
                    foreach (PhysScheme pScheme in toRemove)
                        workplace.Project.SchemeStructure.PSchemeRemoved(pScheme);
                }
                else
                {
                    //Parent scheme is null, when this is TopScheme.
                    //Find and remove PScheme representating this PBug. 
                    //There is only one PScheme, because this PBug is TopScheme. 
                    List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.Bug.Scheme);
                    if (pSchemes.Count != 1)
                    {
                        Debugger.Break();
                    }
                    workplace.Project.SchemeStructure.PSchemeRemoved(pSchemes[0]);
                }
            }
            else
            {
                //Special Bug (scheme is null).
                //Get PSchemes of parent scheme.
                List<PhysScheme> parentPSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.ParentScheme);
                foreach (PhysScheme parentPScheme in parentPSchemes)
                {
                    //Find SpecialChild representating this PBug and remove it from parent scheme.
                    parentPScheme.SpecialChildren[this.ID].NoLongerInUse = true;
                    parentPScheme.SpecialChildren.Remove(this.ID);
                }

            }
        }
    }
}
