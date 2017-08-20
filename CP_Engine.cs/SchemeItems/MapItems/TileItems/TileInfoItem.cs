namespace CP_Engine.MapItems
{
    /// <summary>
    /// Provides metadata to TileData structure.
    /// </summary>
    class TileInfoItem
    {
        /// <summary>
        /// Unique identifier.
        /// Type of this instance.
        /// </summary>
        internal int Type { get; set; }

        /// <summary>
        /// Collection of side settings.
        /// </summary>
        internal TileSide[] TileSide { get; private set; }

        /// <summary>
        /// Determines whether TileData.Offset is used to adjust TileData.HorzWidth.
        /// </summary>
        internal bool OffsetHorizontal { get; private set; }

        /// <summary>
        /// Determines whether TileData.Offset is used to adjust TileData.VertWidth.
        /// </summary>
        internal bool OffsetVertical { get; private set; }

        /// <summary>
        /// Tile type of this instance.
        /// </summary>
        internal TileTypes TileType { get; private set; }

        internal TileInfoItem(bool top, bool right, bool bot, bool left)
        {
            TileSide = new TileSide[4];
            TileSide[Sides.Top] = new TileSide(top);
            TileSide[Sides.Right] = new TileSide(right);
            TileSide[Sides.Bot] = new TileSide(bot);
            TileSide[Sides.Left] = new TileSide(left);
        }

        internal TileInfoItem()
        {
            TileSide = new TileSide[4];
        }

        /// <summary>
        /// Creates predefined tile-info.
        /// </summary>
        /// <param name="type"></param>
        internal TileInfoItem(int type)
        {
            TileSide = new TileSide[4];
            this.Type = type;
            this.TileType = TileTypes.Vire;
            switch (type)
            {
                case 1:
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(true, false);
                    break;
                case 2:
                    TileSide[Sides.Top] = new TileSide(true, false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 3:
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(true, false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 4:
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(true, false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 5:
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 6:
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 7:
                    TileSide[Sides.Top] = new TileSide(true, false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(true, false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 8:
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 9:
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 10:
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 11:
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 12:
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 13:
                    this.TileType = TileTypes.Konvertor;
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 14:
                    this.TileType = TileTypes.Konvertor;
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 15:
                    this.TileType = TileTypes.Konvertor;
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 16:
                    this.TileType = TileTypes.Konvertor;
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 17:
                    this.TileType = TileTypes.Repeater;
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 18:
                    this.TileType = TileTypes.Repeater;
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 19:
                    this.TileType = TileTypes.Repeater;
                    TileSide[Sides.Top] = new TileSide(true);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(true);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 20:
                    this.TileType = TileTypes.Repeater;
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
                case 21:
                    this.TileType = TileTypes.Input;
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(true);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(false);
                    break;
                case 22:
                    this.TileType = TileTypes.Output;
                    TileSide[Sides.Top] = new TileSide(false);
                    TileSide[Sides.Right] = new TileSide(false);
                    TileSide[Sides.Bot] = new TileSide(false);
                    TileSide[Sides.Left] = new TileSide(true);
                    break;
            }
            if (this.TileType == TileTypes.Vire)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (TileSide[i].IsUsed && TileSide[i].UsesOffset == true)
                    {
                        this.TileType = TileTypes.ComposedVire;
                        break;
                    }
                }
            }
            OffsetHorizontal = false;
            if ((this.TileSide[Sides.Left].IsUsed && this.TileSide[Sides.Left].UsesOffset) || (this.TileSide[Sides.Right].IsUsed && this.TileSide[Sides.Right].UsesOffset))
                OffsetHorizontal = true;

            OffsetVertical = false;
            if ((this.TileSide[Sides.Top].IsUsed && this.TileSide[Sides.Top].UsesOffset) || (this.TileSide[Sides.Bot].IsUsed && this.TileSide[Sides.Bot].UsesOffset))
                OffsetVertical = true;
        }

        /// <summary>
        /// TRUE: TileType is ComposedVire.
        /// </summary>
        /// <returns></returns>
        internal bool IsComposed()
        {
            if (this.TileType == TileTypes.ComposedVire)
                return true;
            return false;
        }

        /// <summary>
        /// TRUE: TileType is Vire {ComposedVire, Konvertor, Repeater, Input, Output }
        /// </summary>
        /// <returns></returns>
        internal bool IsSource()
        {
            if (this.TileType == TileTypes.Vire || this.TileType == TileTypes.ComposedVire)
                return false;
            return true;
        }

        /// <summary>
        /// TRUE: Tile uses left or right side.
        /// </summary>
        /// <returns></returns>
        internal bool UsesHorizontal()
        {
            return (TileSide[Sides.Left].IsUsed || TileSide[Sides.Right].IsUsed);
        }

        /// <summary>
        /// TRUE: Tile uses top or bottom side.
        /// </summary>
        /// <returns></returns>
        internal bool UsesVertical()
        {
            return (TileSide[Sides.Top].IsUsed || TileSide[Sides.Bot].IsUsed);
        }

        /// <summary>
        /// Returns true if provided tile-info has same settings like thsi one.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool MyEquals(TileInfoItem item)
        {
            if (item.IsSource())
                return false;
            for (int i = 0; i < TileSide.Length; i++)
            {
                if (this.TileSide[i].IsUsed != item.TileSide[i].IsUsed)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Coppies settings of tilesides to new instance.
        /// </summary>
        /// <returns></returns>
        internal TileInfoItem CoppySides()
        {
            TileInfoItem toReturn = new TileInfoItem();
            for (int i = 0; i < TileSide.Length; i++)
                toReturn.TileSide[i] = this.TileSide[i].Coppy();
            return toReturn;
        }

        /// <summary>
        /// Returns count of sides, used by this instance.
        /// </summary>
        /// <returns></returns>
        internal int CountOfUsedSides()
        {
            int toReturn = 0;
            for (int i = 0; i < 4; i++)
            {
                if (TileSide[i].IsUsed)
                    toReturn++;
            }
            return toReturn;
        }

        public override string ToString()
        {
            string toReturn = "";
            for (int i = 0; i < TileSide.Length; i++)
            {
                toReturn += TileSide[i].ToString() + " ";
            }
            return toReturn;
        }
    }
}
