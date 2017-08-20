using CP_Engine.SchemeItems;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CP_Engine.MapItems.Collections
{
    class SchemePlacedBugCollection
    {
        internal List<PlacedBug> OrderedItems { get; private set; }

        Dictionary<int, PlacedBug> items;   //KEY: PBug ID, VALUE: PBug
        int currentItemID;                  //Keeps track of usable PBugID

        internal SchemePlacedBugCollection()
        {
            currentItemID = 0;
            OrderedItems = new List<PlacedBug>();
            items = new Dictionary<int, PlacedBug>();
        }

        /// <summary>
        /// Returns PBugID, that is not used in this colleciton yet.
        /// </summary>
        /// <returns></returns>
        internal int GetItemID()
        {
            return currentItemID++;
        }

        /// <summary>
        /// Adds value into this collection.
        /// </summary>
        /// <param name="bug"></param>
        internal void Add(PlacedBug bug)
        {
            this.items.Add(bug.ID, bug);
        }

        /// <summary>
        /// Not Safe!
        /// Returns PBug.
        /// </summary>
        /// <param name="id">PBug id.</param>
        /// <returns></returns>
        internal PlacedBug Get(int id)
        {
            return items[id];
        }

        /// <summary>
        /// Removes PBug.
        /// </summary>
        /// <param name="id">Pbug id.</param>
        internal void Remove(int id)
        {
            this.items.Remove(id);
        }

        /// <summary>
        /// Returns TRUE, if this collection contains PBug with provided ID.
        /// </summary>
        /// <param name="id">PBug id.</param>
        /// <returns></returns>
        internal bool Contains(int id)
        {
            return items.ContainsKey(id);
        }

        /// <summary>
        /// Returns all PBugs in a list.
        /// </summary>
        /// <returns></returns>
        internal List<PlacedBug> GetItems()
        {
            return this.items.Values.ToList();
        }
        
        /// <summary>
        /// Creates phys representation of each PBug stored in this collection.
        /// </summary>
        /// <param name="workplace"></param>
        /// <param name="pScheme">PScheme, that will be filled with new data.</param>
        internal void CreatePhysRepresentation(WorkPlace workplace, PhysScheme pScheme)
        {
            foreach (PlacedBug pBug in this.items.Values)
            {
                pBug.CreatePhysRepre(workplace, pScheme);
            }
        }

        /// <summary>
        /// Changes Order property of PBug. 
        /// Then repairs Order property of all PBugs stored in this collection.
        /// </summary>
        /// <param name="pBug"></param>
        /// <param name="order"></param>
        internal void ChangeOrder(PlacedBug pBug, int order)
        {
            //Remove provided PBug from this collection.
            OrderedItems.Remove(pBug);
            //When provided order =-1, it means, provided PBug will no longer be ordered.
            if (order >= 0)
                OrderedItems.Insert(order, pBug);
            else
                pBug.Order = -1;
            //Set proper Order values.
            int index = 0;
            foreach (PlacedBug current in OrderedItems)
                current.Order = index++;
        }

        internal void ReOrder()
        {
            OrderedItems.Clear();
            foreach (PlacedBug pBug in this.items.Values)
            {
                if (pBug.Order >= 0)
                    OrderedItems.Add(pBug);
            }
            OrderedItems = OrderedItems.OrderBy(x => x.Order).ToList();
            //Set proper Order values.
            int index = 0;
            foreach (PlacedBug current in OrderedItems)
                current.Order = index++;
        }
    }
}
