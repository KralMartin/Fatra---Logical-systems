using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CP_Engine.SchemeItems;

namespace CP_Engine.BugItems
{
    /// <summary>
    /// Represents tiles of PBug on its whole width.
    /// Represenst all inputs or all outputs of PBug.
    /// </summary>
    class CollectionOfOuterSchemeSourceCollection
    {
        OuterSchemeSourceCollection[] items;

        internal CollectionOfOuterSchemeSourceCollection(PlacedBug pBug, bool isInput)
        {
            List<IODescription> sourceItems;
            if (isInput)
                sourceItems = pBug.Bug.IODecription.Get_Inputs();
            else
                sourceItems = pBug.Bug.IODecription.Get_Outputs();

            this.items = new OuterSchemeSourceCollection[sourceItems.Count];

            Point coords = pBug.Coords;
            //Outputs are on bottom side of Bug.
            if (isInput == false)
                coords.Y += 1;

            int i = 0;
            foreach (IODescription desc in sourceItems)
            {
                items[i] = new OuterSchemeSourceCollection(pBug, desc, coords, isInput);
                coords.X += 1;
                i++;
            }
        }

        /// <summary>
        /// Returns wire width on provided index.
        /// </summary>
        /// <param name="index">Number representating tile, counted from left side of PBug.</param>
        /// <returns></returns>
        internal int GetVireWidth(int index)
        {
            if (this.items.Length > index)
                return items[index].GetVireWidth();
            return -1;
        }

        internal OuterSchemeSource GetSchemeSource(int floor, int index)
        {
            if (index < this.items.Length)
            {
                return this.items[index].GetSchemeSource(floor);
            }
            return null;
        }

        internal void DestroyPaths(int index, bool isInput, Scheme scheme)
        {
            if (index < this.items.Length)
                this.items[index].DestroyPaths(isInput, false, scheme);
        }

        internal void DestroySchemeSources(PlacedBug pBug, bool isInput)
        {
            for (int i = 0; i < items.Length; i++)
                items[i].DestroyPaths(isInput, true, pBug.ParentScheme);
        }

        internal int GetItemsLenght()
        {
            return items.Length;
        }
    }


}
