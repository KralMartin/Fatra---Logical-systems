using System;
using CP_Engine.MapItems;
using CP_Engine.MapItems.Collections;
using Microsoft.Xna.Framework;
using CP_Engine.LoadSaveItems;
using CP_Engine.SchemeItems;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace CP_Engine
{
    /// <summary>
    /// Works like map.
    /// </summary>
    class Scheme
    {
        /// <summary>
        /// Amount of tiles from top to bottom of grid.
        /// </summary>
        internal int TilesCountY { get; private set; }

        /// <summary>
        /// Amount of tiles from left to right of grid.
        /// </summary>
        internal int TilesCountX { get; private set; }

        /// <summary>
        /// Bug, containing this scheme.
        /// </summary>
        internal Bug Bug { get; private set; }

        /// <summary>
        /// Sources placed in this scheme. (Diodes, Konvertors, Inputs, Outputs).
        /// </summary>
        internal SchemeSourceCollection Sources { get; private set; }

        /// <summary>
        /// Paths created in this scheme.
        /// </summary>
        internal SchemePathCollection Paths { get; private set; }

        /// <summary>
        /// Bugs, placed in this Scheme.
        /// </summary>
        internal SchemePlacedBugCollection PlacedBugs { get; private set; }

        internal BreakPointCollection BreakPoints { get; private set; }

        private Tile[,] tiles;      //Grid of tiles.
        private WorkPlace workplace;

        internal Scheme(WorkPlace workplace, int newTilesCountX, int newTilesCountY, Bug bug)
        {
            this.workplace = workplace;
            this.Bug = bug;
            this.Bug.Scheme = this;
            SetSizeAndReset(newTilesCountX, newTilesCountY);
        }

        /// <summary>
        /// Sets size of grid.
        /// And resets this instance to stale like after calling constructor.
        /// </summary>
        /// <param name="newTilesCountX"></param>
        /// <param name="newTilesCountY"></param>
        internal void SetSizeAndReset(int newTilesCountX, int newTilesCountY)
        {
            this.TilesCountX = newTilesCountX;
            this.TilesCountY = newTilesCountY;
            this.tiles = new Tile[newTilesCountX, newTilesCountY];

            this.Sources = new SchemeSourceCollection(workplace, this);
            this.Paths = new SchemePathCollection(workplace, this);
            this.PlacedBugs = new SchemePlacedBugCollection();
            this.BreakPoints = new BreakPointCollection(workplace, this);
        }

        internal void SetTileColor(Point coords, int colorID)
        {
            if (tiles[coords.X, coords.Y] == null)
                tiles[coords.X, coords.Y] = new Tile(new TileData());
            tiles[coords.X, coords.Y].Set_ColorID(colorID);

            //Remove instance if it has no color and no data.
            if (tiles[coords.X, coords.Y].Data.ColorID <= 0 && TilesInfo.IsType0(tiles[coords.X, coords.Y].Data.Type))
                tiles[coords.X, coords.Y] = null;
        }

        /// <summary>
        /// Returns all scheme tiles.
        /// </summary>
        /// <returns></returns>
        internal Tile[,] Get_Tiles()
        {
            return this.tiles;
        }

        internal void CutOff()
        {
            workplace.SchemeEventHistory.StartEvent(this, false);

            int top = TilesCountY;
            int bot = 0;
            int left = TilesCountX;
            int right = 0;
            for (int col = 0; col < this.TilesCountX; col++)
            {
                for (int row = 0; row < this.TilesCountY; row++)
                {
                    if (tiles[col, row] != null)
                    {
                        if (top > row)
                            top = row;
                        if (bot < row)
                            bot = row;
                        if (left > col)
                            left = col;
                        if (right < col)
                            right = col;
                    }
                }
            }
            top--;
            if (top < 0)
                top = 0;
            bot += 2;
            if (bot > TilesCountY)
                bot = TilesCountY;
            left--;
            if (left < 0)
                left = 0;
            right += 2;
            if (right > TilesCountX)
                right = TilesCountX;

            SchemeBak bak = new SchemeBak();
            bak.CreateFromScheme(this);

            //Remove all SSources and PlacedBugs.
            //Removing those elements makes sure, there are no paths left in Scheme.
            this.Sources.Clear();
            List<PlacedBug> pBugs = this.PlacedBugs.GetItems();
            foreach (PlacedBug pBug in pBugs)
                workplace.BugAssistant.Delete(pBug);
            this.Paths.Debug();

            int expandByX = -(left + (TilesCountX - right));
            int expandByY = -(top + (TilesCountY - bot));
            bak.FillScheme(workplace, this, expandByX, expandByY, new Point(-left, -top));

            workplace.SchemeEventHistory.FinalizeEvent();
            workplace.SchemeEventHistory.Reset();
            workplace.CurrentWindow.MoveSceenOffsetBy(new Point());
        }

        /// <summary>
        /// Expands scheme to 
        /// </summary>
        /// <param name="side"></param>
        /// <param name="tiles"></param>
        internal void Expand(int side, int tiles)
        {
            workplace.Simulation.StopThread();

            Tile[,] tmpTiles = this.tiles;

            //Determines how much will be tiles moved to the bot and right.
            int adjustX = 0;
            int adjustY = 0;
            //Determines by how many tiles will be scheme expanded.
            int expandByX = 0;
            int expandByY = 0;

            if (Sides.Top == side)
                adjustY = tiles;
            else if (Sides.Left == side)
                adjustX = tiles;

            if (Sides.IsHorizontal(side))
                expandByX = tiles;
            else
                expandByY = tiles;

            //Create scheme bak.
            SchemeBak bak = new SchemeBak();
            bak.CreateFromScheme(this);
            
            //Remove all SSources and PlacedBugs.
            //Removing those elements makes sure, there are no paths left in Scheme.
            this.Sources.Clear();
            List<PlacedBug> pBugs = this.PlacedBugs.GetItems();
            foreach (PlacedBug pBug in pBugs)
                workplace.BugAssistant.Delete(pBug);
            this.Paths.Debug();


            bak.FillScheme(workplace, this,expandByX, expandByY, new Point(adjustX, adjustY));

            workplace.Simulation.StartThread();
            workplace.SchemeEventHistory.Reset();
            workplace.CurrentWindow.MoveSceenOffsetBy(new Point());
        }
        
        /// <summary>
        /// Returns Tile Data on provided coords.
        /// Coords are not validated.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal TileData Get_TileData(Point coords)
        {
            if (tiles[coords.X, coords.Y] != null)
                return tiles[coords.X, coords.Y].Data;
            return new TileData(0);
        }

        /// <summary>
        /// DO NOT change settings of returned Tile, instead call function Set_TileData.
        /// Returns Tile on provided coords.
        /// Coords are not validated.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal Tile Get_Tile(Point coords)
        {
            if (tiles[coords.X, coords.Y] != null)
                return tiles[coords.X, coords.Y];
            return new Tile(new TileData());
        }

        /// <summary>
        /// Sets tile data at coords.
        /// Also repairds provided data variabiles.
        /// Coords are not validated.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="value"></param>
        internal void Set_TileData(Point coords, TileData value)
        {
            //First destroy paths on original tile.
            DestroyOriginalTile(coords);
            
            //Insert item to Scheme.
            if (tiles[coords.X, coords.Y] != null)
                tiles[coords.X, coords.Y].Set_Data(value);
            else
                tiles[coords.X, coords.Y] = new Tile(value);

            //When provided data is empty tile, tile can be null.
            if (tiles[coords.X, coords.Y].Data.ColorID <= 0 && TilesInfo.IsType0(value.Type))
            {
                tiles[coords.X, coords.Y] = null;
                return;
            }

            //When tile is source type, sources has to be inserted into scheme.
            if (TilesInfo.IsBugType(value.Type)== false)
            {
                TileInfoItem info = TilesInfo.GetItem(value.Type);
                if (info.IsSource())
                {
                    int width;
                    if (info.UsesHorizontal())
                        width = value.HorzWidth;
                    else
                        width = value.VertWidth;
                    Sources.AddSource(coords, width, info.TileType);
                }
            }
        }

        /// <summary>
        /// Called before changing TileData on provided coords.
        /// Removes Paths and SSources on provided coords.
        /// </summary>
        /// <param name="coords"></param>
        private void DestroyOriginalTile(Point coords)
        {
            TileInfoItem info;
            if (tiles[coords.X, coords.Y] != null)
            {
                Tile oldTile = tiles[coords.X, coords.Y];
                if (TilesInfo.IsBugType(oldTile.Data.Type))
                    return;
                info = TilesInfo.GetItem(oldTile.Data.Type);
                //Remove paths.
                this.Paths.Remove(oldTile.Paths);
                //When original tile is Source, remove it from Source collection.
                if (info.IsSource())
                    Sources.Remove(coords);

                //Destroy breakpoints
                if (oldTile.BreakPoint != null)
                {
                    this.BreakPoints.Remove(coords);
                    oldTile.BreakPoint = null;
                }
            }
        }

        /// <summary>
        /// Returns TRUE if provided tile coordinates exists on scheme.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal bool ValidateCoords(Point coords)
        {
            if (coords.X < 0 || coords.X >= TilesCountX || coords.Y < 0 || coords.Y >= TilesCountY)
                return false;
            return true;
        }

        /// <summary>
        /// Returns adjected vire width to provided tile and side.
        /// </summary>
        /// <param name="coords">Base coords.</param>
        /// <param name="side">Direction where you moved from base coords.</param>
        /// <returns></returns>
        internal int GetAdjectedTileWidth(Point coords, int side)
        {
            coords = Sides.Move(coords, side);
            if (ValidateCoords(coords))
            {
                int oppositeSide = Sides.OpositeSide(side);
                TileData data = Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type))
                {
                    PlacedBug pBug = this.PlacedBugs.Get(data.HorzWidth);
                    return pBug.GetWireWidth(coords, oppositeSide);
                }
                TileInfoItem info = TilesInfo.GetItem(data.Type);
                if (info.TileSide[oppositeSide].IsUsed)
                    return data.GetWidth(oppositeSide);
            }
            return 0;
        }

        public override string ToString()
        {
            return this.Bug.Title;
        }
    }
}
