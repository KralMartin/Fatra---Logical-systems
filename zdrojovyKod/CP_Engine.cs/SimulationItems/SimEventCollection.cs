using CP_Engine.SchemeItems;
using System.Collections.Generic;

namespace CP_Engine.SimulationItems
{
    /// <summary>
    /// Collection of Events.
    /// </summary>
    class SimEventCollection
    {
        List<EventChangeValue> ecvCollection;
        List<PhysPath> paths;
        Simulation simulation;

        internal SimEventCollection(Simulation simulation)
        {
            this.simulation = simulation;
            ecvCollection = new List<EventChangeValue>();
            paths = new List<PhysPath>();
        }

        /// <summary>
        /// Add path to structure.
        /// Path wont be added to structure if it is allready in.
        /// Path.Execute function will be called over inserted path on next simulation step.
        /// </summary>
        /// <param name="pPath"></param>
        internal void AddPath(PhysPath pPath)
        {
            if (pPath.IsInQue)
                return;
            paths.Add(pPath);
            pPath.IsInQue = true;
        }

        /// <summary>
        /// Returns all paths.
        /// Clears inner collection of paths.
        /// </summary>
        /// <returns></returns>
        internal List<PhysPath> GetPats()
        {
            List<PhysPath> toReturn = paths;
            paths = new List<PhysPath>();
            return toReturn;
        }

        /// <summary>
        /// Add Simulation event.
        /// </summary>
        /// <param name="ecv"></param>
        internal void AddValueChange(EventChangeValue ecv)
        {
            ecvCollection.Add(ecv);
        }

        /// <summary>
        /// Returns all events executed this step.
        /// Removes these events from inner structure.
        /// </summary>
        /// <returns></returns>
        internal List<EventChangeValue> GetEcvs()
        {
            List<EventChangeValue> toReturn = this.ecvCollection;
            ecvCollection = new List<EventChangeValue>();
            return toReturn;
        }
    }
}
