using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Utilities_Mono;
using Utilties_Mono;

namespace ContextMenu_Mono.Menu
{
    public class MenuPanel : Controler
    {
        public delegate void ClickEventHandler(MenuPanel sender);
        public event ClickEventHandler Clicked;

        public delegate void DoubleClickEventHandler(MenuPanel sender);
        public event DoubleClickEventHandler DoubleClicked;

        public delegate void HoverEventHandler(MenuPanel sender, bool isHovered);
        public event HoverEventHandler Hovered;

        public delegate void MoveEventHandler(MenuPanel sender);
        public event MoveEventHandler Moved;

        /// <summary>
        /// Text of MenuPanel. 
        /// Use TextHalign and TextValign to place it in MenuPanel.
        /// </summary>
        public string Text
        {
            get { return DrawnText.Text; }
            set { DrawnText.Text = value; }
        }

        /// <summary>
        /// ToolTip text.
        /// </summary>
        public string ToolTipText { get; set; }

        /// <summary>
        /// Store any object to this property.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Behaviour and appearence settings of this instance.
        /// </summary>
        public MenuPanelSettings Settings { get { return settings; } set { settings = value; } }

        /// <summary>
        /// Child MenuPanels. Note that Children should not be overlaying outside bounds of parent.
        /// </summary>
        public MenuPanelCollection Children { get; private set; }

        /// <summary>
        /// When Mover is pressed and parent of this object has allowed children movement, user can move this object by moving mouse.
        /// </summary>
        public MenuPanel Mover { get; set; }

        internal MenuPanel Parent { get; set; }

        internal DrawnText DrawnText { get; private set; }

        MenuPanelSettings settings;
        private Point oldMousePosition;                         //Used when user is moving children.
        protected Vector2 relativeTextOffset;                   //offset to top left corner of this MenuPanel.
        protected Rectangle relativeBounds;                     //X,Y represents offset to top left corner of parent.
        protected Color hoverColor;
        protected Color pressColor;

        protected bool isPressed;           //Determintes whether this panel is pressed (If any of children is pressed, thsi is not pressed).
        protected bool isHovered;           //Determintes whether this panel is hovered (If any of children is hovered, thsi is not hovered).

        protected MenuPanel pressedSon;     //Pointer to panel, that is pressed or one of it's children is pressed.
        protected MenuPanel hoveredSon;     //Pointer to panel, that is hovered or one of it's children is hovered.
        protected RectBorder border;        //Inner border (It means, border is within panel's bounds).

        public MenuPanel(MenuPanelSettings settings)
        {
            this.Settings = settings;
            this.Children = new MenuPanelCollection(this);
            this.hoverColor = new Color(Color.Blue, 0.3f);
            this.pressColor = new Color(Color.Red, 0.3f);
            this.DrawnText = new DrawnText(Settings.Font, "", Settings.TextHalign, Settings.TextValign);
        }

        public void ChangeOffset(Point offset)
        {
            relativeBounds.X = offset.X;
            relativeBounds.Y = offset.Y;
        }

        public void SimulateClick()
        {
            //Call ExecuteAction function (descendants of this class can do some calculations upon clicking on this panel.)
            //And call Clicked event.
            ExecuteAction();
            if (Clicked != null)
                Clicked(this);
        }

        /// <summary>
        /// Return bounds of MenuPanel.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBounds()
        {
            Point position = GetPosition();
            return new Rectangle(position.X, position.Y, relativeBounds.Width, relativeBounds.Height);
        }

        internal void SetMargin(Point margin)
        {
            settings.Margin = margin;
        }

        /// <summary>
        /// Returns position of top left corner of MenuPanel.
        /// </summary>
        /// <returns></returns>
        public Point GetPosition()
        {
            if (Parent != null)
                return Parent.GetPosition() + new Point(relativeBounds.X, relativeBounds.Y);
            return new Point(relativeBounds.X, relativeBounds.Y);
        }

        /// <summary>
        /// Returns size of MenuPanel.
        /// </summary>
        /// <returns></returns>
        public Point GetSize()
        {
            return new Point(relativeBounds.Width, relativeBounds.Height);
        }

