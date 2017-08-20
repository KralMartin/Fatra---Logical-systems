using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ContextMenu_Mono.ContextMenu
{
    /// <summary>
    /// Context Menu Panel, that holds buttons.
    /// </summary>
    public class ContextPanel
    {
        /// <summary>
        /// Child buttons.
        /// </summary>
        public List<ContextButton> Buttons { get; private set; }

        public int MinimalWidth { get; set; }

        //Helps to open and close next menu with a delay.
        TimeAndPointer open;                //Determines when will be next menu opened.
        TimeAndPointer close;               //Determines when will be next menu closed.

        ContextPanel nextPanel;             //Next panel, which is submenu.
        ContextButton currentlyHovered;     //currently hovered button
        bool isHovered;                     //Determines whether mouse is over this panel.
                                            //(If mouse is over this panel and over child panel at same time, this isHovered is set to false.)
        SpriteFont font;
        Texture2D Texture;
        Rectangle bounds;
        Rectangle allowedArea;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textrure">Panel background texture.</param>
        /// <param name="font">Button font.</param>
        public ContextPanel(Texture2D textrure, SpriteFont font)
        {
            this.font = font;
            this.Texture = textrure;
            isHovered = false;
            open = new TimeAndPointer();
            close = new TimeAndPointer();
            Buttons = new List<ContextButton>();
        }

        /// <summary>
        /// Call after changes in Buttons list.
        /// </summary>
        public void Changed()
        {
            int buttonHeight = font.LineSpacing + Default.ContextMenu_ButtonTextMargin * 2;
            int buttonWidth = this.MinimalWidth;
            foreach (ContextButton btn in Buttons)
                buttonWidth = Math.Max(btn.GetButtonWidth(font), buttonWidth);

            bounds.Width = buttonWidth + Default.ContextMenu_ButtonMargin * 2;
            bounds.Height = buttonHeight * Buttons.Count + Default.ContextMenu_ButtonMargin * 2;
            Point buttonSize = new Point(buttonWidth, buttonHeight);
            foreach (ContextButton btn in Buttons)
                btn.SetSize(buttonSize);
        }

        internal bool MousePress(Point point)
        {
            if (nextPanel != null)
            {
                if (nextPanel.MousePress(point))
                    return true;
            }
            return bounds.Contains(point);
        }

        internal bool WasReleasedOverLinkButton()
        {
            if (currentlyHovered != null)
            {
                if (currentlyHovered.Panel != null)
                    return true;
                return false;
            }
            if (nextPanel != null)
                return nextPanel.WasReleasedOverLinkButton();
            return false;
        }

        /// <summary>
        /// 0   missed, close
        /// 1   inside, close
        /// 2   inside, stay open
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal int MouseRelease(Point position)
        {
            int toReturn;
            if (nextPanel != null)
            {
                toReturn = nextPanel.MouseRelease(position);
                if (toReturn > 0)
                    return toReturn;
            }
            if (bounds.Contains(position))
            {
                if (currentlyHovered != null && currentlyHovered.Contains(position))
                {
                    currentlyHovered.MouseClick();
                    if (currentlyHovered.Panel != null)
                    {
                        this.open.IsOn = false;
                        this.nextPanel = currentlyHovered.Panel;
                        this.nextPanel.Show(new Point(this.bounds.X + bounds.Width, currentlyHovered.Rectangle.Y), allowedArea, bounds.Width);
                        return 2;
                    }
                    return 1;
                }
                return 2;
            }
            return 0;
        }

        internal void Update(GameTime gameTime)
        {
            if (nextPanel != null)
                nextPanel.Update(gameTime);
            if (isHovered == false)
                return;
            if (close.Activate(gameTime.TotalGameTime.TotalMilliseconds))
            {
                this.nextPanel = null;
            }
            if (open.Activate(gameTime.TotalGameTime.TotalMilliseconds))
            {
                this.nextPanel = open.Button.Panel;
                if (currentlyHovered != null)
                    this.nextPanel.Show(new Point(this.bounds.X + bounds.Width, currentlyHovered.Rectangle.Y), allowedArea, bounds.Width);
            }
        }

        /// <summary>
        /// Returns TRUE if cursor if over this panel or its child panel.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="mouse"></param>
        /// <returns>TRUE: cursor is over this panel or its child panel.</returns>
        internal bool MouseMove(GameTime gameTime, MouseState mouse)
        {
            if (nextPanel != null)
            {
                //Update child panel with higher priority.
                if (nextPanel.MouseMove(gameTime, mouse))
                {
                    //If mouse was catched by child panel, this panel wont be updated.
                    isHovered = false;
                    return true;
                }
            }
            if (bounds.Contains(mouse.Position) == false)
            {
                if (isHovered)
                {
                    //Mouse left bounds of this panel.
                    currentlyHovered = null;
                    isHovered = false;
                    //Set mouse-over-effect to child buttons.
                    foreach (ContextButton btn in Buttons)
                    {
                        if (btn.Panel != null && ReferenceEquals(btn.Panel, nextPanel))
                        {
                            btn.HasMouseOverEffect = true;
                            currentlyHovered = btn;
                        }
                        else
                            btn.HasMouseOverEffect = false;
                    }
                }
                //Mouse was not catched by this panel and ancestor can process it.
                return false;
            }
            //Mouse is over this panel.
            isHovered = true;
            if (currentlyHovered != null)
            {
                //Check if mouse is over button that was hovered last update.
                if (currentlyHovered.Contains(mouse.Position))
                {
                    //Mouse remains over same button.
                    //Mouse was not catched by this panel and ancestor can NOT process it.
                    return true;
                }
                else
                {
                    //Mouse is no longer over hovered buttton.
                    //Disable opening of child panel.
                    open.IsOn = false;
                    if (nextPanel != null)
                    {
                        //Start closing of child panel.
                        close.Button = currentlyHovered;
                        close.IsOn = true;
                        close.Time = gameTime.TotalGameTime.TotalMilliseconds + Default.ContextMenuDelay;
                    }
                    //Fire hovered event.
                    currentlyHovered.LostHover();

                    currentlyHovered.HasMouseOverEffect = false;
                    currentlyHovered = null;
                }
            }
            //Find button, that contains mouse position.
            foreach (ContextButton btn in Buttons)
            {
                if (btn.Contains(mouse.Position))
                {
                    if (btn.Panel != null)
                    {
                        //Start opening child panel.
                        open.Button = btn;
                        open.IsOn = true;
                        open.Time = gameTime.TotalGameTime.TotalMilliseconds + Default.ContextMenuDelay;
                    }
                    //Fire hovered event.
                    btn.GainHover();

                    btn.HasMouseOverEffect = true;
                    currentlyHovered = btn;
                    break;
                }
            }
            //Mouse was not catched by this panel and ancestor can NOT process it.
            return true;
        }

        /// <summary>
        /// Show this panel at provided position.
        /// </summary>
        /// <param name="position">Top left corner of this panel.</param>
        /// <param name="allowedArea">Allowed area of context menu.</param>
        /// <param name="panelWidth">Width of parent panel.</param>
        internal void Show(Point position, Rectangle allowedArea, int panelWidth)
        {
            //Reset all variabiles.
            this.allowedArea = allowedArea;
            this.nextPanel = null;
            this.currentlyHovered = null;
            this.isHovered = false;
            this.close.IsOn = false;
            this.open.IsOn = false;

            //Adjust position of this panel, if it wont fit to allowed area.
            bounds.X = position.X;
            bounds.Y = position.Y;
            int textMargin = CalculateButtonTextMargin();

            if (allowedArea.X + allowedArea.Width < bounds.X + bounds.Width)
            {
                if (panelWidth <= 0)
                    bounds.X = allowedArea.X + allowedArea.Width - bounds.Width;
                else
                    bounds.X -= bounds.Width + panelWidth;
            }
            if (allowedArea.Y + allowedArea.Height < bounds.Y + bounds.Height)
            {
                bounds.Y = allowedArea.Y + allowedArea.Height - bounds.Height;
            }

            //Reset variabiles of child buttons and set theirs position.
            int deltaY = bounds.Y + Default.ContextMenu_ButtonMargin;
            foreach (ContextButton btn in Buttons)
            {
                btn.HasMouseOverEffect = false;
                btn.SetPosition(new Point(bounds.X + Default.ContextMenu_ButtonMargin, deltaY));
                deltaY += btn.Rectangle.Height;
            }
        }
        
        internal void Draw(SpriteBatch sb)
        {
            if (nextPanel != null)
                nextPanel.Draw(sb);
            sb.Draw(this.Texture, bounds, Color.White);
            foreach (ContextButton btn in Buttons)
                btn.Draw(sb, font, this.Texture);
        }

        private int CalculateButtonTextMargin()
        {
            return font.LineSpacing / 4;
        }
    }
}
