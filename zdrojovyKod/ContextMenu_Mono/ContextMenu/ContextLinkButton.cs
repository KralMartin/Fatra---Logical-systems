using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ContextMenu_Mono.ContextMenu
{
    public class ContextLinkButton : ContextButton
    {
        Vector2 glyphOffset;

        public ContextLinkButton(ContextPanel panel, string text) : base(text)
        {
            this.Panel = panel;
            glyphOffset = new Vector2();
        }

        internal override void SetPosition(Point position)
        {
            base.SetPosition(position);
            glyphOffset.X = bounds.X + bounds.Width - Default.ContextMenu_ButtonGlyph;
            glyphOffset.Y = bounds.Y;
        }

        internal override void Draw(SpriteBatch sb, SpriteFont font, Texture2D texture)
        {
            base.Draw(sb, font, texture);
            sb.DrawString(font, "->", glyphOffset, Color.Black);
        }
    }
}
