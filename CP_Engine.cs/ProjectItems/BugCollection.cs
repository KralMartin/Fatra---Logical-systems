using CP_Engine.BugItems;
using System.Collections.Generic;
using System.Linq;

namespace CP_Engine.ProjectItems
{
    /// <summary>
    /// Contains all bugs, that user can use within project.
    /// </summary>
    public class BugCollection
    {
        Dictionary<int, Bug> dict;
        List<Bug> list;

        internal BugCollection()
        {
            dict = new Dictionary<int, Bug>();
            list = new List<Bug>();

            //Create special bugs.
            Add(new MemmoryBug());
            Add(new SimpleDisplayBug());
            Add(new ClockBug());
        }

        /// <summary>
        /// Add bug into collection.
        /// </summary>
        /// <param name="value"></param>
        internal void Add(Bug value)
        {
            list.Add(value);
            list.OrderBy(x => x.Title);
            dict.Add(value.ID, value);
        }

        /// <summary>
        /// Returns unique ID, that differs from all bug.ID in this collection.
        /// </summary>
        /// <returns></returns>
        internal int GetBugID()
        {
            int max = 100;
            foreach (Bug bug in list)
            {
                if (bug.ID > max)
                    max = bug.ID;
            }
            max++;
            return max;
        }

        /// <summary>
        /// Returns default name of Scheme.
        /// </summary>
        /// <returns></returns>
        internal string DefaultBugName()
        {
            int index = 0;
            string toReturn = null;
            bool nameAllreadyExists;
            while (true)
            {
                index++;
                toReturn = "Scheme " + index;
                nameAllreadyExists = false;
                foreach (Bug bug in list)
                {
                    if (bug.Title == toReturn)
                    {
                        nameAllreadyExists = true;
                        break;
                    }
                }
                if (nameAllreadyExists == false)
                    return toReturn;
            }
        }
                
        public List<Bug> GetItems()
        {
            return list;
        }

        internal Bug GetItem(int bugID)
        {
            return dict[bugID];
        }
    }
}
