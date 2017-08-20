using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.WorldNS
{
    public class Tile
    {
        int X;
        int Y;
        public Texture2D GroundTexture { get; set; }
        public double Elevation { get; set; }
        public Biome Biome { get; set; }

        public Tile(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void Draw(SpriteBatch batch, Rectangle position)
        {
            batch.Draw(GroundTexture, position, Color.White);
        }
    }
}
