using Microsoft.Xna.Framework;

namespace CP_Engine
{
    class Sides
    {
        internal const int Top = 0;
        internal const int Right = 1;
        internal const int Bot = 2;
        internal const int Left = 3;

        /// <summary>
        /// Alters coordinates based on provided side.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        internal static Point Move(Point coords, int side)
        {
            switch (side)
            {
                case Sides.Top:
                    coords.Y--;
                    break;
                case Sides.Bot:
                    coords.Y++;
                    break;
                case Sides.Right:
                    coords.X++;
                    break;
                case Sides.Left:
                    coords.X--;
                    break;
            }
            return coords;
        }

        internal static bool IsHorizontal(int side)
        {
            if (side == Left || side == Right)
                return true;
            return false;
        }

        /// <summary>
        /// Returns opposite side to provided side.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        internal static int OpositeSide(int side)
        {
            switch (side)
            {
                case Top:
                    return Bot;
                case Right:
                    return Left;
                case Bot:
                    return Top;
                case Left:
                    return Right;
                default:
                    return 0;
            }
        }
    }
}
