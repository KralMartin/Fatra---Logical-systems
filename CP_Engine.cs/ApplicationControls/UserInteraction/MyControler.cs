using ContextMenu_Mono.ContextMenu;
using CP_Engine.BugItems;
using CP_Engine.MapItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Utilities_Mono;
using Utilties_Mono;

namespace CP_Engine
{
    class MyControler : Controler
    {
        WorkPlace workplace;
        Rectangle bounds;
        MouseButtonDown rightButtonState;
        MouseButtonDown leftButtonState;
        Point leftTileCoords;                       //Tile where user pressed left mouse button. 
        Point lastMousePosition;                    //Mouse position previous update.
        PopupMenuGenerator popupmenuGenerator;
        bool isCtrlDown;

        internal MyControler(WorkPlace workplace, Rectangle bounds, ContextMenuClass contextMenuLayer)
        {
            this.workplace = workplace;
            this.bounds = bounds;
            rightButtonState = new MouseButtonDown();
            leftButtonState = new MouseButtonDown();
            popupmenuGenerator = new PopupMenuGenerator(workplace, contextMenuLayer);
            DebugValue = "grid controler";
        }

        internal void Resize(Rectangle bounds)
        {
            this.bounds = bounds;
        }

        public override bool MouseWheel(int delta)
        {
            if (bounds.Contains(UserInput.MouseState.Position))
            {
                if (isCtrlDown)
                {
                    Point coords = workplace.CurrentWindow.GetTileAt(UserInput.MouseState.Position);
                    if (workplace.CurrentWindow.Scheme.ValidateCoords(coords))
                    {
                        if (delta > 0)
                            workplace.OffsetAssistant.IncrementOffset(coords);
                        else
                            workplace.OffsetAssistant.DecrementOffset(coords);
                    }
                }
                else
                {
                    if (delta > 0)
                        workplace.CurrentWindow.Zoom(2);
                    else
                        workplace.CurrentWindow.Zoom(-2);
                }
                return true;
            }
            return false;
        }

        public override void KeyRelease(Keys key)
        {
            if (key == Keys.LeftControl)
            {
                isCtrlDown = false;
            }
        }

        public override bool KeyPress(Keys key)
        {
            if (key == Keys.LeftControl)
            {
                isCtrlDown = true;
                return true;
            }
            return false;
        }

        public override bool MousePressLeft()
        {
            if (bounds.Contains(UserInput.MouseState.Position))
            {
                //Left mouse button was pressed over grid.
                leftButtonState.Press(UserInput.MouseState.Position);
                if (workplace.CurrentAction == Actions.Selecting)
                {
                    if (UserInput.PressedKeys.Contains(Keys.LeftControl)== false)
                        workplace.CurrentWindow.Selection.Items.Clear();
                    //workplace.CurrentWindow.Selection.StartRectangularSelection(UserInput.MouseState.Position, UserInput.MouseState.Position);
                    leftTileCoords = workplace.CurrentWindow.GetClosestTile(UserInput.MouseState.Position);
                }
                else if (workplace.CurrentAction == Actions.DrawingLines)
                {
                    workplace.DrawingAssistant.PrepareLine(leftButtonState.Position, leftButtonState.Position);
                }
                return true;
            }
            return false;
        }

        public override void DoubleClick(UserInput userInput)
        {
            //Get tile coords.
            Point coords = workplace.CurrentWindow.GetTileAt(userInput.MouseState.Position);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
                return;

            TileData data = workplace.CurrentWindow.Scheme.Get_TileData(coords);
            if (TilesInfo.IsBugType(data.Type))
            {
                //Open inner scheme of PlacedBug.
                PlacedBug pBug = workplace.CurrentWindow.Scheme.PlacedBugs.Get(data.HorzWidth);
                if(pBug.Bug.IsUserCreated())
                    workplace.OpenWindow(workplace.CurrentWindow.PhysScheme.Children[pBug.ID]);
            }
            else
            {
                //Select all tiles.
                if (userInput.PressedKeys.Contains(Keys.LeftControl) == false)
                    workplace.CurrentWindow.Selection.Items.Clear();
                SimplePathFinder.SelectPoints(workplace.CurrentWindow, coords);
            }
        }

