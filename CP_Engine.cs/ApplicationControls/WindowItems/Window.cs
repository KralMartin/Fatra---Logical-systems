using ContextMenu_Mono;
using ContextMenu_Mono.TabWindow;
using CP_Engine.SchemeItems;
using CP_Engine.WindowAssistants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities_Mono;

namespace CP_Engine
{
    /// <summary>
    /// Provides functions to manipulate witch scheme(zoom, move).
    /// Provides functions to trasfer coords to global positions.
    /// </summary>
    public class Window : Tab
    {
        private const int MinimumTilesShownInWindow = 2;    //User will see at last this many tiles in each dimension.

        /// <summary>
        /// Collection of selected tiles.
        /// </summary>
        public Selection Selection { get; private set; }

        /// <summary>
        /// Width of tiles. Can by modified by zooming out/in.
        /// Sum of tile width and tile spacing.
        /// </summary>
        internal int RealTileWidth { get; private set; }

        /// <summary>
        /// Pointer to workplace.
        /// </summary>
        internal WorkPlace WorkPlace { get; private set; }

        /// <summary>
        /// Scheme, which is shown by this window.
        /// </summary>
        internal Scheme Scheme { get; private set; }

        /// <summary>
        /// PScheme shown by this window.
        /// </summary>
        internal PhysScheme PhysScheme { get; private set; }


        Point OffsetToScheme;           //Top left corner of window in scheme.
        Rectangle bounds;               //Bounds of window.
        WindowDraw windowDraw;
        SpriteFont font;

        int minX, minY, maxX, maxY;     //range of tiles that are visible for user.
        int defX, defY;                 //Relative offset of tile[minX,minY] to top left corner of window.

        internal Window(WorkPlace workplace, Rectangle bounds, PhysScheme physScheme)
        {
            this.WorkPlace = workplace;
            this.PhysScheme = physScheme;
            this.Scheme = physScheme.PlacedBug.Bug.Scheme;
            
            //Create border and calculate inner bounds
            RectBorder border = new RectBorder(4, bounds, ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black));
            this.bounds = border.GetInnerRectange();

            this.windowDraw = new WindowDraw(this, border);
            this.font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            this.Selection = new Selection(this);

            //Initialize size and position of window.
            this.RealTileWidth = 24;
            Zoom(0);
            MoveScreenOffsetTo(new Point());
        }

        public void ExpandScheme(int side, int tiles)
        {
            this.Scheme.Expand(side, tiles);
        }

        #region Position transform functions

        /// <summary>
        /// Returns global offset of top left corent of tile on provided coords.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal Point CoordsToScreenPos(Point coords)
        {
            coords.X *= RealTileWidth;
            coords.Y *= RealTileWidth;

            coords = coords - this.OffsetToScheme;
            coords.X += bounds.X;
            coords.Y += bounds.Y;
            return coords;
        }

        /// <summary>
        /// Returns side, that is closes to provided point.
        /// Divides tile to 4 triangles. Returns side on which provided point intersects with triangle.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        internal int GetSideOfPointInTile(Point mousePosition)
        {
            //Get position of mouse within tile.
            Point pointInTile = GetPositionWithinScheme(mousePosition);
            pointInTile.X = pointInTile.X % this.RealTileWidth;
            pointInTile.Y = pointInTile.Y % this.RealTileWidth;

            //Calculate how to properly rotate item.
            int[] distances = new int[4];
            for (int i = 0; i < 4; i++)
            {
                Point basePoint = GetBasePoint(i);
                distances[i] = Math.Abs(pointInTile.X - basePoint.X);
                distances[i] += Math.Abs(pointInTile.Y - basePoint.Y);
            }
            int minDistance = int.MaxValue;
            int minSide = 0;
            for (int i = 0; i < 4; i++)
            {
                if (minDistance > distances[i])
                {
                    minDistance = distances[i];
                    minSide = i;
                }
            }
            return minSide;
        }
        
