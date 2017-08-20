
namespace CP_Engine.SchemeItems
{
    class SpecialPhysScheme
    {
        /// <summary>
        /// Parent PhysScheme.
        /// </summary>
        internal PhysScheme ParentPScheme { get; private set; }

        /// <summary>
        /// PlacedBug represented by this instance.
        /// </summary>
        internal PlacedBug PlacedBug { get; private set; }

        /// <summary>
        /// Bool values, used to fully recover state of SpecialBug.
        /// </summary>
        internal bool[] Values { get; set; }

        internal int Number { get; set; }

        /// <summary>
        /// This instance has been removed from scheme.
        /// </summary>
        internal bool NoLongerInUse { get; set; }

        internal SpecialPhysScheme(PlacedBug placedBug, PhysScheme parentScheme)
        {
            this.ParentPScheme = parentScheme;
            this.PlacedBug = placedBug;
            this.Values = new bool[placedBug.Bug.GetValueSize()];
        }
    }
}