        /// <summary>
        /// Call this function only when:
        /// A: MenuPanel has fully initialized parent.
        /// B: MenuPanel has no parent, but it its Halign = Left, Valign = Top.
        /// </summary>
        public void Changed()
        {
            if (Parent != null)
                Changed(Parent.relativeBounds);
            else
                Changed(new Rectangle());
        }

        /// <summary>
        /// Call this function after any changes to variabiles in Settings property.
        /// </summary>
        /// <param name="parentBounds">Bounds of parent.</param>
        public void Changed(Rectangle parentBounds)
        {
            Changed_Protected();
            Point parentSize = new Point(parentBounds.Width, parentBounds.Height);
            Point myOffset = CalculateOffset(this.Settings.Halign, this.Settings.Valign, this.Settings.Margin, this.Settings.Size, parentSize);
            relativeBounds.X = myOffset.X ;
            relativeBounds.Y = myOffset.Y;
        }

        /// <summary>
        /// Calculates relative offset of son in parent MenuPanel.
        /// </summary>
        /// <param name="son">Son MenuPanel</param>
        /// <returns></returns>
        internal Point CalculateOffset(MenuPanel son)
        {
            Point sonSize = new Point(son.relativeBounds.Width, son.relativeBounds.Height);
            return CalculateOffset(son.Settings.Halign, son.Settings.Valign, son.Settings.Margin, sonSize, this.Settings.Size);
        }

        /// <summary>
        /// Calculates offset of son relative to top left corner of parent.
        /// </summary>
        /// <param name="halign">Son's horizontal aligment.</param>
        /// <param name="valigh">Son's vertical aligment.</param>
        /// <param name="margin">Son's margin.</param>
        /// <param name="size">Son's size.</param>
        /// <param name="parentSize">Parent's size.</param>
        /// <returns></returns>
        internal Point CalculateOffset(HorizontalAligment halign, VerticalAligment valigh, Point margin, Point size, Point parentSize)
        {
            Point toReturn = new Point();
            switch (halign)
            {
                case HorizontalAligment.Left:
                    toReturn.X = margin.X;
                    break;
                case HorizontalAligment.Right:
                    toReturn.X = parentSize.X - margin.X - size.X;
                    break;
                case HorizontalAligment.Center:
                    toReturn.X = (parentSize.X - size.X) / 2;
                    break;
            }
            switch (valigh)
            {
                case VerticalAligment.Top:
                    toReturn.Y = margin.Y;
                    break;
                case VerticalAligment.Bottom:
                    toReturn.Y = parentSize.Y - margin.Y - size.Y;
                    break;
                case VerticalAligment.Center:
                    toReturn.Y = (parentSize.Y - size.Y) / 2;
                    break;
            }
            return toReturn;
        }

