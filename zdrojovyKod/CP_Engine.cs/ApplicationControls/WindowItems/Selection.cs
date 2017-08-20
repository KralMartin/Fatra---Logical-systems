using CP_Engine.MapItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace CP_Engine
{
    public class Selection
    {
        /// <summary>
        /// HashSet of all selected items.
        /// </summary>
        public HashSet<Point> Items { get; private set; }

        /// <summary>
        /// Only changes color of scheme.
        /// </summary>
        internal bool IsValid { get; set; }

        Window window;
        List<Point> itemsBak;

        internal Selection(Window window)
        {
            this.window = window;
            this.Items = new HashSet<Point>();
            itemsBak = new List<Point>();
            this.IsValid = true;
        }

        /// <summary>
        /// When all selected tiles shares same tiletype, return their tiletype, else return -1.
        /// </summary>
        /// <returns></returns>
        internal int GetSelectedTilesType()
        {
            TileData data;
            int toReturn = -1;
            foreach (Point coords in Items)
            {
                data = window.Scheme.Get_TileData(coords);
                if (toReturn == -1)
                    toReturn = data.Type;
                else if (data.Type != toReturn)
                    return -1;
            }
            return toReturn;
        }

        /// <summary>
        /// Returns smallest Y value and smallest X value of selected tiels.
        /// </summary>
        /// <returns></returns>
        internal Point MostTopLeftCoord()
        {
            List<Point> list = Items.ToList();
            int minY = int.MaxValue;
            int minX = int.MaxValue;
            foreach (Point coords in list)
            {
                if (coords.X < minX)
                    minX = coords.X;
                if (coords.Y < minY)
                    minY = coords.Y;
            }
            return new Point(minX, minY);
        }

        /// <summary>
        /// Add point at global position to selection.
        /// </summary>
        /// <param name="globalPosition"></param>
        internal void SelectPointAt(Point globalPosition)
        {
            globalPosition = window.GetTileAt(globalPosition);
            if (globalPosition.X >= 0)
                this.Items.Add(globalPosition);
        }
        
        internal void StartRectangularSelection(Point tileCoordsA, Point tileCoordsB)
        {
            itemsBak.Clear();
            foreach (Point coords in Items)
                itemsBak.Add(coords);
            MouseMoveRectangularSelection(tileCoordsA, tileCoordsB);
        }
        
        /// <summary>
        /// Add points in rectangular selecton.
        /// </summary>
        /// <param name="tileCoordsA"></param>
        /// <param name="tileCoordsB"></param>
        internal void MouseMoveRectangularSelection(Point tileCoordsA, Point tileCoordsB)
        {
            this.Items.Clear();
            foreach (Point coords in itemsBak)
                Items.Add(coords);

            int minX, maxX;
            if (tileCoordsA.X < tileCoordsB.X)
            {
                minX = tileCoordsA.X;
                maxX = tileCoordsB.X;
            }
            else
            {
                minX = tileCoordsB.X;
                maxX = tileCoordsA.X;
            }
            int minY, maxY;
            if (tileCoordsA.Y < tileCoordsB.Y)
            {
                minY = tileCoordsA.Y;
                maxY = tileCoordsB.Y;
            }
            else
            {
                minY = tileCoordsB.Y;
                maxY = tileCoordsA.Y;
            }
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                    this.Items.Add(new Point(x, y));
            }
        }
    }
}
