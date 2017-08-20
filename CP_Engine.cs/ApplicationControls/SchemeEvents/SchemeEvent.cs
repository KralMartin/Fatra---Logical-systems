using ContextMenu_Mono.Advanced;
using CP_Engine.LoadSaveItems;
using CP_Engine.MapItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CP_Engine.SchemeEvents
{
    /// <summary>
    /// Stores state of scheme before and after changes were made.
    /// </summary>
    class SchemeEvent
    {
        /// <summary>
        /// IOputs were modified and PlacedBugs that holds this scheme has to be replaced.
        /// </summary>
        internal bool RequiresReplace { get; set; }

        int oldInputs, oldOutputs;          //Old count of inputs and outputs of scheme.
        SchemeBak oldState, newState;       //Old and new state of scheme.
        Scheme scheme;                      //Changed scheme.

        internal SchemeEvent(Scheme scheme)
        {
            //Remmember old state of scheme.
            this.scheme = scheme;
            this.oldState = new SchemeBak();
            this.oldState.CreateFromScheme(scheme);
            this.oldInputs = scheme.Bug.IODecription.Get_Inputs().Count;
            this.oldOutputs = scheme.Bug.IODecription.Get_Outputs().Count;
        }

        /// <summary>
        /// Checks if changes in scheme are valid. 
        /// Also do additional changes in project, if changes were valid.
        /// </summary>
        /// <param name="workplace"></param>
        /// <returns></returns>
        internal bool Validate(WorkPlace workplace)
        {
            //Remmember new state of scheme.
            this.newState = new SchemeBak();
            this.newState.CreateFromScheme(this.scheme);

            //Count ioputs.
            int newInputs = scheme.Bug.IODecription.Get_Inputs().Count;
            int newOutputs = scheme.Bug.IODecription.Get_Outputs().Count;

            bool foundError = false;
            string errMsg = null;

            if (newInputs <= 0 || newOutputs <= 0)
            {
                //You cant have PlacedBug with 0 inputs or outputs.
                errMsg = "Cant have zero inputs/outputs.";
                foundError = true;
            }
            else if (AreAllTilesEmpty(Math.Max(oldInputs, oldOutputs), Math.Max(newInputs, newOutputs), workplace) == false)
            {
                //When you increase width of PlacedBug, you have to have enough free space for this operation.
                errMsg = "Not enough place to insert new inputs/outputs";
                foundError = true;
            }
            if (foundError)
            {
                //Error was found, undo changes.
                Undo(workplace);
                Form form = DefaultUI.CreateFormText("Error", errMsg);
            }
            else
            {
                //Changes were valid.
                AfterExecute(workplace, false);
            }
            return !foundError;
        }

        /// <summary>
        /// Undo changes.
        /// </summary>
        /// <param name="workplace"></param>
        internal void Undo(WorkPlace workplace)
        {
            Execute(this.oldState, workplace);
            AfterExecute(workplace, true);
        }

        /// <summary>
        /// Redo changes.
        /// </summary>
        /// <param name="workplace"></param>
        internal void Redo(WorkPlace workplace)
        {
            Execute(this.newState, workplace);
            AfterExecute(workplace, true);
        }

        /// <summary>
        /// Sets scheme to one of its states.
        /// </summary>
        /// <param name="bak">Desired state of scheme.</param>
        /// <param name="workplace"></param>
        private void Execute(SchemeBak bak, WorkPlace workplace)
        {
            Repair repair = new Repair(workplace, this.scheme);

            //1. Delete PlacedBugs, that do not exists in desired state.
            foreach (PlacedBug pBug in this.scheme.PlacedBugs.GetItems())
            {
                if (Contains(bak, pBug.ID) == false)
                    workplace.BugAssistant.Delete(pBug);
            }
            //2. Insert PlacedBugs, that are not present in current state.
            foreach (PlacedBug pBugBak in bak.PlacedBugs)
            {
                if (this.scheme.PlacedBugs.Contains(pBugBak.ID) == false)
                {
                    Bug bug = workplace.Project.Bugs.GetItem(pBugBak.BugID);
                    PlacedBug pBug = workplace.BugAssistant.Insert(bug, pBugBak.Coords, this.scheme, repair);
                    pBug.Order = pBugBak.Order;
                    pBug.Description = pBugBak.Description;
                    pBug.Number = pBugBak.Number;
                }
            }
            //3. Insert tiles that differs from desired state.
            for (int row = 0; row < scheme.TilesCountY; row++)
            {
                for (int col = 0; col < scheme.TilesCountX; col++)
                {
                    Point coords = new Point(col, row);
                    TileData oldData = this.scheme.Get_TileData(coords);
                    //Ignore BugTiles.
                    if (TilesInfo.IsBugType(oldData.Type) == false)
                    {
                        TileData newData = bak.Tiles[col, row];
                        if (newData.MyEquals(oldData) == false)
                        {
                            this.scheme.Set_TileData(coords, newData);
                            repair.Add(coords);
                        }
                        else if (newData.ColorID != oldData.ColorID)
                            scheme.SetTileColor(coords, newData.ColorID);
                    }
                }
            }
            //Reorder items
            this.scheme.PlacedBugs.ReOrder();

            //Recreate breakPoints.
            this.scheme.BreakPoints.Import(bak.BreakPointsBak);

            //Set desired state to Bug.
            this.scheme.Bug.Inport(bak.BugBak, new Point());

            //Destoy adjected paths in scheme.
            repair.Acknowledge();
        }

        internal bool HasChanged()
        {
            for (int row = 0; row < this.scheme.TilesCountY; row++)
            {
                for (int col = 0; col < this.scheme.TilesCountX; col++)
                {
                    if (oldState.Tiles[col, row].MyEquals(scheme.Get_TileData(new Point(col, row))) == false)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns TRUE, if PlacedBugs collection withing SchemeBak contains provided PlacedBugID.
        /// </summary>
        /// <param name="bak">Instante to check for id.</param>
        /// <param name="pBugID">Placed bug id to find.</param>
        /// <returns></returns>
        private bool Contains(SchemeBak bak, int pBugID)
        {
            foreach (PlacedBug pBugBak in bak.PlacedBugs)
            {
                if (pBugBak.ID == pBugID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Function executed after Undo/Redo/Validate.
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="changeTab"></param>
        private void AfterExecute(WorkPlace workplace, bool changeTab)
        {
            if (RequiresReplace)
                workplace.BugAssistant.Replace(scheme);
            //Close windows that show PhysSchemes that are already removed.
            workplace.CloseWindows();
            //Open window on scheme, where changes were made.
            workplace.OpenWindow(this.scheme);
        }

        /// <summary>
        /// Returns TRUE if all PBugs can be expanded.
        /// PlacedBugs are expanded to left side.
        /// </summary>
        /// <param name="originalWidth"></param>
        /// <param name="newWidth">Maximum number of input or outputs of old bug.</param>
        /// <param name="workplace">Maximum number of input of outputs of new bug.</param>
        /// <returns></returns>
        private bool AreAllTilesEmpty(int originalWidth, int newWidth, WorkPlace workplace)
        {
            //Minimal size of bugs is 2 tiles.
            originalWidth = Math.Max(2, originalWidth);
            newWidth = Math.Max(2, newWidth);

            List<PlacedBug> pBugs = workplace.Project.SchemeStructure.Get_PlacedBugs(this.scheme);
            foreach (PlacedBug pBug in pBugs)
            {
                for (int col = originalWidth; col < newWidth + 1; col++)
                {
                    for (int row = 0; row < 2; row++)
                    {
                        Point coords = new Point(pBug.Coords.X + col, pBug.Coords.Y + row);
                        TileData data = pBug.ParentScheme.Get_TileData(coords);
                        if (col < newWidth)
                        {
                            if (data.Type != 0)
                                return false;
                        }
                        else if (TilesInfo.IsBugType(data.Type) == false)
                        {
                            TileInfoItem info = TilesInfo.GetItem(data.Type);
                            if (info.TileSide[Sides.Left].IsUsed)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
