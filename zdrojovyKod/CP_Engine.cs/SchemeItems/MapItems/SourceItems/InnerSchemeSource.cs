using CP_Engine.SimulationItems;

namespace CP_Engine.SchemeItems
{
    /// <summary>
    /// Representates inputs outputs in Scheme. 
    /// The one-block ones, that are visible for user in scheme.
    /// </summary>
    class InnerSchemeSource : SchemeSource
    {
        bool isInput;

        internal InnerSchemeSource(Scheme scheme, ExactGridPosition exactPosition, int id, bool isInput)
        {
            Initialize(exactPosition, scheme, id);
            this.isInput = isInput;
        }

        internal override bool UsesIsInputIn()
        {
            return isInput;
        }

        internal override bool UsesIsOutputIn()
        {
            return !isInput;
        }

        internal override bool GetValue(PhysScheme pScheme)
        {
            if (this.NoLongerInUse)
                return false;
            return pScheme.Sources[this.Identifier].Value;
        }

        internal override bool GetValue(PhysScheme parentPScheme, SchemeSource outerSchemeSource)
        {
            if (this.NoLongerInUse)
                return false;
            //This function is called only when isInput = FALSE.
            PhysScheme pScheme = parentPScheme.Children[outerSchemeSource.Identifier];
            PhysSource pSource = pScheme.Sources[this.Identifier];
            return pSource.Value;
        }

        internal override void OuterInputChanged(bool inputValue, PhysScheme parentPScheme, OuterSchemeSource outerSSource, Simulation sim)
        {
            //This function is called only when isInput = TRUE.
            PhysScheme pScheme = parentPScheme.Children[outerSSource.Identifier];
            PhysSource pSource = pScheme.Sources[this.Identifier];
            sim.Events.AddValueChange(new EventChangeValuePhysSource(pSource, sim.Step, inputValue));
        }

        internal override void InputChanged(bool inputValue, PhysScheme pScheme, Simulation sim)
        {
            //This function is called only when isInput = FALSE.
            PhysSource pSource = pScheme.Sources[this.Identifier];
            sim.Events.AddValueChange(new EventChangeValuePhysSource(pSource, sim.Step, inputValue));
        }

        internal override void ValueChanged(EventChangeValuePhysSource eve, Simulation sim)
        {
            if (this.NoLongerInUse)
                return;
            if (eve.PSource.Value != eve.NewValue)
            {
                //First change value of physSource.
                eve.PSource.Value = eve.NewValue;
                if (this.isInput)
                {
                    //This is input to Scheme.
                    //Acknowledge path leading from this SchemeSource.
                    PhysPath pPath = eve.PSource.PhysScheme.Paths[this.IsInputIn.ID];
                    sim.Events.AddPath(pPath);
                }
                else
                {
                    //This is output from Scheme.
                    //When PhysSCheme is top-scheme of project, it does have PlacedBug, but that PlacedBug has no ParentScheme.
                    if (eve.PSource.PhysScheme.ParentPScheme != null)
                    {
                        //Get outer SchemeSource bound to this SchemeSource.
                        SchemeSource outerSSource = eve.PSource.PhysScheme.PlacedBug.SchemeSources[this.Identifier];
                        //Acknowledge path leading from outer SchemeSource.
                        outerSSource.ValueChanged(eve, sim);
                    }
                }
            }
        }
    }
}
