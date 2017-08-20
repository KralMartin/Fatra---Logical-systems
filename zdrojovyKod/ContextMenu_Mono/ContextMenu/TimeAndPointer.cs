namespace ContextMenu_Mono.ContextMenu
{
    /// <summary>
    /// Helps to determine, when and which panel will be showed from button-link.
    /// </summary>
    class TimeAndPointer
    {
        public ContextButton Button { get; set; }
        public double Time { get; set; }
        public bool IsOn { get; set; }

        public bool Activate(double timeTotal)
        {
            if (IsOn && Time <= timeTotal)
            {
                IsOn = false;
                return true;
            }
            return false;
        }
    }
}
