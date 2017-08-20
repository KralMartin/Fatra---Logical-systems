using System;
using CP_Engine.MapItems;
using Microsoft.Xna.Framework;

namespace CP_Engine.WorkplaceAssistants
{
    class BreakPointAssistant
    {
        WorkPlace workplace;

        internal BreakPointAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        internal void Click(Point mousePosition)
        {
            MouseMove(mousePosition);
            if (workplace.CurrentWindow.Selection.IsValid == false)
                return;
            Point coords = workplace.CurrentWindow.GetTileAt(mousePosition);
            Tile tile = workplace.CurrentWindow.Scheme.Get_Tile(coords);
            TileInfoItem info = TilesInfo.GetItem(tile.Data.Type);

            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            if (tile.BreakPoint == null)
                workplace.CurrentWindow.Scheme.BreakPoints.Add(coords);
            else
            {
                workplace.CurrentWindow.Scheme.BreakPoints.Remove(coords);
                tile.BreakPoint = null;
            }
            workplace.SchemeEventHistory.FinalizeEvent();
        }

        internal void MouseMove(Point mousePosition)
        {
            //Get tile coords.
            Point coords = workplace.CurrentWindow.GetTileAt(mousePosition);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;
            TileData data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            workplace.CurrentWindow.Selection.Items.Clear();
            workplace.CurrentWindow.Selection.Items.Add(coords);

            if (TilesInfo.IsBugType(data.Type) || TilesInfo.IsType7(data.Type) || TilesInfo.IsType12(data.Type))
            {
                workplace.CurrentWindow.Selection.IsValid = false;
                return;
            } 
            TileInfoItem info = TilesInfo.GetItem(data.Type);    
            workplace.CurrentWindow.Selection.IsValid = (info.TileType == TileTypes.Vire);            
        }
    }
}
