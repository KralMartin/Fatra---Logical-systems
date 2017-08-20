using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CP_Engine.BugItems
{
    /// <summary>
    /// Represents one tile of PBug.
    /// </summary>
    class OuterSchemeSourceCollection
    {
        OuterSchemeSource[] items;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBug">Place bug.</param>
        /// <param name="ioDescription">IODescription of tile, represented by this instance.</param>
        /// <param name="coords">Coords of tile, represented by this instance.</param>
        /// <param name="isInput">TRUE: this instance contains inputs, FALSE: this instance contains outputs</param>
        internal OuterSchemeSourceCollection(PlacedBug pBug, IODescription ioDescription, Point coords, bool isInput)
        {
            List<SchemeSource> sourcesOnCoords = ioDescription.SchemeSourcesOnCoords;
            this.items = new OuterSchemeSource[sourcesOnCoords.Count];

            for (int i = 0; i < items.Length; i++)
            {
                //Get InnerSSource.
                SchemeSource innerSSrouce = sourcesOnCoords[i];
                //Create OuterSSource.
                items[i] = new OuterSchemeSource(new ExactGridPosition(ioDescription.Coords, i), isInput, new ExactGridPosition(coords, i), pBug);
                //Store newly created OuterSSource into PBug.
                pBug.SchemeSources.Add(innerSSrouce.Identifier, items[i]);
                //Set newly created OuterSSource as uninitialized.
                pBug.ParentScheme.Sources.UninitializedSources.Add(items[i]);
            }
        }

        internal OuterSchemeSource GetSchemeSource(int floor)
        {
            if (floor < this.items.Length)
                return items[floor];
            return null;
        }

        internal void DestroyPaths(bool isInput, bool uninitializeSources, Scheme scheme)
        {
            if (isInput)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (uninitializeSources)
                        items[i].NoLongerInUse = true;
                    scheme.Paths.Remove(items[i].IsOutputIn);
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (uninitializeSources)
                        items[i].NoLongerInUse = true;
                    scheme.Paths.Remove(items[i].IsInputIn);
                }
            }
        }

        internal int GetVireWidth()
        {
            return items.Length;
        }
    }
}