        public void CutOffScheme()
        {
            this.Scheme.CutOff();
        }

        /// <summary>
        /// * * 1 * *   1 top base
        /// *       *   2 right base
        /// 2       3   3 left base
        /// *       *   4 bot base
        /// * * 4 * *
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        private Point GetBasePoint(int side)
        {
            int tileWidth = this.RealTileWidth;
            switch (side)
            {
                case Sides.Top:
                    return new Point(tileWidth / 2, 0);
                case Sides.Bot:
                    return new Point(tileWidth / 2, tileWidth);
                case Sides.Left:
                    return new Point(0, tileWidth / 2);
                case Sides.Right:
                    return new Point(tileWidth, tileWidth / 2);
                default:
                    return new Point();
            }
        }

        /// <summary>
        /// Gets closest tile coords to provided position.
        /// </summary>
        /// <param name="globalPosition">Screen mouse position.</param>
        /// <returns></returns>
        internal Point GetClosestTile(Point globalPosition)
        {
            //Get position inside window.
            globalPosition.X -= bounds.X;
            globalPosition.Y -= bounds.Y;

            //Get position in scheme.
            globalPosition.X += OffsetToScheme.X;
            globalPosition.Y += OffsetToScheme.Y;

            if (globalPosition.X < 0)
                globalPosition.X = 0;
            if (globalPosition.Y < 0)
                globalPosition.Y = 0;

            //Get tile coordinates from position.
            globalPosition.X /= RealTileWidth;
            globalPosition.Y /= RealTileWidth;

            if (globalPosition.X >= this.Scheme.TilesCountX)
                globalPosition.X = this.Scheme.TilesCountX - 1;
            if (globalPosition.Y >= this.Scheme.TilesCountY)
                globalPosition.Y = this.Scheme.TilesCountY - 1;

            return globalPosition;
        }

        /// <summary>
        /// Transforms mouse screen position to tile coords.
        /// Returns (-1,-1) point, if no tile is hovered.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        internal Point GetTileAt(Point mousePosition)
        {
            //Get position inside window.
            mousePosition.X -= bounds.X;
            mousePosition.Y -= bounds.Y;

            if (mousePosition.X < OffsetToScheme.X * -1 || mousePosition.Y < OffsetToScheme.Y * -1)
                return new Point(-1, -1);

            //Get position in scheme.
            mousePosition.X += OffsetToScheme.X;
            mousePosition.Y += OffsetToScheme.Y;

            //Get tile coordinates from position.
            mousePosition.X /= RealTileWidth;
            mousePosition.Y /= RealTileWidth;

            if (mousePosition.X >= this.Scheme.TilesCountX || mousePosition.Y >= this.Scheme.TilesCountY)
                return new Point(-1, -1);
            return mousePosition;
        }

        /// <summary>
        /// Converts mouse position on screen to position within scheme.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        internal Point GetPositionWithinScheme(Point mousePosition)
        {
            //Get position inside window.
            mousePosition.X -= bounds.X;
            mousePosition.Y -= bounds.Y;

            //Get position in scheme.
            mousePosition.X += OffsetToScheme.X;
            mousePosition.Y += OffsetToScheme.Y;

            return mousePosition;
        }
        #endregion
        #region Window screen movement.

        /// <summary>
        /// Zooms in/out grid.
        /// </summary>
        /// <param name="delta"></param>
        internal void Zoom(int delta)
        {
            Vector2 center = new Vector2((OffsetToScheme.X + bounds.Width / 2f) / (float)RealTileWidth, (OffsetToScheme.Y + bounds.Height / 2f) / (float)RealTileWidth);
            int step = 5;
            if (delta == 0)
                step = 0;
            else if (delta < 0)
                step *= -1;
            RealTileWidth += step;//qwe
            //RealTileWidth = 32;
            if (RealTileWidth < 10)
                RealTileWidth = 10;
            windowDraw.SetWidths(RealTileWidth);
            Center(center);
        }

