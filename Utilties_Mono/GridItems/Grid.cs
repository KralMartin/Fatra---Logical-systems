using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities_Mono;

namespace Utilties_Mono.GridItems
{
    public class Grid
    {
        public Rectangle Bunds { get; private set; }
        public Tile[,] Tiles { get; set; }

        RectBorder border;
        Point offset;
        int tilesX, tilesY;
        int minX, defX, minY, defY, maxX, maxY;
        int tileWidth;
        int margin;
        SpriteFont font;

        public Grid(Rectangle bounds, int borderWidth, int tilesX, int tilesY, Tile[,] tiles)
        {
            this.Tiles = tiles;
            this.border = new RectBorder(borderWidth, bounds, TextureLoader.Instance.CreateSimpleTexture(Color.Black));
            this.Bunds = border.GetInnerRectange();
            this.tilesX = tilesX;
            this.tilesY = tilesY;
            this.font = TextureLoader.Instance.GetFont("f1");
            margin = 0;
            SetZoom(16);
            MoveScreenOffsetTo(new Point());
        }

        public void SetMargin(int value)
        {
            this.margin = value;
            MoveScreenOffsetTo(offset);
        }

        public void SetZoom(int value)
        {
            int realTileWidth = tileWidth + margin;      //tilewidth+margin
            Vector2 center = new Vector2((offset.X + Bunds.Width / 2f) / (float)realTileWidth, (offset.Y + Bunds.Height / 2f) / (float)realTileWidth);
            this.tileWidth = value;
            if (tileWidth < 4)
                tileWidth = 4;
            Center(center.ToPoint());
        }

        public void Zoom(int delta)
        {
            SetZoom(tileWidth + delta);
        }

        /// <summary>
        /// Centers window to new position.
        /// </summary>
        /// <param name="position">Position in scheme.</param>
        internal void Center(Point coords)
        {
            int realTileWidth = tileWidth + margin;      //tilewidth+margin
            coords.X = (int)(-Bunds.Width / 2f + realTileWidth * coords.X);
            coords.Y = (int)(-Bunds.Height / 2f + realTileWidth * coords.Y);

            MoveScreenOffsetTo(coords);
        }

        public void MoveScreenOffsetBy(Point delta)
        {
            MoveScreenOffsetTo(offset + delta);
        }

        public void MoveScreenOffsetTo(Point newPosition)
        {
            this.offset = newPosition;
            int realTileWidth= tileWidth+margin;      //tilewidth+margin

            //Make sure, user allways see at last a bit of scheme.
            //if (OffsetToScheme.X * -1 > this.bounds.Width - Window.MinimumTilesShownInWindow * RealTileWidth)
            //    OffsetToScheme.X = (this.bounds.Width - Window.MinimumTilesShownInWindow * RealTileWidth) * -1;
            //else if ((Scheme.TilesCountX * RealTileWidth - OffsetToScheme.X) < Window.MinimumTilesShownInWindow * RealTileWidth)
            //    this.OffsetToScheme.X = Scheme.TilesCountX * RealTileWidth - Window.MinimumTilesShownInWindow * RealTileWidth;

            //if (OffsetToScheme.Y * -1 > this.bounds.Height - Window.MinimumTilesShownInWindow * RealTileWidth)
            //    OffsetToScheme.Y = (this.bounds.Height - Window.MinimumTilesShownInWindow * RealTileWidth) * -1;
            //else if ((Scheme.TilesCountY * RealTileWidth - OffsetToScheme.Y) < Window.MinimumTilesShownInWindow * RealTileWidth)
            //    this.OffsetToScheme.Y = Scheme.TilesCountY * RealTileWidth - Window.MinimumTilesShownInWindow * RealTileWidth;

            if (offset.X > 0)
            {
                minX = (int)offset.X / realTileWidth;
                defX = (int)offset.X % realTileWidth * -1;
                maxX = (Bunds.Width - defX) / realTileWidth + 1 + minX;
            }
            else
            {
                minX = 0;
                defX = (int)offset.X * -1;
                maxX = (int)((offset.X + Bunds.Width) / realTileWidth) + 1;
            }
            if (maxX >= tilesX)
                maxX = tilesX;

            if (offset.Y > 0)
            {
                minY = (int)offset.Y / realTileWidth;
                defY = (int)offset.Y % realTileWidth * -1;
                maxY = (Bunds.Height - defY) / realTileWidth + 1 + minY;
            }
            else
            {
                minY = 0;
                defY = (int)offset.Y * -1;
                maxY = (int)((offset.Y + Bunds.Height) / realTileWidth) + 1;
            }
            if (maxY >= tilesY)
                maxY = tilesY;
        }

        public void Draw(SpriteBatch sb)
        {
            int realTileWidth = tileWidth + margin;      //tilewidth+margin
            Rectangle tileRect = new Rectangle(defX + Bunds.X, defY+ Bunds.Y, tileWidth, tileWidth);
            for (int y = minY; y < maxY; y++)
            {
                tileRect.X = Bunds.X + defX;

                for (int x = minX; x < maxX; x++)
                {
                    Tile tile= Tiles[x, y];
                    if (tile.texture != null)
                        sb.Draw(tile.texture, tileRect, Color.White);
                    //sb.DrawString(font, tile.text, new Vector2(tileRect.X, tileRect.Y), Color.White);
                    tileRect.X += realTileWidth;
                    //end of x loop
                }
                tileRect.Y +=  realTileWidth;
                //end of y loop
            }
            border.DrawStatic(sb);
        }
    }
}
