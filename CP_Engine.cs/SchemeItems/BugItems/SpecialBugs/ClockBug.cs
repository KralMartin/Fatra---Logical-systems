using CP_Engine.SchemeItems;
using CP_Engine.SimulationItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CP_Engine.BugItems
{
    class ClockBug : Bug
    {
        internal const int OutputIdentifier = 3;

        SchemeSource output;

        internal ClockBug() : base(2)
        {
            this.Title = "Clock";
            this.Description = "Toogles between 0 and 1.";
            
            output = new SpecialInnerSchemeSource(OutputIdentifier, false, this);

            List<SchemeSource> sourcesOnCoords = new List<SchemeSource>();
            sourcesOnCoords.Add(output);
            this.IODecription.Add(new IODescription(false, "O", "Output", 0, new Point(3, 0), sourcesOnCoords));
        }

        internal override void SpecialPhysSchemeCreated(SpecialPhysScheme pScheme, Simulation sim)
        {
            EventChangeValueSpecialBug ecv = new EventChangeValueSpecialBug(pScheme, null, sim.Step, false);
            sim.Events.AddValueChange(ecv);
        }

        internal override void ValueChanged(EventChangeValueSpecialBug ecv, Simulation sim)
        {
            ecv.PhysScheme.Number++;
            if (ecv.PhysScheme.Number >= ecv.PhysScheme.PlacedBug.Number)
            {
                ecv.PhysScheme.Number = 0;
                ecv.PhysScheme.Values[0] = !ecv.PhysScheme.Values[0];

                //Get outer SSource.
                SchemeSource source = ecv.PhysScheme.PlacedBug.SchemeSources[this.output.Identifier];
                //Get path leading from outer SSource.
                PhysPath pPath = ecv.PhysScheme.ParentPScheme.Paths[source.IsInputIn.ID];
                //Create event. 
                sim.Events.AddPath(pPath);
            }
            EventChangeValueSpecialBug ecv2 = new EventChangeValueSpecialBug(ecv.PhysScheme, null, sim.Step, false);
            sim.Events.AddValueChange(ecv2);
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
                return "Clk(1)";
            else
                return "Clk(0)";
        }

        internal override int GetValueSize()
        {
            return 1;
        }
    }
}