        protected virtual void Changed_Protected()
        {
            //Firstly set realbounds size to desired size.
            this.relativeBounds.Width = Settings.Size.X;
            this.relativeBounds.Height = Settings.Size.Y;

            if (Children.Count > 0)
            {
                switch (this.Settings.ChildrenLayout)
                {
                    case ChildrenLayouts.HorizontalStack:
                        int adjustment = 0;
                        int index = 0;
                        foreach (MenuPanel current in this.Children)
                        {
                            //Get son's relative offset.
                            current.Changed_Protected();
                            Point sonOffset = CalculateOffset(current);

                            //Calculate required height of parent.
                            if (Settings.AdjustHeightToContent && relativeBounds.Y < sonOffset.Y * 2 + current.relativeBounds.Height)
                                relativeBounds.Y = sonOffset.Y * 2 + current.relativeBounds.Height;

                            //Adjust son's offset.X by total witdh of previous sons.
                            sonOffset.X += adjustment;
                            //Set son's real bounds.               
                            current.relativeBounds.X = sonOffset.X;
                            current.relativeBounds.Y = sonOffset.Y;
                            //Next son's offset.X will be adjusted by this value.
                            adjustment = current.relativeBounds.X + current.relativeBounds.Width;

                            index++;
                        }
                        if (Settings.AdjustWidthToContent)
                        {
                            if (relativeBounds.X < adjustment + Children[0].Settings.Margin.X)
                                relativeBounds.X = adjustment + Children[0].Settings.Margin.X;
                        }
                        break;
                    case ChildrenLayouts.Normal:
                        foreach (MenuPanel current in this.Children)
                        {
                            //Get son's relative offset.
                            current.Changed_Protected();
                            Point sonOffset = CalculateOffset(current);
                            //Set son's real bounds.               
                            current.relativeBounds.X = sonOffset.X;
                            current.relativeBounds.Y = sonOffset.Y;

                            //Calculate size change.
                            if (Settings.AdjustWidthToContent && relativeBounds.X < current.relativeBounds.X + current.relativeBounds.Width)
                                relativeBounds.Width = current.relativeBounds.X + current.relativeBounds.Width;
                            if (Settings.AdjustHeightToContent && relativeBounds.Y < current.relativeBounds.Y + current.relativeBounds.Height)
                                relativeBounds.Height = current.relativeBounds.Y + current.relativeBounds.Height;
                        }
                        break;
                    case ChildrenLayouts.VerticalStack:
                        adjustment = 0;
                        foreach (MenuPanel current in this.Children)
                        {
                            //Get son's relative offset.
                            current.Changed_Protected();
                            Point sonOffset = CalculateOffset(current);

                            //Calculate required width of parent.
                            if (Settings.AdjustWidthToContent && relativeBounds.Width < sonOffset.X * 2 + current.relativeBounds.Width)
                                relativeBounds.Width = sonOffset.X * 2 + current.relativeBounds.Width;

                            //Adjust son's offset.X by total witdh of previous sons.
                            sonOffset.Y += adjustment;
                            //Set son's real bounds.               
                            current.relativeBounds.X = sonOffset.X;
                            current.relativeBounds.Y = sonOffset.Y;
                            //Next son's offset.X will be adjusted by this value.
                            adjustment = current.relativeBounds.Y + current.relativeBounds.Height;
                        }
                        if (Settings.AdjustHeightToContent)
                        {
                            if (relativeBounds.Height < adjustment + Children[0].Settings.Margin.Y)
                                relativeBounds.Height = adjustment + Children[0].Settings.Margin.Y;
                        }
                        break;
                }
            }
            //Calculate text relative offset.
            TextChanged();

            //Set Border.
            if (Settings.BorderWidth > 0)
                border = new RectBorder(Settings.BorderWidth, new Rectangle(0, 0, relativeBounds.Width, relativeBounds.Height), Settings.BorderColor);
            else
                border = null;
        }

        /// <summary>
        /// This function is called when user clicked on button.
        /// Function is called before firing Click event.
        /// </summary>
        internal virtual void ExecuteAction() { }

