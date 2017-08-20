using CP_Engine.MapItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CP_Engine.WorkplaceAssistants
{
    public class DrawingAssistant
    {
        WorkPlace workplace;
        //SOURCE: Point where user pressed LMB, FORCE: Point where mouse is, while user is dragging.
        Point source, force, sourceCoords, forceCoords;
        bool isHorizontal;
        List<Point> selectedPoints;

        internal DrawingAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
            this.selectedPoints = new List<Point>();
        }

        /// <summary>
        /// Preview vire to scheme.
        /// </summary>
        /// <param name="source">Vire starting position.</param>
        /// <param name="force">Vire ending position.</param>
        public void PrepareLine(Point source, Point force)
        {
            bool sourceOrForceTilesChanged = false;
            this.source = source;
            this.force = force;
            Window window = workplace.CurrentWindow;

            //Coordinates of last tile.
            Point newForceCoords = window.GetClosestTile(force);
            if (newForceCoords != forceCoords)
            {
                forceCoords = newForceCoords;
                sourceOrForceTilesChanged = true;
            }
            //Coordinates of first tile.
            Point newSourceCoords = window.GetTileAt(source);
            if (window.Scheme.ValidateCoords(newSourceCoords))
            {
                if (newSourceCoords != sourceCoords)
                {
                    sourceCoords = newSourceCoords;
                    sourceOrForceTilesChanged = true;
                }
            }
            else
            {
                //When there is no tile on first coordinates, do nothing.
                return;
            }
            //If nothing changed, do nothing.
            if (sourceOrForceTilesChanged)
                return;

            this.selectedPoints.Clear();
            window.Selection.Items.Clear();
            //Add tiles to window selection and to local list.
            if (Math.Abs(source.X - force.X) < Math.Abs(source.Y - force.Y))
            {
                //User is drawing horizontal line.
                isHorizontal = false;
                if (sourceCoords.Y <= forceCoords.Y)
                {
                    for (int i = sourceCoords.Y; i <= forceCoords.Y; i++)
                    {
                        Point p = new Point(sourceCoords.X, i);
                        window.Selection.Items.Add(p);
                        selectedPoints.Add(p);
                    }
                }
                else
                {
                    for (int i = sourceCoords.Y; i >= forceCoords.Y; i--)
                    {
                        Point p = new Point(sourceCoords.X, i);
                        window.Selection.Items.Add(p);
                        selectedPoints.Add(p);
                    }
                }
            }
            else
            {
                //User is drawing vertical line.
                isHorizontal = true;
                if (sourceCoords.X <= forceCoords.X)
                {
                    for (int i = sourceCoords.X; i <= forceCoords.X; i++)
                    {
                        Point p = new Point(i, sourceCoords.Y);
                        window.Selection.Items.Add(p);
                        selectedPoints.Add(p);
                    }
                }
                else
                {
                    for (int i = sourceCoords.X; i >= forceCoords.X; i--)
                    {
                        Point p = new Point(i, sourceCoords.Y);
                        window.Selection.Items.Add(p);
                        selectedPoints.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// Insert vire to scheme.
        /// </summary>
        /// <param name="source">Vire starting position.</param>
        /// <param name="force">Vire ending position.</param>
        public void InsertLine(Point source, Point force)
        {
            PrepareLine(source, force);
            if (this.selectedPoints.Count <= 0)
                return;

            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            Repair repair = new Repair(workplace, workplace.CurrentWindow.Scheme);

            if (selectedPoints.Count > 1)
                InsertLongLine(repair);
            else
                InsertOneBlock(repair);

            repair.RepairInner();
            repair.RepairOuter();

            workplace.SchemeEventHistory.FinalizeEvent();
        }

        private void InsertLongLine(Repair repair)
        {
            TileData dataA = new TileData();
            TileData dataB;
            TileData dataM;
            TileInfoItem infoB;
            TileInfoItem infoA;
            TileInfoItem infoM;

            //Get vire width.
            int width = GetWidth();
            //Create pattern of inserting tile.
            if (isHorizontal)
            {
                infoA = new TileInfoItem(false, true, false, true);
                dataA.HorzWidth = width;
            }
            else
            {
                infoA = new TileInfoItem(true, false, true, false);
                dataA.VertWidth = width;
            }

            foreach (Point coords in selectedPoints)
            {
                //Get old tile data and tile info.
                dataB = workplace.CurrentWindow.Scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(dataB.Type) == false)
                {
                    infoB = TilesInfo.GetItem(dataB.Type);

                    //Merge old tile date with tile data pattern.
                    dataM = Merge(dataA, dataB);
                    infoM = Merge(infoA, infoB);
                    dataM.Type = infoM.Type;
                    dataM.Repair();

                    //Insert merged data into scheme.
                    workplace.CurrentWindow.Scheme.Set_TileData(coords, dataM);
                    repair.Add(coords);
                }
            }
            //Modify details.
            JoinEnds(selectedPoints[0]);
            JoinEnds(selectedPoints[selectedPoints.Count - 1]);
        }

        private void InsertOneBlock(Repair repair)
        {
            //Get original data on coords.
            Point coords = selectedPoints[0];
            TileData data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(data.Type))
                return;
            TileInfoItem info = TilesInfo.GetItem(data.Type);
            //Get side, where user clicked within tile.
            int side = workplace.CurrentWindow.GetSideOfPointInTile(this.source);
            //Do nothing if tile side is allready used.
            if (info.TileSide[side].IsUsed)
                return;
            //Do nothing if adjected width is 0.
            int adjectedWidth = workplace.CurrentWindow.Scheme.GetAdjectedTileWidth(coords, side);
            if (adjectedWidth <= 0)
                return;
            //Do nothing if adjected width is not shared with current tile's width.
            if (Sides.IsHorizontal(side))
            {
                if (data.HorzWidth > 0 && data.HorzWidth != adjectedWidth)
                    return;
            }
            else
            {
                if (data.VertWidth > 0 && data.VertWidth != adjectedWidth)
                    return;
            }

            //Create new data.
            TileInfoItem newInfo = info.CoppySides();
            newInfo.TileSide[side] = new TileSide(true);
            newInfo = TilesInfo.GetExactTemplate(newInfo);

            TileData newData = data;
            newData.Type = newInfo.Type;
            if (Sides.IsHorizontal(side))
                newData.HorzWidth = adjectedWidth;
            else
                newData.VertWidth = adjectedWidth;

            newData.Repair();

            workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);
            repair.Add(coords);
        }

        /// <summary>
        /// Returns vire width of inserted line.
        /// Prioritly uses width of starting tile (Tile where user clicked to draw a line).
        /// </summary>
        /// <returns></returns>
        private int GetWidth()
        {
            TileData data1 = workplace.CurrentWindow.Scheme.Get_TileData(selectedPoints[0]);
            TileData data2 = workplace.CurrentWindow.Scheme.Get_TileData(selectedPoints[selectedPoints.Count - 1]);
            if (isHorizontal)
            {
                if (data1.HorzWidth > 0)
                    return data1.HorzWidth;
                if (data1.VertWidth > 0)
                    return data1.VertWidth;
                if (data2.HorzWidth > 0)
                    return data2.HorzWidth;
                if (data2.VertWidth > 0)
                    return data2.VertWidth;
            }
            else
            {
                if (data1.VertWidth > 0)
                    return data1.VertWidth;
                if (data1.HorzWidth > 0)
                    return data1.HorzWidth;
                if (data2.VertWidth > 0)
                    return data2.VertWidth;
                if (data2.HorzWidth > 0)
                    return data2.HorzWidth;
            }
            return 1;
        }

        /// <summary>
        /// Only transforms unconnected 4 ends tile to connected 4 ends tile.  
        ///  1       1
        /// 1 1  => 1x1
        ///  1       1
        /// </summary>
        /// <param name="coords"></param>
        private void JoinEnds(Point coords)
        {
            TileData oldData = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            if (TilesInfo.IsType7(oldData.Type) && oldData.HorzWidth == oldData.VertWidth)
            {
                //Tile of type=7 has allways offset set to width of side, that is not using offset.
                TileData newData = oldData;
                newData.Type = 12;
                newData.Repair();
                workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);
            }
        }

        /// <summary>
        /// Merge two provided tile-infos to one tile-info. 
        /// </summary>
        /// <param name="infoA">Higher priority tile-info.</param>
        /// <param name="infoB">Lower priority tile-info.</param>
        /// <returns></returns>
        private TileInfoItem Merge(TileInfoItem infoA, TileInfoItem infoB)
        {
            TileInfoItem mergedInfo = new TileInfoItem();
            for (int i = 0; i < 4; i++)
                mergedInfo.TileSide[i] = new TileSide((infoA.TileSide[i].IsUsed || infoB.TileSide[i].IsUsed));
            mergedInfo = TilesInfo.GetExactTemplate(mergedInfo);
            return mergedInfo;
        }

        /// <summary>
        /// Merges two TileData to one.
        /// </summary>
        /// <param name="dataA">Higher priority data</param>
        /// <param name="dataB">Lower priority data</param>
        /// <returns></returns>
        private TileData Merge(TileData dataA, TileData dataB)
        {
            TileData toReturn = new TileData();
            if (dataA.HorzWidth > 0)
                toReturn.HorzWidth = dataA.HorzWidth;
            else
                toReturn.HorzWidth = dataB.HorzWidth;
            if (dataA.VertWidth > 0)
                toReturn.VertWidth = dataA.VertWidth;
            else
                toReturn.VertWidth = dataB.VertWidth;
            toReturn.Offset = dataB.Offset;
            return toReturn;
        }
    }
}
