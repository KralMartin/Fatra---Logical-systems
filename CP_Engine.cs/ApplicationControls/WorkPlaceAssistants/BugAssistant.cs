using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CP_Engine.WorkplaceAssistants
{
    /// <summary>
    /// Provides functions to insert and delete Pbugs.
    /// </summary>
    class BugAssistant
    {
        WorkPlace workplace;

        internal BugAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        /// <summary>
        /// Deletes and Inserts all PBugs containing provided scheme.
        /// </summary>
        /// <param name="scheme"></param>
        internal void Replace(Scheme scheme)
        {
            Repair repair = new Repair(workplace, scheme);
            List<PlacedBug> pBugs = workplace.Project.SchemeStructure.Get_PlacedBugs(scheme);
            foreach (PlacedBug pBug in pBugs)
            {
                pBug.RemoveSchemeSources();
                DeleteTiles(pBug);
            }
            foreach (PlacedBug pBug in pBugs)
            {
                pBug.CreateSchemeSources();
                CreateTiles(pBug, repair);
            }
        }

        /// <summary>
        /// Insert Bug into Scheme.
        /// </summary>
        /// <param name="bug">Bug to insert.</param>
        /// <param name="coords">Coords of placed Bug</param>
        /// <param name="parentScheme">Parent scheme.</param>
        /// <param name="repair">Repair instance</param>
        /// <returns></returns>
        internal PlacedBug Insert(Bug bug, Point coords, Scheme parentScheme, Repair repair)
        {
            //Create PlacedBug.
            PlacedBug pBug = new PlacedBug(parentScheme.PlacedBugs.GetItemID(), coords, bug, parentScheme);
            workplace.Project.SchemeStructure.PBugCreated(pBug);

            //Create SchemeSources of created PlacedBug.
            pBug.CreateSchemeSources();
            //Insert PlacedBug to parent Scheme.
            parentScheme.PlacedBugs.Add(pBug);
            //Insert Bug-Tiles into parent Scheme.
            CreateTiles(pBug, repair);

            //Create phys representation of PlacedBug for each of pSchemes of parent Scheme.
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(parentScheme);
            foreach (PhysScheme pScheme in pSchemes)
                pBug.CreatePhysRepre(workplace, pScheme);
            return pBug;
        }

        /// <summary>
        /// Deletes Bug on provided coords.
        /// </summary>
        /// <param name="coords">Exact coords of Bug.</param>
        /// <param name="parentScheme">Parent scheme.</param>
        internal void Delete(Point coords, Scheme parentScheme)
        {
            TileData data = parentScheme.Get_TileData(coords);
            PlacedBug pBug = parentScheme.PlacedBugs.Get(data.HorzWidth);
            Delete(pBug);
        }

        /// <summary>
        /// Deletes provided PBug.
        /// </summary>
        /// <param name="pBug"></param>
        internal void Delete(PlacedBug pBug)
        {
            //Remove it from parent scheme.
            pBug.ParentScheme.PlacedBugs.Remove(pBug.ID);
            //Remove outer SchemeSources.
            pBug.RemoveSchemeSources();
            //Remove all PSchemes that were contained in this PBug.
            pBug.RemoveAllPhysRepres(workplace);
            //Remove PBug from SchemeStructure.
            if (pBug.Bug.Scheme != null)
                workplace.Project.SchemeStructure.PBugRemoved(pBug);
            //Delete tiles under PBug.
            DeleteTiles(pBug);
            //Reorder parent scheme
            pBug.ParentScheme.PlacedBugs.ReOrder();
        }

        /// <summary>
        /// Deletes tiles under Bug.
        /// </summary>
        /// <param name="pBug"></param>
        private void DeleteTiles(PlacedBug pBug)
        {
            TileData data = new TileData(0);
            for (int x = 0; x < pBug.GetBugWidth(); x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Point coords = new Point(pBug.Coords.X + x, pBug.Coords.Y + y);
                    pBug.ParentScheme.Set_TileData(coords, data);
                }
            }
        }

        /// <summary>
        /// Creates tiles under Bub.
        /// Also inserts those tiles into Repair instance.
        /// </summary>
        /// <param name="pBug"></param>
        /// <param name="repair"></param>
        private void CreateTiles(PlacedBug pBug, Repair repair)
        {
            TileData newData = new TileData(99);
            newData.HorzWidth = pBug.ID;
            for (int x = 0; x < pBug.Bug.GetBugWidth(); x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Point coords = new Point(pBug.Coords.X + x, pBug.Coords.Y + y);
                    pBug.ParentScheme.Set_TileData(coords, newData);
                    repair.Add(coords);
                }
            }
        }
    }
}
