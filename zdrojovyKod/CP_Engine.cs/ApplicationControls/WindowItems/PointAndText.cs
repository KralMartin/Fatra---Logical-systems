using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine.cs.ApplicationControls.WindowItems
{
    struct PointAndText
    {
        Point tileOffset;
        string text;

        internal PointAndText(Point tileOffset, string text)
        {
            this.tileOffset = tileOffset;
            this.text = text;
        }
    }
}
