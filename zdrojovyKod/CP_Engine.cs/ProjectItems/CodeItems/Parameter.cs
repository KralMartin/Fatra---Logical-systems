namespace CP_Engine.cs.ProjectItems.CodeItems
{
    class Parameter
    {
        internal int NumberLenght { get; set; }
        internal char Name { get; set; }
        internal int Adjust { get; set; }
        internal int Index { get; set; }

        public Parameter(char name, int index)
        {
            this.Name = name;
            this.Index = index;
        }

    }
}