        /// <summary>
        /// Calculates text offset and/or changes size of this MenuPanel based of text size.
        /// </summary>
        public void TextChanged()
        {
            if (Text != null && this.Settings.Font!= null)
            {
                DrawnText.Font = Settings.Font;
                DrawnText.Text = this.Text;
                DrawnText.TextMargin = Settings.TextMargin;
                DrawnText.TextHalign = Settings.TextHalign;
                DrawnText.TextValign = Settings.TextValign;
                
                Point minSize = new Point();
                Point maxSize = new Point(int.MaxValue, int.MaxValue);
                if (Settings.AdjustWidthToContent == false)
                {
                    minSize.X = Settings.Size.X;
                    maxSize.X = Settings.Size.X;
                }
                if (Settings.AdjustHeightToContent == false)
                {
                    minSize.Y = Settings.Size.Y;
                    maxSize.Y = Settings.Size.Y;
                }
                DrawnText.MinSize = minSize;
                DrawnText.MaxSize = maxSize;
                DrawnText.Changed();

                Point requiredSize = DrawnText.GetSize();
                if (Settings.AdjustWidthToContent)
                    maxSize.X = requiredSize.X;
                if (Settings.AdjustHeightToContent)
                    maxSize.Y = requiredSize.Y;

                if (maxSize.X > relativeBounds.Width)
                    relativeBounds.Width = maxSize.X;
                if (maxSize.Y > relativeBounds.Height)
                    relativeBounds.Height = maxSize.Y;


                Point textSize = Settings.Font.MeasureString(this.Text).ToPoint();
                if (textSize.Y <= 0)
                    textSize.Y = Settings.Font.LineSpacing;
                if (Settings.AdjustWidthToContent)
                {
                    if (Settings.TextMargin.X * 2 + textSize.X > relativeBounds.Width)
                        relativeBounds.Width = Settings.TextMargin.X * 2 + textSize.X;
                }
                if (Settings.AdjustHeightToContent)
                {
                    if (Settings.TextMargin.Y * 2 + textSize.Y > relativeBounds.Height)
                        relativeBounds.Height = Settings.TextMargin.Y * 2 + textSize.Y;
                }
                relativeTextOffset = CalculateOffset(Settings.TextHalign, Settings.TextValign, Settings.TextMargin, textSize, new Point(relativeBounds.Width, relativeBounds.Height)).ToVector2();
            }
        }
        
        internal virtual void Draw(SpriteBatch sb, Point adjustment)
        {
            Rectangle rect = relativeBounds;
            adjustment.X += rect.X;
            adjustment.Y += rect.Y;
            rect.X = adjustment.X;
            rect.Y = adjustment.Y;
            if (this.Settings.BackGroundTexture != null)
            {
                if (Settings.IgnoreEffects)
                    sb.Draw(this.Settings.BackGroundTexture, rect, Color.White);
                else if (isPressed)
                    sb.Draw(this.Settings.BackGroundTexture, rect, pressColor);
                else if (isHovered)
                    sb.Draw(this.Settings.BackGroundTexture, rect, hoverColor);
                else
                    sb.Draw(this.Settings.BackGroundTexture, rect, Color.White);
            }
            DrawnText.DrawTopLeftCorner(sb, adjustment);

            foreach (MenuPanel current in Children)
            {
                current.Draw(sb, adjustment);
            }
            if (border != null)
                border.DrawDynamic(sb, adjustment);
        }

        #region ControlerOverrides

        public sealed override void LostMouseMove()
        {
            MenuLostMouseMove();
        }

        public sealed override void DoubleClick(UserInput userInput)
        {
            DoubleClick(UserInput.MouseState.Position);
        }

        public sealed override bool MousePressLeft()
        {
            return MenuMousePressLeft(UserInput.MouseState.Position);
        }

        public sealed override void MouseReleaseLeft(UserInput userInput)
        {
            MenuMouseReleaseLeft(userInput.MouseState.Position);
        }

        public sealed override bool MouseMove()
        {
            return MenuMouseMove(UserInput.MouseState.Position);
        }

        public override bool MousePressRight()
        {
            return relativeBounds.Contains(UserInput.MouseState.Position);
        }

        public override bool MouseWheel(int delta)
        {
            return relativeBounds.Contains(UserInput.MouseState.Position);
        }
        #endregion

        #region Handling user-input

        private void MenuLostMouseMove()
        {
            isHovered = false;
            if (hoveredSon != null)
            {
                hoveredSon.MenuLostMouseMove();
            }
        }

        private bool DoubleClick(Point position)
        {
            if (relativeBounds.Contains(position))
            {
                //Change position, so it will be relative to top left corner of this MenuPanel.
                position.X -= relativeBounds.X;
                position.Y -= relativeBounds.Y;
                foreach (MenuPanel current in this.Children)
                {
                    if (current.DoubleClick(position))
                    {
                        return true;
                    }
                }
                if (Settings.IgnoreEffects == false)
                {
                    if (DoubleClicked != null)
                        DoubleClicked(this);
                    return true;
                }
            }
            return false;
        }

