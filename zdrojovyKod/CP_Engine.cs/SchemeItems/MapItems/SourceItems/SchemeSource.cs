using CP_Engine.MapItems;
using CP_Engine.SimulationItems;

namespace CP_Engine.SchemeItems
{
    class SchemeSource
    {
        /// <summary>
        /// TRUE: This instance is no longer in use and its data may non-existing references.
        /// </summary>
        internal bool NoLongerInUse { get; set; }

        /// <summary>
        /// Exact position in Scheme.
        /// </summary>
        internal ExactGridPosition Position { get; private set; }

        /// <summary>
        /// Scheme that stores this instance.
        /// </summary>
        internal Scheme Scheme { get; private set; }

        /// <summary>
        /// Do not has to be unique.
        /// This is required to find Placed bug or PhysSource, based on instance type.
        /// </summary>
        internal int Identifier { get; private set; }

        /// <summary>
        /// Path, that is affected by value of this PhysSource.
        /// </summary>
        internal SchemePath IsInputIn { get; set; }

        /// <summary>
        /// Value of this PhysSource is affected by this Path.
        /// </summary>
        internal SchemePath IsOutputIn { get; set; }

        protected void Initialize(ExactGridPosition position, Scheme scheme, int id)
        {
            this.Position = position;
            this.Scheme = scheme;
            this.Identifier = id;
        }

        /// <summary>
        /// Determines whether this instance uses IsInputIn variabile.
        /// </summary>
        /// <returns></returns>
        internal virtual bool UsesIsInputIn() { return false; }

        /// <summary>
        /// Determines whether this instance uses IsOutputIn variabile.
        /// </summary>
        /// <returns></returns>
        internal virtual bool UsesIsOutputIn() { return false; }

        /// <summary>
        /// Returns value of phys representation of this SchemeSource.
        /// Executed only if this instance is InnerSchemeSource.
        /// Function is called only from OuterSchemeSource.
        /// </summary>
        /// <param name="parentPScheme">Parent PScheme, where outer SSource is placed.</param>
        /// <param name="outerSchemeSource"></param>
        /// <returns></returns>
        internal virtual bool GetValue(PhysScheme parentPScheme, SchemeSource outerSchemeSource) { return false; }

        /// <summary>
        /// Returns value of phys representation of this SchemeSource.
        /// Overriden only by OutterSchemeSource, as they have not PSource, they call GetValue of their inner SSource.
        /// </summary>
        /// <param name="pScheme">PhysScheme</param>
        /// <returns></returns>
        internal virtual bool GetValue(PhysScheme pScheme) { return false; }

        /// <summary>
        /// Value of path on input of this SchemeSource was changed.
        /// </summary>
        /// <param name="inputValue">New value.</param>
        /// <param name="pScheme">PhysScheme on which input was changed.</param>
        /// <param name="sim">Simulation</param>
        internal virtual void InputChanged(bool inputValue, PhysScheme pScheme, Simulation sim) { }

        /// <summary>
        /// Value of path on input of this SchemeSource was changed.
        /// Executed only if this instance is InnerSchemeSource.
        /// Function is called only from OuterSchemeSource.
        /// </summary>
        /// <param name="inputValue">New value.</param>
        /// <param name="parentPScheme">Parent PhysScheme.</param>
        /// <param name="outerSSource">Outer SchemeSource</param>
        /// <param name="sim">Simulation</param>
        internal virtual void OuterInputChanged(bool inputValue, PhysScheme parentPScheme, OuterSchemeSource outerSSource, Simulation sim) { }

        /// <summary>
        /// Value of this input has changed.
        /// When input value of this SchemeSource was changed, InputChanged function is called. 
        /// That function causes execution of this function. So:
        /// InputChanged: acknowledges this input, that value has been changed.
        /// ValueChanged: actually changes value of phys-representation of this SchemeSource.
        /// </summary>
        /// <param name="eve">Parameters.</param>
        /// <param name="sim"></param>
        internal virtual void ValueChanged(EventChangeValuePhysSource eve, Simulation sim) { }
    }
}
