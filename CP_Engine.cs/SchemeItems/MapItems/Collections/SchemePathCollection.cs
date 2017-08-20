using CP_Engine.SchemeItems;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace CP_Engine.MapItems.Collections
{
    class SchemePathCollection
    {
        private WorkPlace workplace;
        private Scheme scheme;
        private int usableID;                       //Variabile, that keeps track of used identifiers for Paths.
        private Dictionary<int, SchemePath> items;  //KEY: path ID, VALUE: path

        internal SchemePathCollection(WorkPlace workplace, Scheme scheme)
        {
            this.workplace = workplace;
            this.scheme = scheme;
            usableID = 0;
            items = new Dictionary<int, SchemePath>();
        }

        /// <summary>
        /// Removes provided paths and all their physical representations.
        /// Also adds all related SchemeSources to Scheme.Sources.UninitializedSources.
        /// </summary>
        /// <param name="paths"></param>
        internal void Remove(SchemePath[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
                Remove(paths[i]);
        }

        /// <summary>
        /// Removes provided path and all its physical representations.
        /// Also adds all related SchemeSources to Scheme.Sources.UninitializedSources.
        /// </summary>
        /// <param name="path"></param>
        internal void Remove(SchemePath path)
        {
            if (path == null)
                return;
            //Do nothing if path do not exists anymore.
            if (path.NoLongerInUse)
                return;
            path.NoLongerInUse = true;

            //Loop trough all path inputs.
            foreach (SchemeSource sSource in path.Inputs)
            {
                //Remove pointer to this path.
                sSource.IsInputIn = null;

                //Add SchemeSource to uninitialized collection.
                scheme.Sources.UninitializedSources.Add(sSource);
            }
            //Same thing with path outputs.
            foreach (SchemeSource sSource in path.Outputs)
            {
                sSource.IsOutputIn = null;
                scheme.Sources.UninitializedSources.Add(sSource);
            }
            //Remove path from this structure.
            this.items.Remove(path.ID);
            //Remove physical representations of this path.
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.scheme);
            foreach (PhysScheme pScheme in pSchemes)
                pScheme.Paths.Remove(path.ID);
        }

        /// <summary>
        /// Creates physical representation for each of paths in this Scheme.
        /// </summary>
        /// <param name="pScheme"></param>
        internal void CreatePhysRepresentation(WorkPlace workplace, PhysScheme pScheme)
        {
            foreach (KeyValuePair<int, SchemePath> entry in items)
            {
                PhysPath pPath = new PhysPath(entry.Value, pScheme);
                pScheme.Paths.Add(entry.Key, pPath);
                workplace.Simulation.Events.AddPath(pPath);
            }
        }

        /// <summary>
        /// Creates new Path.
        /// Stores it in Scheme and creates physical representation to each of PhysSchemes.
        /// </summary>
        /// <returns></returns>
        internal SchemePath Create()
        {
            //Add to this structure.
            SchemePath toReturn = new SchemePath(usableID++);
            items.Add(toReturn.ID, toReturn);

            //Get all PhysSchemes of this Scheme and create new PhysPath to each of them.
            //Also add newly created PhysPaths to simulation.
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(this.scheme);
            foreach (PhysScheme pScheme in pSchemes)
            {
                PhysPath pPath = new PhysPath(toReturn, pScheme);
                pScheme.Paths.Add(toReturn.ID, pPath);
                workplace.Simulation.Events.AddPath(pPath);
            }
            return toReturn;
        }

        internal void Debug()
        {
            if (this.items.Count > 0)
            {
                Debugger.Break();
            }
        }
    }
}
