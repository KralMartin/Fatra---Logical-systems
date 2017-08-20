using Microsoft.Xna.Framework;
using CP_Engine.MapItems;

namespace CP_Engine.WorkplaceAssistants
{
    class RemovingAssistant
    {
        WorkPlace workplace;

        internal RemovingAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        /// <summary>
        /// Removes side of tile.
        /// </summary>
        /// <param name="mousePosition"></param>
        internal void RemoveLine(Point mousePosition)
        {
            Point coords = workplace.CurrentWindow.GetTileAt(mousePosition);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;

            TileData data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(data.Type))
            {
                //If user clicked on Bug, remove it.
                DeleteBug(coords);
                return;
            }
            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            TileInfoItem info = TilesInfo.GetItem(data.Type);

            //Get side within a Tile. Do nothing is side is not used allready.
            int side = workplace.CurrentWindow.GetSideOfPointInTile(mousePosition);


            //Create new Data.
            TileInfoItem newInfo;
            if (info.IsComposed() && info.TileSide[side].IsUsed)
            {
                newInfo = info.CoppySides();
                newInfo.TileSide[side] = new TileSide(false);
                newInfo = TilesInfo.GetExactTemplate(newInfo);

                //Cant make L shaped tile from T shaped tile, when T shaped tile uses two diferent vire-widths of horizontal and vertical vire.
                if (newInfo.IsComposed() == false && newInfo.UsesHorizontal() && newInfo.UsesVertical() && info.IsComposed() == true && data.HorzWidth != data.VertWidth)
                    newInfo = new TileInfoItem(0);
            }
            else
                newInfo = new TileInfoItem(0);

            TileData newData = data;
            newData.Type = newInfo.Type;
            newData.Repair();

            Repair repair = new Repair(workplace, workplace.CurrentWindow.Scheme);
            workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);
            repair.RepairInner();
            repair.RepairOuter();
            workplace.SchemeEventHistory.FinalizeEvent();
        }

        private void DeleteBug(Point coords)
        {
            //Create SchemeEvent.
            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            Repair rr = new Repair(workplace, workplace.CurrentWindow.Scheme);
            workplace.BugAssistant.Delete(coords, workplace.CurrentWindow.Scheme);
            rr.RepairOuter();
            workplace.SchemeEventHistory.FinalizeEvent();
        }
    }
}
