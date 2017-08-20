using CP_Engine.SchemeItems;
using System.Collections.Generic;
using System;
using System.Xml;
using CP_Engine.BugItems;
using CP_Engine.cs.Utilities;

namespace CP_Engine.ProjectItems
{
    /// <summary>
    /// Keeps track of schemes and physSchemes.
    /// </summary>
    class SchemeStructure
    {
        WorkPlace workplace;
        Dictionary<Scheme, SchemeStructureItem> items;

        internal SchemeStructure(WorkPlace workplace)
        {
            this.workplace = workplace;
            this.items = new Dictionary<Scheme, SchemeStructureItem>();
        }

        internal void PSchemeCreated(PhysScheme pScheme)
        {
            items[pScheme.PlacedBug.Bug.Scheme].PSchemes.Add(pScheme);
        }

        internal void PSchemeRemoved(PhysScheme pScheme)
        {
            //Remove itself.
            bool res =items[pScheme.PlacedBug.Bug.Scheme].PSchemes.Remove(pScheme);
            //Do same foreach children
            foreach (PhysScheme child in pScheme.Children.Values)
                PSchemeRemoved(child);
        }

        internal void PSchemeReseted(PhysScheme pScheme)
        {
            //Remove children
            foreach (PhysScheme child in pScheme.Children.Values)
                PSchemeRemoved(child);
        }

        internal void PBugCreated(PlacedBug pBug)
        {
            //Keep track only of scheme bugs.
            if (pBug.Bug.Scheme != null)
            {
                SchemeStructureItem item = this.items[pBug.Bug.Scheme];
                item.PlacedBugs.Add(pBug);

                if (item.Parents.Contains(pBug.ParentScheme) == false)
                    item.Parents.Add(pBug.ParentScheme);
            }
        }

        internal void PBugRemoved(PlacedBug pBug)
        {
            SchemeStructureItem item = this.items[pBug.Bug.Scheme];
            item.PlacedBugs.Remove(pBug);

            foreach (PlacedBug current in item.PlacedBugs)
            {
                if (object.ReferenceEquals(current.ParentScheme, pBug.ParentScheme))
                    return;
            }
            item.Parents.Remove(pBug.ParentScheme);
        }

        internal void SchemeCreated(Scheme scheme)
        {
            items.Add(scheme, new SchemeStructureItem());
        }

        internal void SchemeDeleted(Scheme scheme)
        {
            items.Remove(scheme);
        }

        /// <summary>
        /// Returns PBugs that contain provided scheme.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        internal List<PlacedBug> Get_PlacedBugs(Scheme scheme)
        {
            return items[scheme].PlacedBugs;
        }

        /// <summary>
        /// Returns PhysSchemes that represents provided scheme.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        internal List<PhysScheme> Get_PhysSchemes(Scheme scheme)
        {
            return items[scheme].PSchemes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        internal List<Bug> GetAllowedSchemesIn(Scheme scheme)
        {
            //Schemes that are not allowed in provided scheme.
            List<Scheme> notAllowed = new List<Scheme>();

            Queue<Scheme> que = new Queue<Scheme>();
            que.Enqueue(scheme);
            //Initialize que by inserting provided scheme into it. Sheme is not allowed to be inserted into same scheme.
            while (que.Count > 0)
            {
                Scheme current = que.Dequeue();
                if (notAllowed.Contains(current) == false)
                {
                    //Scheme is not allowed in provided scheme, because scheme is parent to provided scheme.
                    notAllowed.Add(current);
                    SchemeStructureItem item = this.items[current];
                    foreach (Scheme parentScheme in item.Parents)
                        que.Enqueue(parentScheme);
                }
            }
            //Create list of bugs, that their Scheme is not in NOTALLOWED collection.
            List<Bug> toReturn = new List<Bug>();
            foreach (Bug bug in workplace.Project.Bugs.GetItems())
            {
                if (bug.Scheme == null || notAllowed.Contains(bug.Scheme) == false)
                    toReturn.Add(bug);
            }
            return toReturn;
        }

        internal List<Bug> GetBugsOrderedByDependancy()
        {
            List<Bug> allBugs = workplace.Project.Bugs.GetItems();
            List<Bug> bugs = new List<Bug>();
            foreach (Bug bug in allBugs)
            {
                if (bug.IsUserCreated())
                    bugs.Add(bug);
            }

            List<Bug> toReturn = new List<Bug>();
            while (bugs.Count>0)
            {
                Bug toAdd = null;
                foreach (Bug bug in bugs)
                {
                    SchemeStructureItem item = items[bug.Scheme];
                    bool allParentsReaddy = true;
                    foreach (Scheme parentScheme in item.Parents)
                    {
                        if (toReturn.Contains(parentScheme.Bug) == false)
                        {
                            allParentsReaddy = false;
                            break;
                        }
                    }
                    if (allParentsReaddy)
                    {
                        toAdd = bug;
                        break;
                    }
                }
                if (toAdd != null)
                {
                    bugs.Remove(toAdd);
                    toReturn.Add(toAdd);
                }
            }
            toReturn.Reverse();
            return toReturn;
        }

        #region Save memmory values
        internal void SaveMemmoryValues(XmlWriter xml)
        {
            foreach (Bug bug in workplace.Project.Bugs.GetItems())
            {
                if (bug.IsUserCreated())
                    Save(bug.Scheme, xml);
            }
        }

        private void Save(Scheme scheme, XmlWriter xml)
        {
            List<PlacedBug> memories = new List<PlacedBug>();
            foreach (PlacedBug pBug in scheme.PlacedBugs.GetItems())
            {
                if (pBug.Bug is MemmoryBug)
                    memories.Add(pBug);
            }
            if (memories.Count > 0)
            {
                List<PhysScheme> pSchemes = Get_PhysSchemes(scheme);
                foreach (PhysScheme pScheme in pSchemes)
                    Save(pScheme, memories, xml);
            }
        }

        private void Save(PhysScheme pScheme, List<PlacedBug> memories, XmlWriter xml)
        {
            foreach (PlacedBug memmory in memories)
            {
                SpecialPhysScheme specialPscheme = pScheme.SpecialChildren[memmory.ID];
                string path = MemmoryValueBak.GetPath(specialPscheme);
                xml.WriteElementString(XML.Item, path);
            }
        }
        #endregion

    }
}
