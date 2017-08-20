using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace CP_Engine.PathFinderItems
{
    /// <summary>
    /// Creates Paths and stores themm into scheme.
    /// </summary>
    class PathFinder
    {
        WorkPlace workplace;
        List<SchemePath> createdPaths;                  //Paths, that have been craeted in this instance.
        List<ExactGridPosition> itemsAllreadyIn;        //Items allready stored in currently created path.
        Queue<ExactGridPosition> itemsToAdd;            //Items that will be inserted into currently created path.
        Scheme scheme;                                  //Current scheme, that help me find path.
        Tile[,] tiles;                                  //Tiles of current scheme.
        SchemePath path;                                //Currently created path.

        //debug
        List<Point> failedAt = new List<Point>();

        internal PathFinder(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        internal void FindPaths(List<SchemeSource> sSources)
        {
            this.createdPaths = new List<SchemePath>();
            foreach (SchemeSource sSource in sSources)
                FindPath(sSource);


            if (failedAt.Count > 0)
            {
                Debugger.Break();
            }
        }

        private void FindPath(SchemeSource source)
        {
            if (source.NoLongerInUse)
                return;
            if (source.UsesIsInputIn() && source.IsInputIn == null)
                FindPath_Private(source, true);
            if (source.UsesIsOutputIn() && source.IsOutputIn == null)
                FindPath_Private(source, false);
        }

        internal void FindPath_Private(SchemeSource source, bool asInput)
        {
            //Initialize variabiles
            itemsAllreadyIn = new List<ExactGridPosition>();
            itemsToAdd = new Queue<ExactGridPosition>();
            this.scheme = source.Scheme;
            this.tiles = scheme.Get_Tiles();
            path = this.scheme.Paths.Create();
            createdPaths.Add(path);

            TileData data = source.Scheme.Get_TileData(source.Position.Coords);
            //Side where to move from provided SSource.
            int side;
            if (asInput)
            {
                //Provided SSource is input to path.
                side = TilesInfo.GetOutputSide(data.Type);
                path.Inputs.Add(source);
                source.IsInputIn = path;
            }
            else
            {
                //Provided SSource is output of path.
                side = TilesInfo.GetInputSide(data.Type);
                path.Outputs.Add(source);
                source.IsOutputIn = path;
            }
            //Mark that exactGridPosition is allready stored in path.
            ExactGridPosition exactGridPosition = new ExactGridPosition();
            exactGridPosition.Coords = source.Position.Coords;
            exactGridPosition.Floor = source.Position.Floor;
            itemsAllreadyIn.Add(exactGridPosition);

            //Move to another tile.
            GoTo(source.Position.Coords, side, source.Position.Floor);
            
            TileInfoItem info;
            Tile tile;
            while (itemsToAdd.Count > 0)
            {
                exactGridPosition = itemsToAdd.Dequeue();

                tile = tiles[exactGridPosition.Coords.X, exactGridPosition.Coords.Y];
                data = tile.Data;
                info = TilesInfo.GetItem(data.Type);

                //Mark, that i have been at exactGridPosition.
                itemsAllreadyIn.Add(exactGridPosition);
                //DEBUG i should never get into conditions.
                if (tile.Paths[exactGridPosition.Floor] != null)
                {
                    if (createdPaths.Contains(tile.Paths[exactGridPosition.Floor]) == false)
                    {
                        if (tile.Paths[exactGridPosition.Floor].NoLongerInUse == false)
                        {
                            if (failedAt.Contains(exactGridPosition.Coords) == false)
                                failedAt.Add(exactGridPosition.Coords);
                            //Debugger.Break();
                        }
                    }
                }
                //Set path to tile.
                tile.Paths[exactGridPosition.Floor] = path;

                //Go to adjected tiles.
                for (int i = 0; i < 4; i++)
                {
                    if (info.TileSide[i].IsUsed)
                    {
                        int floor = exactGridPosition.Floor;
                        if (info.TileSide[i].UsesOffset)
                            floor -= data.Offset;

                        if (data.Offset < 0)
                            floor += data.Offset;

                        if (floor >= 0 && floor < data.GetWidth(i))
                        {
                            GoTo(exactGridPosition.Coords, i, floor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// TRUE: ItemsAllreadyIn do not contain provided position.
        /// </summary>
        /// <param name="pathData"></param>
        /// <returns></returns>
        private bool Contains(ExactGridPosition pathData)
        {
            foreach (ExactGridPosition current in this.itemsAllreadyIn)
            {
                if (current.Floor == pathData.Floor && current.Coords == pathData.Coords)
                    return true;
            }
            return false;
        }

        private void GoTo(Point coords, int side, int floor)
        {
            coords = Sides.Move(coords, side);
            side = Sides.OpositeSide(side);
            if (scheme.ValidateCoords(coords))
            {
                Tile tile = tiles[coords.X, coords.Y];
                if (tile != null)
                {
                    TileData data = tile.Data;
                    if (TilesInfo.IsBugType(data.Type))
                    {
                        //Let PBug insert SSources into Path.
                        PlacedBug pBug = scheme.PlacedBugs.Get(data.HorzWidth);
                        pBug.AddToPath(path, new ExactGridPosition(coords, floor), side);
                    }
                    else
                    {
                        TileInfoItem info = TilesInfo.GetItem(data.Type);

                        //When this condition is met, theres error in scheme.
                        int sideWidth;
                        if (Sides.IsHorizontal(side))
                            sideWidth = data.HorzWidth;
                        else
                            sideWidth = data.VertWidth;

                        if (floor >= sideWidth)
                        {
                            //turns out you get here more often
                            //probably in case, where tile side goes to empty tile.
                            //Debugger.Break();
                            return;
                        }
                        //===================
                        if (info.TileSide[side].UsesOffset)
                            floor += data.Offset;

                        if (data.Offset < 0)
                            floor -= data.Offset;

                        ExactGridPosition exPos = new ExactGridPosition();
                        exPos.Coords = coords;
                        exPos.Floor = floor;

                        if (info.IsSource())
                        {
                            //Tile has SSource representation.
                            //Get SSource on coords.
                            SchemeSource source = scheme.Sources.GetSource(exPos);
                            if (TilesInfo.GetOutputSide(info.Type) == side)
                            {
                                if (source.IsInputIn == null || source.IsInputIn.ID != path.ID)
                                {
                                    //When i am on output side of SSource, add it into path as Input.
                                    path.Inputs.Add(source);
                                    source.IsInputIn = path;
                                }
                            }
                            else if (TilesInfo.GetInputSide(info.Type) == side)
                            {
                                if (source.IsOutputIn == null || source.IsOutputIn.ID != path.ID)
                                {
                                    //When i am on input side of SSource, add it into path as Output.
                                    path.Outputs.Add(source);
                                    source.IsOutputIn = path;
                                }
                            }
                        }
                        else if (Contains(exPos) == false && info.TileSide[side].IsUsed)
                        {
                            //Continue searching trough this tile.
                            itemsToAdd.Enqueue(exPos);
                        }
                        //if (Contains(exPos) == false)
                        //{
                        //    if (info.IsSource())
                        //    {
                        //        //Tile has SSource representation.
                        //        //Get SSource on coords.
                        //        SchemeSource source = scheme.Sources.GetSource(exPos);
                        //        if (TilesInfo.GetOutputSide(info.Type) == side)
                        //        {
                        //            //When i am on output side of SSource, add it into path as Input.
                        //            path.Inputs.Add(source);
                        //            source.IsInputIn = path;
                        //        }
                        //        else if (TilesInfo.GetInputSide(info.Type) == side)
                        //        {
                        //            //When i am on input side of SSource, add it into path as Output.
                        //            path.Outputs.Add(source);
                        //            source.IsOutputIn = path;
                        //        }
                        //    }
                        //    else if (info.TileSide[side].IsUsed)
                        //    {
                        //        //Continue searching trough this tile.
                        //        itemsToAdd.Enqueue(exPos);
                        //    }
                        //}
                    }
                }
            }
        }

    }
}
