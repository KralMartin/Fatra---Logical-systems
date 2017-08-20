using Microsoft.Xna.Framework;

namespace CP_Engine.SchemeItems
{
    struct ExactGridPosition
    {
        internal Point Coords { get; set; }
        internal int Floor { get; set; }

        internal ExactGridPosition(Point coords, int floor)
        {
            this.Coords = coords;
            this.Floor = floor;
        }
    }
}
