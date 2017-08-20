using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities_Mono;

namespace Utilities_Mono
{
    public class RectBorder
    {
        Tect top;
        Tect bot;
        Tect left;
        Tect right;
        int borderWidth;

        public RectBorder(int borderWidth, Rectangle rectangle, Texture2D texture)
        {
            this.borderWidth = borderWidth;
            Rectangle rect = rectangle;
            rect.Height = borderWidth;
            top = new Tect(texture, rect);

            rect = rectangle;
            rect.Y = rect.Height + rect.Y - borderWidth;
            rect.Height = borderWidth;
            bot = new Tect(texture, rect);

            rect = rectangle;
            rect.Y += borderWidth;
            rect.Height -= borderWidth * 2;
            rect.Width = borderWidth;
            left = new Tect(texture, rect);

            rect = rectangle;
            rect.Y += borderWidth;
            rect.Height -= borderWidth * 2;
            rect.X += rectangle.Width - borderWidth;
            rect.Width = borderWidth;
            right = new Tect(texture, rect);
        }

        /// <summary>
        /// Returns rectangle, excluding border width.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetInnerRectange()
        {
            return new Rectangle(top.Rectangle.X + left.Rectangle.Width, top.Rectangle.Y + top.Rectangle.Height, top.Rectangle.Width - 2 * left.Rectangle.Width, left.Rectangle.Height);
        }

        public Rectangle GetOuterRectangle()
        {
            return new Rectangle(top.Rectangle.X, top.Rectangle.Y, top.Rectangle.Width, left.Rectangle.Height + top.Rectangle.Height * 2);
        }

        public int Get_BorderWidth()
        {
            return borderWidth;
        }

        public void DrawStatic(SpriteBatch batch)
        {
            top.DrawStatic(batch);
            bot.DrawStatic(batch);
            left.DrawStatic(batch);
            right.DrawStatic(batch);
        }

        public void DrawDynamic(SpriteBatch sb, Point offset)
        {
            top.DrawDynamic(sb,offset);
            bot.DrawDynamic(sb, offset);
            left.DrawDynamic(sb, offset);
            right.DrawDynamic(sb, offset);
        }
    }
}
