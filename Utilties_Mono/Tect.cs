using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Utilities_Mono
{
    /// <summary>
    /// Simple class to draw Texture at Rectangle.
    /// </summary>
    public class Tect
    {
        public Texture2D Texture { get; set; }
        public Rectangle Rectangle
        {
            get { return bounds; }
            set { bounds = value; }
        }
        protected Rectangle bounds;

        public Tect() { }

        public Tect(Texture2D texture, Rectangle bounds)
        {
            this.Texture = texture;
            this.bounds = bounds;
        }

        public bool Contains(Point point)
        {
            if (bounds.Contains(point))
                return true;
            return false;
        }

        public void DrawStatic(SpriteBatch sb)
        {
            sb.Draw(Texture, bounds, Color.White);
        }

        public void Set_X(int value)
        {
            bounds.X = value;
        }

        public void Set_Y(int value)
        {
            bounds.Y = value;
        }

        public void DrawDynamic(SpriteBatch sb, Point offset)
        {
            Rectangle rect = bounds;
            rect.X += offset.X;
            rect.Y += offset.Y;
            sb.Draw(Texture, rect, Color.White);
        }
    }
}
