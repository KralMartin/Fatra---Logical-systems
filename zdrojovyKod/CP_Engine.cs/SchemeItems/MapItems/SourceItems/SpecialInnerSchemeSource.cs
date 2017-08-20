namespace CP_Engine.SchemeItems
{
    class SpecialInnerSchemeSource : SchemeSource
    {
        bool isInput;
        Bug bug;

        internal SpecialInnerSchemeSource(int id, bool isInput, Bug bug)
        {
            this.isInput = isInput;
            Initialize(new ExactGridPosition(), null, id);
            this.bug = bug;
        }

        internal override void OuterInputChanged(bool inputValue, PhysScheme parentPScheme, OuterSchemeSource outerSSource, Simulation sim)
        {
            this.bug.OuterInputChanged(inputValue, parentPScheme, outerSSource, this, sim);
        }

        internal override bool GetValue(PhysScheme parentPScheme, SchemeSource outerSchemeSource)
        {
            return this.bug.GetInnerValue(parentPScheme, outerSchemeSource, this);
        }

    }
}
