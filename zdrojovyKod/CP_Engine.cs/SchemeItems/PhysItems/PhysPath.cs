using CP_Engine.MapItems;

namespace CP_Engine.SchemeItems
{
    class PhysPath
    {
        /// <summary>
        /// Current value of this instance.
        /// </summary>
        internal bool Value { get; set; }

        /// <summary>
        /// Determines whether this instance is stored in Simulation event collection.
        /// </summary>
        internal bool IsInQue { get; set; }

        /// <summary>
        /// SchemePath bound to this instance.
        /// </summary>
        internal SchemePath SchemePath { get; private set; }

        private PhysScheme pScheme;     // PhysScehme in which is thsi instance stored.
        private bool hasBeenExecuted;   // Determines whether Execute function was called over thsi instance.

        internal PhysPath(SchemePath schemePath, PhysScheme pScheme)
        {
            this.SchemePath = schemePath;
            this.pScheme = pScheme;
            this.hasBeenExecuted = false;
        }

        /// <summary>
        /// Called by Simulation class.
        /// Get value from input and acknowledge outputs aboud value changed.
        /// </summary>
        /// <param name="sim"></param>
        internal void Execute(Simulation sim)
        {
            if (this.SchemePath.NoLongerInUse)
                return;
            //Path was executed by simulation and will be removed from simulation event collection.
            this.IsInQue = false;

            //Get value from all inputs.
            bool newValue = false;
            foreach (SchemeSource sSource in this.SchemePath.Inputs)
            {
                if (sSource.GetValue(this.pScheme))
                {
                    newValue = true;
                    break;
                }
            }
            //If value was changed (or if this function is called first time), acknowledge all Outputs bound to this path.
            if (hasBeenExecuted == false)
            {
                hasBeenExecuted = true;
                this.Value = newValue;
                AcknowledgePathOutputs(sim);
            }
            else if (newValue != this.Value)
            {
                this.Value = newValue;
                AcknowledgePathOutputs(sim);
            }
        }

        /// <summary>
        /// Inserts all Outputs of this path to Simulation.
        /// </summary>
        /// <param name="sim"></param>
        private void AcknowledgePathOutputs(Simulation sim)
        {
            foreach (SchemeSource sSource in this.SchemePath.Outputs)
                sSource.InputChanged(this.Value, this.pScheme, sim);
        }
    }
}
