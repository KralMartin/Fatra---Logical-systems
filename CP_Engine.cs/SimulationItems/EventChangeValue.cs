using CP_Engine.SchemeItems;

namespace CP_Engine.SimulationItems
{
    /// <summary>
    /// Determines when and to what value will be something changed.
    /// What will be changed is described in descendants of this class.
    /// </summary>
    class EventChangeValue
    {
        internal bool NewValue { get; private set; }
        internal int Step { get; private set; }

        protected EventChangeValue(int step, bool newValue)
        {
            this.Step = step;
            this.NewValue = newValue;
        }

        internal virtual void Execute(Simulation sim) { }
    }

    class EventChangeValuePhysSource : EventChangeValue
    {
        internal PhysSource PSource { get; private set; }

        internal EventChangeValuePhysSource(PhysSource pSource, int step, bool newValue) : base(step, newValue)
        {
            this.PSource = pSource;
        }

        internal override void Execute(Simulation sim)
        {
            this.PSource.SchemeSource.ValueChanged(this, sim);
        }
    }

    class EventChangeValueSpecialBug : EventChangeValue
    {
        internal SpecialPhysScheme PhysScheme { get; private set; }
        internal SchemeSource InnerSchemeSource { get; private set; }

        internal EventChangeValueSpecialBug(SpecialPhysScheme physScheme, SchemeSource innerSchemeSource, int step, bool newValue) : base(step, newValue)
        {
            this.PhysScheme = physScheme;
            this.InnerSchemeSource = innerSchemeSource;
        }

        internal override void Execute(Simulation sim)
        {
            if (this.PhysScheme.NoLongerInUse)
                return;
            this.PhysScheme.PlacedBug.Bug.ValueChanged(this, sim);
        }
    }
}
