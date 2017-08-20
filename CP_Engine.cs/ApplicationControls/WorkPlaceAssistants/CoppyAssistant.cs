using CP_Engine.MapItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CP_Engine.WorkplaceAssistants
{
    public class CoppyAssistant
    {
        WorkPlace workplace;
        Scheme sourceScheme;
        List<Point> sourceSelection;
        Point sourceTopLeftPoint;
        List<PlacedBug> sourcePBugs;
        List<Point> currentSourceSelection;
        Dictionary<Point, TileData> sourceData;

        internal CoppyAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        internal void Coppy()
        {
            this.sourceScheme = workplace.CurrentWindow.Scheme;
            this.sourceTopLeftPoint = workplace.CurrentWindow.Selection.MostTopLeftCoord();
            this.sourceSelection = workplace.CurrentWindow.Selection.Items.ToList();
            sourcePBugs = FindAllPBugsIn(this.sourceSelection, this.sourceScheme);

            //Add to selection all tiles, that are under partialy selected PlacedBugs.
            currentSourceSelection = this.sourceSelection.ToList();
            foreach (PlacedBug pBug in sourcePBugs)
            {
                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < pBug.GetBugWidth(); col++)
                    {
                        Point coords = new Point(pBug.Coords.X + col, pBug.Coords.Y + row);
                        if (currentSourceSelection.Contains(coords) == false)
                            currentSourceSelection.Add(coords);
                    }
                }
            }
            //BackUp TileData.
            sourceData = new Dictionary<Point, TileData>();
            foreach (Point coords in this.sourceSelection)
            {
                TileData data = this.sourceScheme.Get_TileData(coords);
                sourceData.Add(coords, data);
            }
        }

        internal void Paste()
        {
            Scheme targetScheme = workplace.CurrentWindow.Scheme;
            if (workplace.CurrentWindow.Selection.Items.Count <= 0)
                return;

            //Get Point, that will move whole selection
            Point adjustment = workplace.CurrentWindow.Selection.MostTopLeftCoord();
            adjustment -= this.sourceTopLeftPoint;
            List<Point> adjustedCoords = new List<Point>();
            foreach (Point coords in currentSourceSelection)
                adjustedCoords.Add(coords + adjustment);

            //Check if you have enough space to paste items.
            foreach (Point coords in adjustedCoords)
            {
                if (targetScheme.ValidateCoords(coords) == false)
                    return;
            }

            workplace.SchemeEventHistory.StartEvent(targetScheme, true);
            workplace.SchemeEventHistory.EventRequiresBugReplace();

            //Delete all tiles and pBugs.
            ClearSelection(adjustedCoords);

            Repair repair = new Repair(workplace, targetScheme);
            //Insert bugs
            foreach (PlacedBug pBug in this.sourcePBugs)
                workplace.BugAssistant.Insert(pBug.Bug, pBug.Coords+adjustment, targetScheme, repair); 
            //Insert tiles.
            foreach (Point coords in this.sourceSelection)
            {
                TileData data = sourceData[coords];
                if (TilesInfo.IsBugType(data.Type) == false)
                {
                    Point newCoords = coords + adjustment;
                    targetScheme.Set_TileData(newCoords, data);
                    repair.Add(newCoords);
                }
            }
            repair.RepairOuter();
            workplace.SchemeEventHistory.FinalizeEvent();
        }

        public void Delete()
        {
            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            ClearSelection(workplace.CurrentWindow.Selection.Items.ToList());
            workplace.SchemeEventHistory.FinalizeEvent();
        }

        private void ClearSelection(List<Point> selection)
        {
            Scheme scheme = workplace.CurrentWindow.Scheme;
            foreach (Point coords in selection)
            {
                TileData data = scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type))
                {
                    PlacedBug pBug = scheme.PlacedBugs.Get(data.HorzWidth);
                    workplace.BugAssistant.Delete(pBug);
                }
                else
                {
                    scheme.Set_TileData(coords, new TileData(0));
                }
            }
        }

        private List<PlacedBug> FindAllPBugsIn(List<Point> coordsCol, Scheme scheme)
        {
            List<PlacedBug> toReturn = new List<PlacedBug>();

            foreach (Point coords in coordsCol)
            {
                TileData data = scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type))
                {
                    PlacedBug pBug = scheme.PlacedBugs.Get(data.HorzWidth);
                    if (toReturn.Contains(pBug) == false)
                        toReturn.Add(pBug);
                }
            }
            return toReturn;
        }
    }
}
