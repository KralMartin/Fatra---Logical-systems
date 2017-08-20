using CP_Engine.MapItems;
using Microsoft.Xna.Framework;

namespace CP_Engine.WorkplaceAssistants
{
    /// <summary>
    /// Helps to insert items to scheme.
    /// </summary>
    class InsertAssistant
    {
        WorkPlace workplace;

        internal InsertAssistant(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        internal void InsertKonvertor(Point mousePosition)
        {
            Insert(mousePosition, false);
        }

        internal void InsertDiode(Point mousePosition)
        {
            Insert(mousePosition, true);
        }

        internal void InsertInput(Point mousePosition)
        {
            //Type 21
            InsertInOutput(mousePosition, true);
        }

        internal void InsertOutput(Point mousePosition)
        {
            //Type 22
            InsertInOutput(mousePosition, false);
        }

        internal void InsertInOutput(Point mousePosition, bool input_output)
        {
            //Get tile coords.
            Point coords = workplace.CurrentWindow.GetTileAt(mousePosition);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;

            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            Repair repair = new Repair(workplace, workplace.CurrentWindow.Scheme);

            TileData oldData = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            TileData newData = new TileData();

            //Set width.
            if (oldData.HorzWidth > 0)
                newData.HorzWidth = oldData.HorzWidth;
            else
                newData.HorzWidth = 1;

            //Set type.
            if (input_output)
            {
                newData.Type = 21;
                Connect(coords, Sides.Right, newData.HorzWidth, repair);
            }
            else
            {
                newData.Type = 22;
                Connect(coords, Sides.Left, newData.HorzWidth, repair);
            }

            //Insert source to scheme, to event and to list of changed tiles.
            repair.Add(coords);
            workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);

            repair.RepairInner();
            repair.RepairOuter();

            workplace.SchemeEventHistory.FinalizeEvent();
        }

        /// <summary>
        /// Inserts item(Konvertor) to scheme,
        /// properly rotated based on mouse position.
        /// </summary>
        /// <param name="mousePosition"></param>
        private void Insert(Point mousePosition, bool diode_konvertor)
        {
            //Get tile coords.
            Point coords = workplace.CurrentWindow.GetTileAt(mousePosition);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;


            //Get position of mouse within tile
            int side = workplace.CurrentWindow.GetSideOfPointInTile(mousePosition);

            //Insert item to scheme.
            TileData oldData = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(oldData.Type))
                return;
            TileData newData = CreateData(side, oldData, diode_konvertor);

            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            Repair repair = new Repair(workplace, workplace.CurrentWindow.Scheme);

            //Insert source to scheme, to event and to list of changed tiles.
            repair.Add(coords);
            workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);

            //Connect source with adjected lines.
            if (Sides.IsHorizontal(side))
            {
                Connect(coords, Sides.Left, newData.HorzWidth, repair);
                Connect(coords, Sides.Right, newData.HorzWidth, repair);
            }
            else
            {
                Connect(coords, Sides.Top, newData.VertWidth, repair);
                Connect(coords, Sides.Bot, newData.VertWidth, repair);
            }
            repair.RepairInner();
            repair.RepairOuter();

            workplace.SchemeEventHistory.FinalizeEvent();
        }

        /// <summary>
        /// Connects source with adjected line, if there is any.
        /// </summary>
        /// <param name="coords">Coords where source is.</param>
        /// <param name="side">Direction where to move from provided coords.</param>
        /// <param name="sourceWidth"></param>
        /// <param name="eve">SchemeEvent where to store changes.</param>
        private void Connect(Point coords, int side, int sourceWidth, Repair repair)
        {
            //Move to side.
            coords = Sides.Move(coords, side);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;
            side = Sides.OpositeSide(side);
            //Get original data.
            TileData data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(data.Type))
                return;
            TileInfoItem info = TilesInfo.GetItem(data.Type);
            //Do nothing only if adjected side is allready used.
            if (info.TileSide[side].IsUsed)
                return;
            //Create new data.
            TileInfoItem repaired = info.CoppySides();
            repaired.TileSide[side] = new TileSide(true);
            repaired = TilesInfo.GetExactTemplate(repaired);
            if (repaired.Type != 0)
            {
                TileData newData = data;
                newData.Type = repaired.Type;
                if (newData.HorzWidth <= 0)
                    newData.HorzWidth = sourceWidth;
                if (newData.VertWidth <= 0)
                    newData.VertWidth = sourceWidth;

                repair.Add(coords);
                workplace.CurrentWindow.Scheme.Set_TileData(coords, newData);
            }
        }

        /// <summary>
        /// Returns tile data, based on provided side.
        /// </summary>
        /// <param name="side">Side where mouse is within tile.</param>
        /// <param name="initialData">Original data in scheme.</param>
        /// <returns></returns>
        private TileData CreateData(int side, TileData initialData, bool diode_konvertor)
        {
            TileData toReturn = initialData;
            if (toReturn.HorzWidth <= 0)
                toReturn.HorzWidth = 1;
            if (toReturn.VertWidth <= 0)
                toReturn.VertWidth = 1;

            toReturn.Type = 4;
            switch (side)
            {
                case Sides.Top:
                    toReturn.Type = 13;
                    break;
                case Sides.Right:
                    toReturn.Type = 14;
                    break;
                case Sides.Bot:
                    toReturn.Type = 15;
                    break;
                case Sides.Left:
                    toReturn.Type = 16;
                    break;
            }
            if (diode_konvertor)
                toReturn.Type += 4;

            //Set width
            TileInfoItem info = TilesInfo.GetItem(toReturn.Type);
            toReturn.Repair();
            return toReturn;
        }
    }
}
