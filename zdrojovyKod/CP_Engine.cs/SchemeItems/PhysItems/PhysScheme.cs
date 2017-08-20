using CP_Engine.MapItems;
using System.Collections.Generic;
using System;
using System.Text;

namespace CP_Engine.SchemeItems
{
    class PhysScheme
    {
        /// <summary>
        /// PBug represented by this instance.
        /// </summary>
        internal PlacedBug PlacedBug { get; private set; }

        /// <summary>
        /// Parent PScheme to this instance.
        /// </summary>
        internal PhysScheme ParentPScheme { get; set; }

        /// <summary>
        /// Child PhysSchemes.
        /// </summary>
        internal Dictionary<int, PhysScheme> Children { get; private set; }

        /// <summary>
        /// Child SpecialPhysSchemes.
        /// </summary>
        internal Dictionary<int, SpecialPhysScheme> SpecialChildren { get; private set; }

        /// <summary>
        /// Collecion of PhysSources.
        /// </summary>
        internal Dictionary<int, PhysSource> Sources { get; private set; }

        /// <summary>
        /// Collection of PhysPaths.
        /// </summary>
        internal Dictionary<int, PhysPath> Paths { get; private set; }

        internal Dictionary<int, bool[]> BreakPoints { get; private set; }

        internal PhysScheme(PlacedBug placedBug)
        {
            this.PlacedBug = placedBug;
            this.Sources = new Dictionary<int, PhysSource>();
            this.Paths = new Dictionary<int, PhysPath>();
            this.Children = new Dictionary<int, PhysScheme>();
            this.SpecialChildren = new Dictionary<int, SpecialPhysScheme>();
            this.BreakPoints = new Dictionary<int, bool[]>();
        }
        
        /// <summary>
        /// Returns combined value of horizontal paths stored in provided tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        internal bool GetHorizontalValue(Tile tile)
        {
            TileInfoItem info = TilesInfo.GetItem(tile.Data.Type);
            if (info.UsesHorizontal() == false)
                return false;
            int min = 0;
            if (info.OffsetHorizontal)
                min = tile.Data.Offset;

            if (tile.Data.Offset < 0)
                min -= tile.Data.Offset;

            int max = tile.Data.HorzWidth + min;
            for (int i = min; i < max; i++)
            {
                if (tile.Paths[i] != null && tile.Paths[i].GetValue(this))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns combined value of vertical paths stored in provided tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        internal bool GetVerticalValue(Tile tile)
        {
            TileInfoItem info = TilesInfo.GetItem(tile.Data.Type);
            if (info.UsesVertical() == false)
                return false;
            int min = 0;
            if (info.OffsetVertical)
                min = tile.Data.Offset;

            if (tile.Data.Offset < 0)
                min -= tile.Data.Offset;

            int max = tile.Data.VertWidth + min;
            for (int i = min; i < max; i++)
            {
                if (tile.Paths[i] != null && tile.Paths[i].GetValue(this))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Returns string visible for user, that describes how to get into this instance.
        /// </summary>
        /// <returns></returns>
        internal string GetPath()
        {
            Stack<string> stack = new Stack<string>();
            PhysScheme current = this;
            while (current != null)
            {
                stack.Push(current.PlacedBug.Bug.Title);
                current = current.ParentPScheme;
            }

            string toReturn = null;
            while (stack.Count > 0)
            {
                if (toReturn == null)
                    toReturn = stack.Pop();
                else
                    toReturn += "/" + stack.Pop();
            }
            return toReturn;
        }
    }
}
