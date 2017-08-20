using CP_Engine.SimulationItems;

namespace CP_Engine.SchemeItems
{
    /// <summary>
    /// Basic SchemeSource. 
    /// Representates simple konvertors and diodes.
    /// </summary>
    class BasicSchemeSource : SchemeSource
    {
        /// <summary>
        /// TRUE: Konvertor, FALSE: Diode
        /// </summary>
        bool isKonvertor;

        internal BasicSchemeSource(Scheme scheme, ExactGridPosition exactPosition, int id, bool isKonvertor)
        {
            Initialize(exactPosition, scheme, id);
            this.isKonvertor = isKonvertor;
        }

        internal override bool UsesIsInputIn()
        {
            return true;
        }

        internal override bool UsesIsOutputIn()
        {
            return true;
        }

        internal override bool GetValue(PhysScheme pScheme)
        {
            if (this.NoLongerInUse)
                return false;
            return pScheme.Sources[this.Identifier].Value;
        }

        internal override void InputChanged(bool inputValue, PhysScheme pScheme, Simulation sim)
        {
            if (this.isKonvertor)
                inputValue = !inputValue;
            PhysSource pSource = pScheme.Sources[this.Identifier];
            sim.Events.AddValueChange(new EventChangeValuePhysSource(pSource, sim.Step, inputValue));
        }

        internal override void ValueChanged(EventChangeValuePhysSource eve, Simulation sim)
        {
            if (this.NoLongerInUse)
                return;
            if (eve.PSource.Value != eve.NewValue)
            {
                eve.PSource.Value = eve.NewValue;
                PhysPath pPath = eve.PSource.PhysScheme.Paths[this.IsInputIn.ID];
                sim.Events.AddPath(pPath);
            }
        }
    }
}
