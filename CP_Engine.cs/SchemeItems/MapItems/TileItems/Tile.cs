using CP_Engine.SchemeItems;
using System;

namespace CP_Engine.MapItems
{
    class Tile
    {
        /// <summary>
        /// Data describing this tile.
        /// </summary>
        internal TileData Data { get; private set; }

        /// <summary>
        /// Paths, contained in this instance.
        /// </summary>
        internal SchemePath[] Paths { get; private set; }

        internal BreakPoint BreakPoint { get; set; }

        internal Tile(TileData data)
        {
            Set_Data(data);
        }
        
        internal void Set_Data(TileData data)
        {
            this.Data = data;
            if (TilesInfo.IsBugType(data.Type))
                return;

            //Calculate total size of Paths array.
            TileInfoItem info = TilesInfo.GetItem(data.Type);
            int horzWidth = 0;
            if (info.UsesHorizontal())
            {
                horzWidth = data.HorzWidth;
                if (info.OffsetHorizontal)
                    horzWidth += data.Offset;
            }
            int vertWidth = 0;
            if (info.UsesVertical())
            {
                vertWidth = data.VertWidth;
                if (info.OffsetVertical)
                    vertWidth += data.Offset;
            }
            int offsetAdjust = 0;
            if (data.Offset < 0)
                offsetAdjust = data.Offset;
            Paths = new SchemePath[Math.Max(horzWidth, vertWidth) - offsetAdjust];
        }

        /// <summary>
        /// Returns value of each path in array.
        /// </summary>
        /// <param name="pScheme"></param>
        /// <returns></returns>
        internal bool[] GetValues(PhysScheme pScheme)
        {
            bool[] toReturn = new bool[Paths.Length];
            for (int i = 0; i < Paths.Length; i++)
            {
                //Path can be null (for example, this is newly created tile).
                if (Paths[i] != null)
                {
                    if (Paths[i].NoLongerInUse)
                        toReturn[i] = false;
                    else
                        toReturn[i] = pScheme.Paths[Paths[i].ID].Value;
                }
            }
            return toReturn;
        }

        internal void Set_ColorID(int colorID)
        {
            TileData tmp = this.Data;
            tmp.ColorID = colorID;
            this.Data = tmp;
        }
    }
}
