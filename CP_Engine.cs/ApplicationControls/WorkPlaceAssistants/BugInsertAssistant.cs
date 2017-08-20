using ContextMenu_Mono.Advanced;
using CP_Engine.MapItems;
using Microsoft.Xna.Framework;

namespace CP_Engine.WorkplaceAssistants
{
    class BugInsertAssistant
    {
        WorkPlace workplace;
        Bug currentBug;         //Bug, that is about to be placed into scheme by user.
        Point lastCoords;       //Coords in last update.
        bool selectionValid;

        internal BugInsertAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        internal void StartInserting(Bug bug)
        {
            this.currentBug = bug;
            this.lastCoords = new Point(-1, -1);
        }

        internal void MouseMove(Point coords)
        {
            if (lastCoords == coords)
                return;
            lastCoords = coords;

            //Reset Selection.
            selectionValid = true;
            workplace.CurrentWindow.Selection.Items.Clear();

            //Do nothing if Bug is outside of bounds of scheme grid.
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;
            if (workplace.CurrentWindow.Scheme.ValidateCoords(new Point(coords.X + currentBug.GetBugWidth() - 1, coords.Y + 1)) == false)
                return;

            //Check if Bug can be placed on current position.
            for (int x = 0; x < currentBug.GetBugWidth(); x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Point currentCoords = new Point(coords.X + x, coords.Y + y);
                    workplace.CurrentWindow.Selection.Items.Add(currentCoords);
                    TileData data = workplace.CurrentWindow.Scheme.Get_TileData(currentCoords);
                    if (TilesInfo.IsBugType(data.Type) || TilesInfo.IsType21(data.Type) || TilesInfo.IsType22(data.Type))
                        selectionValid = false;
                }
            }
            workplace.CurrentWindow.Selection.IsValid = selectionValid;
        }

        internal void MouseClick(Point coords)
        {
            MouseMove(coords);
            //Prevents creating Bug loop.
            if (workplace.Project.SchemeStructure.GetAllowedSchemesIn(workplace.CurrentWindow.Scheme).Contains(this.currentBug) == false)
            {
                DefaultUI.CreateFormText("Error", "Cant insert this bug into this scheme");
                workplace.Set_CurrentAction(Actions.Selecting);
            }
            else if (selectionValid)
            {
                //Insert Bug into scheme.
                workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
                Repair repair = new Repair(workplace, workplace.CurrentWindow.Scheme);
                workplace.BugAssistant.Insert(this.currentBug, this.lastCoords, workplace.CurrentWindow.Scheme, repair);
                repair.RepairOuter();
                workplace.SchemeEventHistory.FinalizeEvent();
            }
        }

    }
}