        private bool MenuMousePressLeft(Point position)
        {
            //Catch input only when mouse is within this bounds.
            if (relativeBounds.Contains(position))
            {
                //Set previous position of mouse, when moving children is allowed.
                if (Settings.AllowChildrenMovement)
                    oldMousePosition = position;

                //Change position, so it will be relative to top left corner of this MenuPanel.
                position.X -= relativeBounds.X;
                position.Y -= relativeBounds.Y;
                foreach (MenuPanel current in this.Children)
                {
                    if (current.MenuMousePressLeft(position))
                    {
                        //If child panel is pressed, this cannot be pressed.
                        //But I have to remmember which child is pressed.
                        isPressed = false;
                        pressedSon = current;
                        return true;
                    }
                }
                //If no child is pressed, I am pressed.
                isPressed = true;
                return true;
            }
            return false;
        }

        private void MenuMouseReleaseLeft(Point position)
        {
            if (pressedSon != null)
            {
                //When one of my children is pressed.
                //AdjustedPoint represents mouse position relative to top left corner of this panel.
                Point adjustedPoint = position;
                adjustedPoint.X -= relativeBounds.X;
                adjustedPoint.Y -= relativeBounds.Y;
                //Call this function over pressed son.
                pressedSon.MenuMouseReleaseLeft(adjustedPoint);
                //After mouse release, pointer to pressed son is set to null.
                pressedSon = null;
            }
            else if (isPressed)
            {
                //When mouse is released and this is pressed, set isPressed = FALSE.
                isPressed = false;
                if (this.relativeBounds.Contains(position))
                {
                    SimulateClick();
                }
            }
        }

        private bool MenuMouseMove(Point position)
        {
            //AdjustedPoint represents mouse position relative to top left corner of this panel.
            Point adjustedPosition = position;
            adjustedPosition.X -= relativeBounds.X;
            adjustedPosition.Y -= relativeBounds.Y;
            
            if (hoveredSon != null)
            {
                //Move with child if it is allowed.
                //If child was moved, do nothing else in this method.
                if (Settings.AllowChildrenMovement && hoveredSon.Mover != null && hoveredSon.Mover.isPressed)
                {
                    hoveredSon.relativeBounds.X += position.X - oldMousePosition.X;
                    hoveredSon.relativeBounds.Y += position.Y - oldMousePosition.Y;
                    oldMousePosition = position;

                    if (hoveredSon.relativeBounds.X < 0)
                        hoveredSon.relativeBounds.X = 0;
                    else if (hoveredSon.relativeBounds.X + hoveredSon.relativeBounds.Width > relativeBounds.Width)
                        hoveredSon.relativeBounds.X = relativeBounds.Width - hoveredSon.relativeBounds.Width;

                    if (hoveredSon.relativeBounds.Y < 0)
                        hoveredSon.relativeBounds.Y = 0;
                    else if (hoveredSon.relativeBounds.Y + hoveredSon.relativeBounds.Height > relativeBounds.Height)
                        hoveredSon.relativeBounds.Y = relativeBounds.Height - hoveredSon.relativeBounds.Height;
                    hoveredSon.settings.Margin = new Point(hoveredSon.relativeBounds.X, hoveredSon.relativeBounds.Y);
                    if (hoveredSon.Moved != null)
                        hoveredSon.Moved(this);
                    return true;
                }
                //Call this function over hovered son.
                if (hoveredSon.MenuMouseMove(adjustedPosition))
                    return true;
                hoveredSon = null;
            }
            if (relativeBounds.Contains(position))
            {
                //Find new hovered son.
                foreach (MenuPanel current in this.Children)
                {
                    if (current.MenuMouseMove(adjustedPosition))
                    {
                        isHovered = false;
                        hoveredSon = current;
                        return true;
                    }
                }
                isHovered = true;
                if (Hovered != null)
                    Hovered(this, isHovered);
                return true;
            }
            isHovered = false;
            if (Hovered != null)
                Hovered(this, isHovered);
            return false;
        }

        #endregion

        public override void ControlerDraw(SpriteBatch sb)
        {
            this.Draw(sb, new Point());
        }
    }
}
