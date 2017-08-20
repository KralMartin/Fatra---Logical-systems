using CP_Engine.BugItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace CP_Engine.MapItems.Collections
{
    class SchemeSourceCollection
    {
        /// <summary>
        /// SchemeSources that are not fully initialized (IsInputIn or IsOutputIn is set to NULL)
        /// </summary>
        internal List<SchemeSource> UninitializedSources { get; private set; }

        WorkPlace workplace;
        Scheme scheme;
        int currentIndex;                               //Variabile, that keeps track of used identifiers for SchemeSources.
        Dictionary<Point, List<SchemeSource>> items;    ///KEY: coords within scheme, VALUE: collection of SSources on coords.

        internal SchemeSourceCollection(WorkPlace workplace, Scheme scheme)
        {
            this.workplace = workplace;
            this.scheme = scheme;
            this.currentIndex = 0;
            this.items = new Dictionary<Point, List<SchemeSource>>();
            this.UninitializedSources = new List<SchemeSource>();
        }

        /// <summary>
        /// Returns all SchemeSources on provided coords.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal List<SchemeSource> GetSources(Point coords)
        {
            return items[coords];
        }

        /// <summary>
        /// Returns SSources on provided grid position.
        /// </summary>
        /// <param name="exPos"></param>
        /// <returns></returns>
        internal SchemeSource GetSource(ExactGridPosition exPos)
        {
            List<SchemeSource> sourcesOnCoords = items[exPos.Coords];
            foreach (SchemeSource source in sourcesOnCoords)
            {
                if (source.Position.Floor == exPos.Floor)
                    return source;
            }
            return null;
        }

        /// <summary>
        /// Removes all SchemeSources on provided coords.
        /// </summary>
        /// <param name="coords"></param>
        internal void Remove(Point coords)
        {
            //Get all SchemeSources.
            List<SchemeSource> sourcesOnCoords = items[coords];
            //Get all PSchemes.
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.scheme);
            foreach (SchemeSource sSource in sourcesOnCoords)
            {
                if (sSource.NoLongerInUse == false)
                {
                    //SchemeSource is no longer used.
                    sSource.NoLongerInUse = true;

                    //Remove its output and input path.
                    if (sSource.IsInputIn != null)
                        scheme.Paths.Remove(sSource.IsInputIn);
                    if (sSource.IsOutputIn != null)
                        scheme.Paths.Remove(sSource.IsOutputIn);
                    //Remove phys representation of this SchemeSource from all PSchemes.
                    foreach (PhysScheme pScheme in pSchemes)
                        pScheme.Sources.Remove(sSource.Identifier);
                }
            }
            //Remove list on coords from this dictionary.
            items.Remove(coords);

            //Remove scheme IODescription, if removed SSources were IOput type.
            //Also in this case, PBugs containing this scheme will have to be replaced.
            if (this.scheme.Bug.IODecription.Get_Description(coords) != null)
            {
                this.scheme.Bug.IODecription.Remove(coords);
                workplace.SchemeEventHistory.EventRequiresBugReplace();
            }
        }

        /// <summary>
        /// Adds SchemeSources.
        /// </summary>
        /// <param name="coords">Coords where in scheme will be placed.</param>
        /// <param name="width">Amount of SSources on provided coords.</param>
        /// <param name="tileType">Type of SSources created.</param>
        internal void AddSource(Point coords, int width, TileTypes tileType)
        {
            List<SchemeSource> sourcesOnCoords = new List<SchemeSource>();
            List<PhysScheme> physSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.scheme);
            for (int i = 0; i < width; i++)
            {
                //Create new SchemeSource.
                SchemeSource sSource;
                switch (tileType)
                {
                    case TileTypes.Repeater:
                        sSource = new BasicSchemeSource(this.scheme, new ExactGridPosition(coords, i), GetCurrentID(), false);
                        break;
                    case TileTypes.Konvertor:
                        sSource = new BasicSchemeSource(this.scheme, new ExactGridPosition(coords, i), GetCurrentID(), true);
                        break;
                    case TileTypes.Input:
                        sSource = new InnerSchemeSource(this.scheme, new ExactGridPosition(coords, i), GetCurrentID(), true);
                        break;
                    case TileTypes.Output:
                        sSource = new InnerSchemeSource(this.scheme, new ExactGridPosition(coords, i), GetCurrentID(), false);
                        break;
                    default:
                        sSource = null;
                        break;
                }
                //Add it into list and into Uninitialized collection.
                sourcesOnCoords.Add(sSource);
                this.UninitializedSources.Add(sSource);

                //Create new PhysSource in each of PhysSchemes.
                foreach (PhysScheme pScheme in physSchemes)
                    CreatePhysRepres(pScheme, sSource);
            }
            //Add SSources into this dictionary.
            items.Add(coords, sourcesOnCoords);

            //Add new description to bug, if newly created SSources are of type IOput.
            //Also in this case, PBugs containing this scheme will have to be replaced.
            if (tileType == TileTypes.Input)
            {
                this.scheme.Bug.IODecription.Add(new IODescription(true, "I", "", scheme.Bug.IODecription.Get_Inputs().Count, coords, sourcesOnCoords));
                workplace.SchemeEventHistory.EventRequiresBugReplace();
            }
            else if (tileType == TileTypes.Output)
            {
                this.scheme.Bug.IODecription.Add(new IODescription(false, "O", "", scheme.Bug.IODecription.Get_Outputs().Count, coords, sourcesOnCoords));
                workplace.SchemeEventHistory.EventRequiresBugReplace();
            }
        }

        internal void Clear()
        {
            List<Point> toRemove = new List<Point>();
            foreach (List<SchemeSource> sourcesOnCoords in items.Values)
                toRemove.Add(sourcesOnCoords[0].Position.Coords);
            foreach (Point coords in toRemove)
                Remove(coords);
        }

        /// <summary>
        /// Create PhysSource for each SchemeSource in this collection.
        /// </summary>
        /// <param name="pScheme">PScheme, that will be filled with new data.</param>
        internal void CreatePhysRepresentation(PhysScheme pScheme)
        {
            foreach (KeyValuePair<Point, List<SchemeSource>> entry in items)
            {
                foreach (SchemeSource sSource in entry.Value)
                    CreatePhysRepres(pScheme, sSource);
            }
        }

        /// <summary>
        /// Creates PSource and inserts it into provided PScheme.
        /// </summary>
        /// <param name="pScheme">PScheme, to which PSource will be inserted.</param>
        /// <param name="sSource">SSource, that will be referenced by PSource.</param>
        private void CreatePhysRepres(PhysScheme pScheme, SchemeSource sSource)
        {
            PhysSource physSource = new PhysSource(pScheme, sSource, false);
            pScheme.Sources.Add(sSource.Identifier, physSource);
        }

        /// <summary>
        /// Returns SSource ID, that is not used in thi
        /// </summary>
        /// <returns></returns>
        private int GetCurrentID()
        {
            return this.currentIndex++;
        }
    }
}
