using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    class Default
    {
        public const int TileWidth = 2;

        //Elevation Generator
        public const int MaxElevation = 5;
        public const int TriangularRandomCoef = 4;
        public const int SmoothingCount = 3;
        public const int SmoothingRadius = 1;
        
        public static Rectangle GridRectangle()
        {
            return new Rectangle(0, 0, 1000, 800);
        }
    }
}
