using System.Collections.Generic;

namespace CP_Engine.MapItems
{
    enum TileTypes
    {
        /// <summary>
        /// X connected in middle, L,I,_ shaped tile. Doos not use TileData.Offset.
        /// </summary>
        Vire,
        /// <summary>
        /// X not connected in middle, T shaped tiles. Uses TileData.Offset. 
        /// Horz and Vert widths are not dependant on each other.
        /// </summary>
        ComposedVire,
        /// <summary>
        /// Konverts signal.
        /// </summary>
        Konvertor,
        /// <summary>
        /// (Diode) Allows signal flow only in one direction.
        /// </summary>
        Repeater,
        /// <summary>
        /// Input to scheme.
        /// </summary>
        Input,
        /// <summary>
        /// Output from scheme
        /// </summary>
        Output
    }

    /// <summary>
    /// Contains information about all allowed tiles.
    /// </summary>
    static class TilesInfo
    {
        static TileInfoItem[] items;

        internal static void Initialize()
        {
            List<TileInfoItem> temporaryList = new List<TileInfoItem>();
            for (int i = 0; i <= 22; i++)
            {
                temporaryList.Add(new TileInfoItem(i));
            }
            items = temporaryList.ToArray();
        }

        /// <summary>
        /// Returns tile-info of provided type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static TileInfoItem GetItem(int type)
        {
            return items[type];
        }

        /// <summary>
        /// X shaped tile without connection in middle.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsType7(int type)
        {
            // 1
            //1 1
            // 1
            if (type == 7)
                return true;
            return false;
        }

        /// <summary>
        /// X shaped tile with connection in middle.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsType12(int type)
        {
            // 1
            //1x1
            // 1
            if (type == 12)
                return true;
            return false;
        }

        /// <summary>
        /// Input tile.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsType21(int type)
        {
            if (type == 21)
                return true;
            return false;
        }

        /// <summary>
        /// Output tile.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsType22(int type)
        {
            if (type == 22)
                return true;
            return false;
        }

        /// <summary>
        /// Empty tile.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsType0(int type)
        {
            if (type == 0)
                return true;
            return false;
        }

        /// <summary>
        /// But type tile.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsBugType(int type)
        {
            if (type == 99)
                return true;
            return false;
        }

        /// <summary>
        /// Compares some properties of template to all avalible TileInfoItems 
        /// and returns TileInfoItem, that share same properties.
        /// Not all properties are compared.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static TileInfoItem GetExactTemplate(TileInfoItem item)
        {
            foreach (TileInfoItem template in items)
            {
                if (item.MyEquals(template))
                    return template;
            }
            return items[0];
        }

        /// <summary>
        /// Returns output side of tile.
        /// Relevant only on TileTypes { Konvertor, Repeater, Input, Output }
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static int GetOutputSide(int type)
        {
            if (TilesInfo.IsType21(type))
                return Sides.Right;
            if (TilesInfo.IsType22(type))
                return -1;
            switch (type)
            {
                case 13:
                    return Sides.Top;
                case 14:
                    return Sides.Right;
                case 15:
                    return Sides.Bot;
                case 16:
                    return Sides.Left;
                case 17:
                    return Sides.Top;
                case 18:
                    return Sides.Right;
                case 19:
                    return Sides.Bot;
                case 20:
                    return Sides.Left;
                case 99:
                    return Sides.Bot;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Returns input side of tile.
        /// Relevant only on TileTypes { Konvertor, Repeater, Input, Output }
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static int GetInputSide(int type)
        {
            if (TilesInfo.IsType21(type))
                return -1;
            if (TilesInfo.IsType22(type))
                return Sides.Left;
            return Sides.OpositeSide(GetOutputSide(type));
        }
    }
}
