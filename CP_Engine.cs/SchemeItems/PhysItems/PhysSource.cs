
namespace CP_Engine.SchemeItems
{
    class PhysSource
    {
        /// <summary>
        /// SchemeSource, that is represented by this instance.
        /// </summary>
        internal SchemeSource SchemeSource { get; private set; }

        /// <summary>
        /// PhysScheme in which is this incance stored.
        /// </summary>
        internal PhysScheme PhysScheme { get; private set; }

        /// <summary>
        /// Value of this instance.
        /// </summary>
        internal bool Value { get; set; }

        internal PhysSource(PhysScheme physScheme, SchemeSource schemeSource, bool defValue)
        {
            this.Value = defValue;
            this.PhysScheme = physScheme;
            this.SchemeSource = schemeSource;
        }

    }
}
