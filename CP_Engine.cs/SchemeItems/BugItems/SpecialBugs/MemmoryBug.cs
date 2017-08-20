using CP_Engine.SchemeItems;
using CP_Engine.SimulationItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CP_Engine.BugItems
{
    class MemmoryBug : Bug
    {
        internal const int OutputIdentifier = 3;

        SchemeSource input, reset, output;

        internal MemmoryBug() : base(0)
        {
            this.Title = "Memmory";
            this.Description = "1 bit memmory";

            input = new SpecialInnerSchemeSource(1, true, this);
            reset = new SpecialInnerSchemeSource(2, true, this);
            output = new SpecialInnerSchemeSource(OutputIdentifier, false, this);

            List<SchemeSource> sourcesOnCoords = new List<SchemeSource>();
            sourcesOnCoords.Add(input);
            this.IODecription.Add(new IODescription(true, "I", "Input", 0, new Point(1, 0), sourcesOnCoords));

            sourcesOnCoords = new List<SchemeSource>();
            sourcesOnCoords.Add(reset);
            this.IODecription.Add(new IODescription(true, "R", "Reset", 1, new Point(2, 0), sourcesOnCoords));

            sourcesOnCoords = new List<SchemeSource>();
            sourcesOnCoords.Add(output);
            this.IODecription.Add(new IODescription(false, "O", "Output", 0, new Point(3, 0), sourcesOnCoords));
        }

        internal override void OuterInputChanged(bool inputValue, PhysScheme parentPScheme, OuterSchemeSource outerSSource, SpecialInnerSchemeSource innerSSource, Simulation sim)
        {
            //Get pScheme.
            SpecialPhysScheme pScheme = parentPScheme.SpecialChildren[outerSSource.Identifier];
            //Get input value.
            SchemeSource source = pScheme.PlacedBug.SchemeSources[this.input.Identifier];
            bool inputVal = source.IsOutputIn.GetValue(parentPScheme);
            //Get reset value.
            source = pScheme.PlacedBug.SchemeSources[this.reset.Identifier];
            bool resetVal = source.IsOutputIn.GetValue(parentPScheme);
            //Get current value.
            bool newVal = pScheme.Values[0];

            if (newVal == false && inputVal)
                newVal = true;
            if (resetVal)
                newVal = false;
            //When value was changed, create new event.
            if (pScheme.Values[0] != newVal)
                sim.Events.AddValueChange(new EventChangeValueSpecialBug(pScheme, null, sim.Step, newVal));
        }

        internal override void ValueChanged(EventChangeValueSpecialBug ecv, Simulation sim)
        {            
            if (ecv.PhysScheme.Values[0] == ecv.NewValue)
                return;
            //Change value.
            ecv.PhysScheme.Values[0] = ecv.NewValue;
            //Get outer SSource.
            SchemeSource source = ecv.PhysScheme.PlacedBug.SchemeSources[this.output.Identifier];
            //Get path leading from outer SSource.
            PhysPath pPath = ecv.PhysScheme.ParentPScheme.Paths[source.IsInputIn.ID];
            //Create event. 
            sim.Events.AddPath(pPath);
        }
        
        internal override bool GetInnerValue(PhysScheme parentPScheme, SchemeSource outerSchemeSource, SchemeSource innerSchemeSource)
        {
            SpecialPhysScheme pScheme = parentPScheme.SpecialChildren[outerSchemeSource.Identifier];
            return pScheme.Values[0];
        }

        internal override string GetDisplayText(PhysScheme parentScheme, PlacedBug pBug)
        {
            SpecialPhysScheme pScheme = parentScheme.SpecialChildren[pBug.ID];
            if (pScheme.Values[0])
                return "Mem(1)";
            else
                return "Mem(0)";
        }

        internal override int GetValueSize()
        {
            return 1;
        }
    }
}
