using CP_Engine.SchemeItems;
using System.Collections.Generic;

namespace CP_Engine.MapItems
{
    class SchemePath
    {
        /// <summary>
        /// Sources that are inputs to this path.
        /// </summary>
        internal List<SchemeSource> Inputs { get; private set; }

        /// <summary>
        /// Sources that are outputs of this path.
        /// </summary>
        internal List<SchemeSource> Outputs { get; private set; }

        /// <summary>
        /// Determines if data stored in this path are still valid and whether this path is stored in scheme path-collection.
        /// </summary>
        internal bool NoLongerInUse { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        internal int ID { get; private set; }

        internal SchemePath(int id)
        {
            this.ID = id;
            Inputs = new List<SchemeSource>();
            Outputs = new List<SchemeSource>();
            NoLongerInUse = false;
        }

        /// <summary>
        /// Gets value of this path in provided PhysScheme.
        /// </summary>
        /// <param name="pScheme">PhysScheme, which data will be used to determine return value.</param>
        /// <returns></returns>
        internal bool GetValue(PhysScheme pScheme)
        {
            if (NoLongerInUse)
                return false;
            foreach (SchemeSource sSource in Inputs)
            {
                if (sSource.GetValue(pScheme))
                    return true;
            }
            return false;
        }
    }
}