        public override void MouseReleaseLeft(UserInput userInput)
        {
            if (workplace.CurrentAction == Actions.DrawingLines)
            {
                workplace.DrawingAssistant.InsertLine(leftButtonState.Position, userInput.MouseState.Position);
            }
            else if (workplace.CurrentAction == Actions.Inserting_Konveror)
            {
                workplace.InsertAssistant.InsertKonvertor(this.UserInput.MouseState.Position);
            }
            else if (workplace.CurrentAction == Actions.Inserting_Diode)
            {
                workplace.InsertAssistant.InsertDiode(this.UserInput.MouseState.Position);
            }
            else if (workplace.CurrentAction == Actions.Inserting_Input)
            {
                workplace.InsertAssistant.InsertInput(this.UserInput.MouseState.Position);
            }
            else if (workplace.CurrentAction == Actions.Inserting_Output)
            {
                workplace.InsertAssistant.InsertOutput(this.UserInput.MouseState.Position);
            }
            else if (workplace.CurrentAction == Actions.Removing)
            {
                workplace.RemovingAssistant.RemoveLine(UserInput.MouseState.Position);
            }
            else if (workplace.CurrentAction == Actions.Inserting_Bug)
            {
                workplace.BugInsertAssistant.MouseClick(workplace.CurrentWindow.GetTileAt(UserInput.MouseState.Position));
            }
            else if (workplace.CurrentAction == Actions.BreakPoint)
            {
                workplace.BreakPointAssistant.Click(UserInput.MouseState.Position);
            }

            leftButtonState.Release(userInput.GameTime.TotalGameTime.TotalMilliseconds);
        }

        public override bool MousePressRight()
        {
            if (bounds.Contains(UserInput.MouseState.Position))
            {
                rightButtonState.Press(UserInput.MouseState.Position);
                return true;
            }
            return false;
        }

        public override bool MouseMove()
        {
            if (bounds.Contains(UserInput.MouseState.Position))
            {
                if (rightButtonState.IsPressed)
                {
                    workplace.CurrentWindow.MoveSceenOffsetBy(lastMousePosition - UserInput.MouseState.Position);
                }
                Point currentLeftTileCoords = workplace.CurrentWindow.GetClosestTile(UserInput.MouseState.Position);
                Point exactCoords = workplace.CurrentWindow.GetTileAt(UserInput.MouseState.Position);

                if (workplace.CurrentAction == Actions.Inserting_Konveror || workplace.CurrentAction == Actions.Inserting_Diode
                    || workplace.CurrentAction == Actions.Inserting_Input || workplace.CurrentAction == Actions.Inserting_Output)
                {
                    workplace.CurrentWindow.Selection.Items.Clear();
                    workplace.CurrentWindow.Selection.SelectPointAt(UserInput.MouseState.Position);
                }
                else if (workplace.CurrentAction == Actions.BreakPoint)
                    workplace.BreakPointAssistant.MouseMove(UserInput.MouseState.Position);
                else if (workplace.CurrentAction == Actions.Inserting_Bug)
                {
                    workplace.BugInsertAssistant.MouseMove(workplace.CurrentWindow.GetTileAt(UserInput.MouseState.Position));
                }
                if (leftButtonState.IsPressed)
                {
                    if (workplace.CurrentAction == Actions.Selecting)
                    {
                        workplace.CurrentWindow.Selection.MouseMoveRectangularSelection(leftTileCoords, currentLeftTileCoords);
                    }
                    else if (workplace.CurrentAction == Actions.DrawingLines)
                    {
                        workplace.DrawingAssistant.PrepareLine(leftButtonState.Position, UserInput.MouseState.Position);
                    }
                }
                UpdateStatusText(UserInput.MouseState.Position);
                lastMousePosition = UserInput.MouseState.Position;
                return true;
            }
            return false;
        }

        private void UpdateStatusText(Point mousePosition)
        {
            Point coords = workplace.CurrentWindow.GetTileAt(mousePosition);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords) == false)
            {
                workplace.StatusText.SetTextRight("-");
                return;
            }
            string toolTipText = "";
            string text = "-";
            Tile tile = workplace.CurrentWindow.Scheme.Get_Tile(coords);

            if (TilesInfo.IsBugType(tile.Data.Type))
            {
                PlacedBug pBug= workplace.CurrentWindow.Scheme.PlacedBugs.Get(tile.Data.HorzWidth);
                toolTipText = pBug.GetDescription(coords);
                text = "nejaky input";
            }
            else
            {
                TileInfoItem info = TilesInfo.GetItem(tile.Data.Type);
                if (info.TileType == TileTypes.Input || info.TileType == TileTypes.Output)
                {
                    IODescription desc = workplace.CurrentWindow.Scheme.Bug.IODecription.Get_Description(coords);
                    toolTipText = desc.GetDescription();
                }
                if (TilesInfo.IsType7(tile.Data.Type) == false && info.TileType == TileTypes.Vire && tile.Paths.Length>0)
                    text = BinaryMath.FormatBits(tile.GetValues(workplace.CurrentWindow.PhysScheme), GlobalSettings.DefaultNumberFormat);
            }
            workplace.StatusText.SetTextRight(text);
            workplace.StatusText.SetToolTipText(toolTipText);
        }

        public override void MouseReleaseRight(UserInput userInput)
        {
            rightButtonState.Release(userInput.GameTime.TotalGameTime.TotalMilliseconds);
            if (rightButtonState.Position == userInput.MouseState.Position)
            {
                popupmenuGenerator.ShowContextMenu(userInput.MouseState.Position, workplace.CurrentWindow.Selection);
            }
        }
    }
}
