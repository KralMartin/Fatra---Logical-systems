namespace CP_Engine.MapItems
{
    struct TileSide
    {
        internal bool UsesOffset { get; private set; }
        internal bool IsUsed { get; private set; }

        internal TileSide(bool isUsed, bool isPrimary = true)
        {
            this.UsesOffset = !isPrimary;
            this.IsUsed = isUsed;
        }

        internal TileSide Coppy()
        {
            return new TileSide(IsUsed, UsesOffset);
        }

        public override string ToString()
        {
            if (IsUsed == false)
                return "0";
            if (UsesOffset)
                return "1";
            return "2";
        }
    }
}
