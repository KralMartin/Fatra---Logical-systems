using CP_Engine.MapItems;
using Microsoft.Xna.Framework;

namespace CP_Engine.WorkplaceAssistants
{
    /// <summary>
    /// Helps to set vire width.
    /// </summary>
    class WidthAssistant
    {
        WorkPlace workplace;

        internal WidthAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        /// <summary>
        /// Sets vire-width of selected tiles in currently visible window. 
        /// </summary>
        /// <param name="width">New width.</param>
        internal void SetWidth(int width)
        {
            Window window = workplace.CurrentWindow;
            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            Repair repair = new Repair(workplace, workplace.CurrentWindow.Scheme);
            TileInfoItem info;
            TileData data;
            TileData oldData;
            foreach (Point coords in workplace.CurrentWindow.Selection.Items)
            {
                data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type) == false)
                {
                    oldData = data;
                    info = TilesInfo.GetItem(data.Type);
                    if (info.IsComposed())
                    {
                        //Tile is composed.
                        //Change horizontal/vertical vire-width only if there is adjected(horizontaly/verticaly) tile in selection.
                        if (HasHorizontalFriend(coords))
                            data.HorzWidth = width;
                        if (HasVerticalFriend(coords))
                            data.VertWidth = width;
                    }
                    else
                    {
                        //Tile is not composed.
                        //Change vire-width, only if that width is used in tile.
                        if (info.UsesHorizontal())
                            data.HorzWidth = width;
                        if (info.UsesVertical())
                            data.VertWidth = width;
                    }
                    data.Repair();
                    repair.Add(coords);
                    window.Scheme.Set_TileData(coords, data);
                }
            }
            //Repair scheme.
            repair.RepairInner();
            repair.RepairOuter();

            workplace.SchemeEventHistory.FinalizeEvent();
        }

        /// <summary>
        /// Returns vire-width of current window selection.
        /// Returns -1 if function cant determine selection's vire-width.
        /// </summary>
        /// <returns></returns>
        internal int GetSelectionWidth()
        {
            Window window = workplace.CurrentWindow;
            int toReturn = -1;
            int currentWidth = -1;
            TileInfoItem info;
            TileData data;
            foreach (Point coords in window.Selection.Items)
            {
                //Get tile data.
                data = window.Scheme.Get_TileData(coords);
                if (TilesInfo.IsBugType(data.Type) == false && data.Type != 0)
                {
                    //Get tile info.
                    info = TilesInfo.GetItem(data.Type);
                    if (info.IsComposed())
                    {
                        //Tile is composed
                        int friends = 0;

                        //When tile has horizontal friend, i can count with its horizontal vire-width.
                        if (HasHorizontalFriend(coords))
                        {
                            friends++;
                            currentWidth = data.HorzWidth;
                        }
                        //Same for vertical.
                        if (HasVerticalFriend(coords))
                        {
                            friends++;
                            currentWidth = data.VertWidth;
                        }
                        //Tile has no horizontal nor vertical friend. 
                        //I cant decide which vire-width to use. 
                        if (friends == 0)
                            return -1;
                        //Tile has both horizontal and vertical friend.
                        //If its vires do not share same witdh, i cant decide which width to use.
                        if (friends == 2 && data.VertWidth != data.HorzWidth)
                            return -1;
                        //Else tile has exactly one friend and i know what vire-width to use.
                    }
                    else
                    {
                        //Tile is not composed.
                        //Get its vire-width.
                        if (info.UsesHorizontal())
                            currentWidth = data.HorzWidth;
                        else
                            currentWidth = data.VertWidth;
                    }
                    //If vire-width was set previously and if it do not match previous value, return -1
                    if (toReturn == -1)
                        toReturn = currentWidth;
                    else
                    {
                        if (toReturn != currentWidth)
                            return -1;
                    }
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Returns TRUE, when there is horizontaly adjected tile to provided tile in selection.
        /// </summary>
        /// <param name="coords">Coords of base tile.</param>
        /// <returns></returns>
        private bool HasHorizontalFriend(Point coords)
        {
            Point coord1 = new Point(coords.X + 1, coords.Y);
            if (workplace.CurrentWindow.Selection.Items.Contains(coord1))
                return true;
            coord1 = new Point(coords.X - 1, coords.Y);
            if (workplace.CurrentWindow.Selection.Items.Contains(coord1))
                return true;
            return false;
        }

        /// <summary>
        /// Returns TRUE, when there is verticaly adjected tile to provided tile in selection.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        private bool HasVerticalFriend(Point coords)
        {
            Point coord1 = new Point(coords.X, coords.Y + 1);
            if (workplace.CurrentWindow.Selection.Items.Contains(coord1))
                return true;
            coord1 = new Point(coords.X, coords.Y - 1);
            if (workplace.CurrentWindow.Selection.Items.Contains(coord1))
                return true;
            return false;
        }
    }
}