        /// <summary>
        /// Centers window to new position.
        /// </summary>
        /// <param name="position">Position in scheme.</param>
        internal void Center(Vector2 position)
        {
            position.X = (position.X * RealTileWidth) - bounds.Width / 2f;
            position.Y = position.Y * RealTileWidth - bounds.Height / 2f;
            MoveScreenOffsetTo(position.ToPoint());
        }

        /// <summary>
        /// Adjust window's top left corner by point.
        /// </summary>
        /// <param name="point"></param>
        internal void MoveSceenOffsetBy(Point point)
        {
            this.OffsetToScheme.X += point.X;
            this.OffsetToScheme.Y += point.Y;
            MoveScreenOffsetTo(this.OffsetToScheme);
        }

        /// <summary>
        /// Sets window's top left corner to new position.
        /// </summary>
        /// <param name="newPosition">Position in scheme.</param>
        private void MoveScreenOffsetTo(Point newPosition)
        {
            this.OffsetToScheme = newPosition;

            //Make sure, user allways see at last a bit of scheme.
            if (OffsetToScheme.X * -1 > this.bounds.Width - Window.MinimumTilesShownInWindow * RealTileWidth)
                OffsetToScheme.X = (this.bounds.Width - Window.MinimumTilesShownInWindow * RealTileWidth) * -1;
            else if ((Scheme.TilesCountX * RealTileWidth - OffsetToScheme.X) < Window.MinimumTilesShownInWindow * RealTileWidth)
                this.OffsetToScheme.X = Scheme.TilesCountX*RealTileWidth - Window.MinimumTilesShownInWindow * RealTileWidth;

            if (OffsetToScheme.Y * -1 > this.bounds.Height - Window.MinimumTilesShownInWindow * RealTileWidth)
                OffsetToScheme.Y = (this.bounds.Height - Window.MinimumTilesShownInWindow * RealTileWidth) * -1;
            else if ((Scheme.TilesCountY * RealTileWidth - OffsetToScheme.Y) < Window.MinimumTilesShownInWindow * RealTileWidth)
                this.OffsetToScheme.Y = Scheme.TilesCountY * RealTileWidth - Window.MinimumTilesShownInWindow * RealTileWidth;

            if (OffsetToScheme.X > 0)
            {
                minX = (int)OffsetToScheme.X / RealTileWidth;
                defX = (int)OffsetToScheme.X % RealTileWidth * -1;
                maxX = (bounds.Width - defX) / RealTileWidth + 1 + minX;
            }
            else
            {
                minX = 0;
                defX = (int)OffsetToScheme.X * -1;
                maxX = (int)((OffsetToScheme.X + bounds.Width) / RealTileWidth) + 1;
            }
            if (maxX >= Scheme.TilesCountX)
                maxX = Scheme.TilesCountX;

            if (OffsetToScheme.Y > 0)
            {
                minY = (int)OffsetToScheme.Y / RealTileWidth;
                defY = (int)OffsetToScheme.Y % RealTileWidth * -1;
                maxY = (bounds.Height - defY) / RealTileWidth + 1 + minY;
            }
            else
            {
                minY = 0;
                defY = (int)OffsetToScheme.Y * -1;
                maxY = (int)((OffsetToScheme.Y + bounds.Height) / RealTileWidth) + 1;
            }
            if (maxY >= Scheme.TilesCountY)
                maxY = Scheme.TilesCountY;
        }
        #endregion

        protected override void Resize()
        {
            //Create border and calculate inner bounds
            RectBorder border = new RectBorder(4, this.TabBounds, ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black));
            this.bounds = border.GetInnerRectange();

            this.windowDraw = new WindowDraw(this, border);
            windowDraw.SetWidths(RealTileWidth);
            this.MoveSceenOffsetBy(new Point());
        }

        public sealed override void Draw(SpriteBatch sb)
        {
            windowDraw.Draw(sb, this.bounds, minX, minY, maxX, maxY, defX, defY);
        }
    }
}
