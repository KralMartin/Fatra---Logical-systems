using CP_Engine.MapItems;
using Microsoft.Xna.Framework;

namespace CP_Engine.WorkplaceAssistants
{
    class OffsetAssistant
    {
        WorkPlace workplace;

        internal OffsetAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        /// <summary>
        /// Increments offset on provided coords by 1.
        /// </summary>
        /// <param name="coords"></param>
        internal void IncrementOffset(Point coords)
        {
            ChangeOffset(coords, true);
        }

        /// <summary>
        /// Decrements offset on provided coords by 1.
        /// </summary>
        /// <param name="coords"></param>
        internal void DecrementOffset(Point coords)
        {
            ChangeOffset(coords, false);
        }

        private void ChangeOffset(Point coords, bool increment)
        {
            TileData data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            //Offset can be set only to tiles, that are using offset (TileInfo.OffsetHorizontal = 1 or TileInfo.OffsetVertical = 1)
            //Tile type 7 is using offset, but its value cannot be changed. (Tile type 7 is + shaped and is not connected => bridge)          
            if (TilesInfo.IsType7(data.Type))
                return;
            TileInfoItem info = TilesInfo.GetItem(data.Type);
            //Do nothing if tile is not using both vertical and horizontal width.
            if (info.IsComposed() == false)
                return;

            TileData newData = data;
            if (increment)
                newData.Offset++;
            else
                newData.Offset--;
            newData.Repair();

            if (newData.Offset != data.Offset)
            {
                //Insert data.
                workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
                workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);
                workplace.SchemeEventHistory.FinalizeEvent();
            }
        }
    }
}
