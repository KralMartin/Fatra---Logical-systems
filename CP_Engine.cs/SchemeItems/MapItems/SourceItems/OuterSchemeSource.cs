using CP_Engine.BugItems;
using CP_Engine.SimulationItems;

namespace CP_Engine.SchemeItems
{
    /// <summary>
    /// SchemeSource invisible to user. SchemeSource is part of PlacedBug.
    /// Can be outer Input or outer Output of Scheme.
    /// </summary>
    class OuterSchemeSource : SchemeSource
    {
        ExactGridPosition innerSourceCoords;    //Position where to find InnerSSource
        PlacedBug pBug;                         //PBug, in which this instance is stored.
        bool isInput;

        internal OuterSchemeSource(ExactGridPosition innerSourceCoords, bool isInput, ExactGridPosition position, PlacedBug pBug)
        {
            this.pBug = pBug;
            this.innerSourceCoords = innerSourceCoords;
            this.isInput = isInput;
            this.Initialize(position, pBug.ParentScheme, pBug.ID);
        }

        internal override bool UsesIsInputIn()
        {
            return !isInput;
        }

        internal override bool UsesIsOutputIn()
        {
            return isInput;
        }

        internal override bool GetValue(PhysScheme pScheme)
        {
            if (this.NoLongerInUse)
                return false;
            SchemeSource innerSchemeSource = this.pBug.Bug.IODecription.Get_Description(this.innerSourceCoords.Coords).SchemeSourcesOnCoords[this.innerSourceCoords.Floor];
            return innerSchemeSource.GetValue(pScheme, this);
        }

        internal override void InputChanged(bool inputValue, PhysScheme pScheme, Simulation sim)
        {
            SchemeSource innerSchemeSource = this.pBug.Bug.IODecription.Get_Description(this.innerSourceCoords.Coords).SchemeSourcesOnCoords[this.innerSourceCoords.Floor];
            innerSchemeSource.OuterInputChanged(inputValue, pScheme, this, sim);
        }

        internal override void ValueChanged(EventChangeValuePhysSource eve, Simulation sim)
        {
            if (NoLongerInUse)
                return;
            PhysPath pPath = eve.PSource.PhysScheme.ParentPScheme.Paths[this.IsInputIn.ID];
            sim.Events.AddPath(pPath);
        }

        internal IODescription GetInnerSourceDescription()
        {
            return this.pBug.Bug.IODecription.Get_Description(this.innerSourceCoords.Coords);
        }
    }
}
