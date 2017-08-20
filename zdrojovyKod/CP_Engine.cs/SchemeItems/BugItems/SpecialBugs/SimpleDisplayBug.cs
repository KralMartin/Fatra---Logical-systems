using CP_Engine.SchemeItems;
using CP_Engine.SimulationItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CP_Engine.BugItems
{
    class SimpleDisplayBug : Bug
    {
        public SimpleDisplayBug() : base(1)
        {
            this.Title = "Simple display";
            this.Description = "Shows number";

            List<SchemeSource> sourcesOnCoords = new List<SchemeSource>();
            for (int i = 0; i < 8; i++)
                sourcesOnCoords.Add(new SpecialInnerSchemeSource(i, true, this));
            this.IODecription.Add(new IODescription(true, "I", "Input", 0, new Point(), sourcesOnCoords));
        }

        internal override void OuterInputChanged(bool inputValue, PhysScheme parentPScheme, OuterSchemeSource outerSSource, SpecialInnerSchemeSource innerSSource, Simulation sim)
        {
            //Get pScheme.
            SpecialPhysScheme pScheme = parentPScheme.SpecialChildren[outerSSource.Identifier];
            //Get old input value.
            bool oldValue = pScheme.Values[innerSSource.Identifier];
            //Get new Input value.
            bool newValue = outerSSource.IsOutputIn.GetValue(parentPScheme);
            if (oldValue != newValue)
                sim.Events.AddValueChange(new EventChangeValueSpecialBug(pScheme, innerSSource, sim.Step, newValue));
        }

        internal override void ValueChanged(EventChangeValueSpecialBug ecv, Simulation sim)
        {
            if (ecv.PhysScheme.Values[ecv.InnerSchemeSource.Identifier] == ecv.NewValue)
                return;
            ecv.PhysScheme.Values[ecv.InnerSchemeSource.Identifier] = ecv.NewValue;
        }

        internal override string GetDisplayText(PhysScheme parentScheme, PlacedBug pBug)
        {
            SpecialPhysScheme pScheme = parentScheme.SpecialChildren[pBug.ID];
            return BinaryMath.ToDecimal(pScheme.Values);
        }

        internal override int GetValueSize()
        {
            return 8;
        }
    }
}
