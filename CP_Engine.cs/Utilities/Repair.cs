using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CP_Engine
{
    /// <summary>
    /// Repairs Scheme.
    /// </summary>
    class Repair
    {
        enum RemoveOptions
        {
            /// <summary>
            /// Side can be removed. 
            /// </summary>
            Can,
            /// <summary>
            /// Side has to be removed.
            /// </summary>
            Must,
            /// <summary>
            /// Sides can not be removed.
            /// </summary>
            CanNot
        }
        Scheme scheme;
        WorkPlace workplace;

        HashSet<Point> coordsHash;      //Coords of TileData, that have been modified by outer classes.
        List<Point> coordsList;         //Coords of TileData, that have been modified by outer classes.

        internal Repair(WorkPlace workplace, Scheme scheme)
        {
            this.workplace = workplace;
            this.scheme = scheme;
            this.coordsHash = new HashSet<Point>();
            this.coordsList = new List<Point>();
        }

        /// <summary>
        /// Add coords to this instance, if it does not contain same coords allraedy.
        /// Call this function when you changed tile at provided coords.
        /// </summary>
        /// <param name="coords"></param>
        internal void Add(Point coords)
        {
            if (this.coordsHash.Contains(coords) == false)
            {
                coordsHash.Add(coords);
                coordsList.Add(coords);
            }
        }

        #region inner

        /// <summary>
        /// Repairs tiles at coords, that you provided by executing Add function.
        /// </summary>
        internal void RepairInner()
        {
            bool wasRepaired;
            TileData data;
            TileInfoItem info;

            foreach (Point coords in coordsList)
            {                
                //Get data and info.
                data = scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type))
                    continue;
                info = TilesInfo.GetItem(data.Type);
                int infoSides = info.CountOfUsedSides();

                //Try to remove unnecessary sides of tile.
                wasRepaired = false;
                TileInfoItem repaired = new TileInfoItem();
                for (int i = 0; i < 4; i++)
                {
                    if (info.TileSide[i].IsUsed)
                    {
                        RemoveOptions rOption = ShouldRemoveSide(coords, i, data.GetWidth(i));
                        if (infoSides <= 2)
                        {
                            if (rOption != RemoveOptions.Must)
                                repaired.TileSide[i] = new TileSide(true);
                        }
                        else
                        {
                            if (rOption == RemoveOptions.CanNot)
                                repaired.TileSide[i] = new TileSide(true);
                        }
                    }
                    if (info.TileSide[i].IsUsed != repaired.TileSide[i].IsUsed)
                        wasRepaired = true;
                }
                if (wasRepaired)
                {
                    if (infoSides > 2 && repaired.CountOfUsedSides() <= 2 && data.HorzWidth != data.VertWidth)
                    {
                        //First I has T or X shaped tile.
                        //I created L shaped tile.
                        //When first tile used difrent vire-widths on horz and vert side, i would have L shaped tile with difrent vire-widths
                        //Such tile do not exists. So i have to add one side to this tile. 
                        bool addedSide = false;
                        for (int i = 0; i < 4; i++)
                        {
                            if (repaired.TileSide[i].IsUsed == false && ShouldRemoveSide(coords, i, data.GetWidth(i)) != RemoveOptions.Must)
                            {
                                repaired.TileSide[i] = new TileSide(true);
                                addedSide = true;
                                break;
                            }
                        }
                        //I was not able to add one side to tile, so remove it completly.
                        if (addedSide == false)
                            repaired = new TileInfoItem(0);
                    }

                    //Insert repaired tile into scheme.
                    if (TilesInfo.IsType7(data.Type))
                        data.Offset = 0;
                    repaired = TilesInfo.GetExactTemplate(repaired);
                    data.Type = repaired.Type;
                    data.Repair();
                    scheme.Set_TileData(coords, data);
                }
            }
        }

        /// <summary>
        /// Returns what to do with tile side.
        /// </summary>
        /// <param name="coords">Coords of tile, you are currenly repairing.</param>
        /// <param name="side">Side of vire, you want to remove.</param>
        /// <param name="width">Width of side, you want to remove.</param>
        /// <returns></returns>
        private RemoveOptions ShouldRemoveSide(Point coords, int side, int width)
        {
            coords = Sides.Move(coords, side);
            side = Sides.OpositeSide(side);

            if (scheme.ValidateCoords(coords) == false)
                return RemoveOptions.Can;

            TileData data = this.scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(data.Type))
            {
                PlacedBug pBug = scheme.PlacedBugs.Get(data.HorzWidth);
                if (pBug.GetWireWidth(coords, side) > 0)
                    return RemoveOptions.CanNot;
                return RemoveOptions.Must;
            }

            TileInfoItem info = TilesInfo.GetItem(data.Type);
            if (info.TileSide[side].IsUsed && data.GetWidth(side) == width)
                return RemoveOptions.CanNot;
            return RemoveOptions.Can;
        }
        #endregion

        #region outer

        /// <summary>
        /// Repairs adjeced tiles, to provided coords.
        /// Also destroys paths in adjected tiles, to provided coords.
        /// </summary>
        internal void RepairOuter()
        {
            RepairOuter_private(true);
        }

        /// <summary>
        /// Destroys paths in adjected tiles, to provided coords.
        /// </summary>
        internal void Acknowledge()
        {
            RepairOuter_private(false);
        }

        private void RepairOuter_private(bool repair)
        {
            TileData data;
            TileInfoItem info;
            //Loop trough all provided coords, and find adjected coords them.
            foreach (Point coords in this.coordsList)
            {
                data = this.scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type))
                {
                    //Bug type tile has allways at max only one adjected tile.
                    PlacedBug pBug = scheme.PlacedBugs.Get(data.HorzWidth);
                    for (int i = 0; i < 4; i++)
                        OuterRepairSide(coords, i, pBug.GetWireWidth(coords, i), repair);
                }
                else
                {
                    info = TilesInfo.GetItem(data.Type);
                    for (int i = 0; i < 4; i++)
                    {
                        if (info.TileSide[i].IsUsed)
                            OuterRepairSide(coords, i, data.GetWidth(i), repair);
                    }
                }
            }
        }

        /// <summary>
        /// Repair adjected tile on provided side, if it's wire-width do not match provided wire-width.
        /// </summary>
        /// <param name="coords">Coords of base tile</param>
        /// <param name="side">Where to move from base coords.</param>
        /// <param name="width">Width of vire, you are moving from.</param>
        /// <param name="repair">TRUE: repair adjectd tile, FALSE: only destroy path in adjected tile</param>
        private void OuterRepairSide(Point coords, int side, int width, bool repair)
        {
            coords = Sides.Move(coords, side);
            if (this.scheme.ValidateCoords(coords) == false)
                return;
            if (this.coordsHash.Contains(coords))
                return;
            side = Sides.OpositeSide(side);

            TileData data = this.scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(data.Type) || repair == false)
            {
                //Tile is BugType, nothing to repair here, only destroy paths.
                //Repair variabile is FALSE, nothing to repair here.
                DestroyPaths(coords, side);
                return;
            }
            TileInfoItem info = TilesInfo.GetItem(data.Type);

            if (info.TileSide[side].IsUsed)
            {
                if (data.GetWidth(side) != width)
                {
                    //Vire-widths do not match.
                    //Remove tile-side.
                    TileInfoItem repaired = info.CoppySides();
                    repaired.TileSide[side] = new TileSide(false);
                    repaired = TilesInfo.GetExactTemplate(repaired);

                    data.Type = repaired.Type;
                    data.Repair();
                    scheme.Set_TileData(coords, data);
                }
                else
                    DestroyPaths(coords, side);
            }
        }

        /// <summary>
        /// Destroys all paths on provided coords and tile-side.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="side"></param>
        private void DestroyPaths(Point coords, int side)
        {
            Tile tile = scheme.Get_Tile(coords);

            if (TilesInfo.IsBugType(tile.Data.Type))
            {
                //PBug itself destroys paths.
                PlacedBug pBug = this.scheme.PlacedBugs.Get(tile.Data.HorzWidth);
                pBug.DestroyPaths(coords, side);
                return;
            }
            TileInfoItem info = TilesInfo.GetItem(tile.Data.Type);
            if (info.IsSource())
            {
                //Tile is source.
                //Get all SchemeSources on provided coords.
                //Deinitialize its IsOutputIn/IsInputIn based on from which side I came to provided coords.
                List<SchemeSource> sSources = this.scheme.Sources.GetSources(coords);
                if (TilesInfo.GetInputSide(info.Type) == side)
                {
                    foreach (SchemeSource sSource in sSources)
                    {
                        if (sSource.IsOutputIn != null)
                            scheme.Paths.Remove(sSource.IsOutputIn);
                    }
                }
                else if (TilesInfo.GetOutputSide(info.Type) == side)
                {
                    foreach (SchemeSource sSource in sSources)
                    {
                        if (sSource.IsInputIn != null)
                            scheme.Paths.Remove(sSource.IsInputIn);
                    }
                }
            }
            else
            {
                //Tile is not source.
                //Deinitialize all paths in tile.
                scheme.Paths.Remove(tile.Paths);
            }
        }
        #endregion

    }

}
