using CP_Engine.SchemeItems;
using System.Collections.Generic;

namespace CP_Engine.ProjectItems
{
    class SchemeStructureItem
    {
        /// <summary>
        /// Schemes that are parents to scheme represented by this structure.
        /// </summary>
        internal List<Scheme> Parents { get; private set; }

        /// <summary>
        /// All PhysSchemes representating this Scheme.
        /// </summary>
        internal List<PhysScheme> PSchemes { get; private set; }

        /// <summary>
        /// All placed bugs, that contain this Scheme.
        /// </summary>
        internal List<PlacedBug> PlacedBugs { get; private set; }

        internal SchemeStructureItem()
        {
            this.PSchemes = new List<PhysScheme>();
            this.PlacedBugs = new List<PlacedBug>();
            this.Parents = new List<Scheme>();
        }

    }
}
