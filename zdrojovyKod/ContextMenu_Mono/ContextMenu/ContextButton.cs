using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ContextMenu_Mono.ContextMenu
{
    public class ContextButton
    {
        public delegate void HoverEventHandler(ContextButton sender, bool isHovered);
        public event HoverEventHandler Hovered;

        /// <summary>
        /// Fires when left mouse button is released over this button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ClickEventHandler(ContextButton sender);
        public event ClickEventHandler Clicked;

        /// <summary>
        /// Text of button.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// ToolTip text.
        /// </summary>
        public string ToolTipText { get; set; }

        public object Tag { get; set; }

        public ContextPanel Panel { get; set; }

        internal bool HasMouseOverEffect { get; set; }

        protected Rectangle bounds;
        internal Rectangle Rectangle
        {
            get { return bounds; }
        }

        protected Vector2 textOffset;

        public ContextButton(string text)
        {
            this.Text = text;
            this.textOffset = new Vector2();
        }

        /// <summary>
        /// Fire Clicked event.
        /// </summary>
        internal virtual void MouseClick()
        {
            if (Clicked != null)
                Clicked(this);
        }

        internal virtual int GetButtonWidth(SpriteFont font)
        {
            Vector2 size = font.MeasureString(Text);
            return (int)size.X + Default.ContextMenu_ButtonTextMargin * 2;
        }
        
        internal void SetSize(Point point)
        {
            bounds.Width = point.X;
            bounds.Height = point.Y;
        }

        /// <summary>
        /// Update position of this button.
        /// </summary>
        /// <param name="rect"></param>
        internal virtual void SetPosition(Point position)
        {
            bounds.X = position.X;
            bounds.Y = position.Y;
            this.textOffset.X = this.bounds.X + Default.ContextMenu_ButtonTextMargin;
            this.textOffset.Y = this.bounds.Y + Default.ContextMenu_ButtonTextMargin;
        }

        internal bool Contains(Point point)
        {
            if (bounds.Contains(point))
                return true;
            return false;
        }

        internal virtual void Draw(SpriteBatch sb, SpriteFont font, Texture2D texture)
        {
            if (HasMouseOverEffect)
                sb.Draw(texture, bounds, Color.LightBlue);
            else
                sb.Draw(texture, bounds, Color.White);
            sb.DrawString(font, this.Text, textOffset, Color.Black);
        }

        internal void LostHover()
        {
            if (Hovered != null)
                Hovered(this, false);
        }

        internal void GainHover()
        {
            if (Hovered != null)
                Hovered(this, true);
        }
    }
}
