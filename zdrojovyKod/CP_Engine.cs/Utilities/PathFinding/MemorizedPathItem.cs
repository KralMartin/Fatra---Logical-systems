using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    struct MemorizedPathItem
    {
        internal bool Horz_Vert { get; set; }
        internal Point Coords { get; set; }

        internal MemorizedPathItem(Point coords, bool horz_vert)
        {
            this.Coords = coords;
            this.Horz_Vert = horz_vert;
        }
    }
}
