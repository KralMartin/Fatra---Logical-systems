using ContextMenu_Mono;
using CP_Engine.BugItems;
using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Utilities_Mono;
using Utilties_Mono;

namespace CP_Engine.WindowAssistants
{
    class WindowDraw
    {
        Window window;
        RectBorder border;
        TileData defaultData;

        private int tileWidth;              //Width of tile.
        private int tileSpacing;            //Space between tiles
        private int vireWidthWide;
        private int vireOffsetWide;
        private int joinOffset;
        private int vireWidth;
        private int vireOffset;
        private int joinWidth;

        private Texture2D spacingTexture;
        private Texture2D bugBorderTexture;
        private Texture2D backgroundTexture;
        private Texture2D selectedTexture;
        private Texture2D vireTexture;
        private Texture2D[] diodeTexture;
        private Texture2D[] konvertorTexture;
        private Texture2D inputTexture;
        private Texture2D outputTexture;
        private Texture2D breakPointTexture;

        private Color OnColor;
        private Color OffColor;

        private SpriteFont font;

        internal WindowDraw(Window window, RectBorder border)
        {
            this.window = window;
            this.border = border;

            spacingTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray);
            backgroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.DarkGray());
            bugBorderTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            vireTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Wheat);

            inputTexture = ImportantClassesCollection.TextureLoader.GetTexture(@"InOutput\Input");
            outputTexture = ImportantClassesCollection.TextureLoader.GetTexture(@"InOutput\Output");
            breakPointTexture = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Other\breakPoint"), Color.Black, Color.Transparent);
            font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            defaultData = new TileData(0);

            konvertorTexture = new Texture2D[4];
            konvertorTexture[Sides.Top] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Konvertor\North"), Color.Black, Color.Transparent);
            konvertorTexture[Sides.Bot] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Konvertor\South"), Color.Black, Color.Transparent);
            konvertorTexture[Sides.Left] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Konvertor\East"), Color.Black, Color.Transparent);
            konvertorTexture[Sides.Right] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Konvertor\West"), Color.Black, Color.Transparent);
            diodeTexture = new Texture2D[4];
            diodeTexture[Sides.Top] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Diode\North"), Color.Black, Color.Transparent);
            diodeTexture[Sides.Bot] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Diode\South"), Color.Black, Color.Transparent);
            diodeTexture[Sides.Left] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Diode\East"), Color.Black, Color.Transparent);
            diodeTexture[Sides.Right] = ImportantClassesCollection.TextureLoader.ChangeColor(ImportantClassesCollection.TextureLoader.GetTexture(@"Diode\West"), Color.Black, Color.Transparent);
            OnColor = Color.LimeGreen;
            OffColor = Color.Green;
        }

        /// <summary>
        /// Updates variabiles required for drawing.
        /// Call after window was zoomed in/out.
        /// </summary>
        /// <param name="realTileWidth"></param>
        internal void SetWidths(int realTileWidth)
        {
            tileSpacing = 1;
            tileWidth = realTileWidth - tileSpacing;

            vireOffset = -tileWidth / 20 + 1 + tileWidth / 2;
            vireWidth = tileWidth / 10;

            vireOffsetWide = -tileWidth / 10 + 1 + tileWidth / 2;
            vireWidthWide = tileWidth / 5;

            joinWidth = vireWidthWide + (vireOffset - vireOffsetWide);
            joinOffset = (tileWidth - joinWidth) / 2 + tileSpacing;
        }

        internal void Draw(SpriteBatch sb, Rectangle bounds, int minX, int minY, int maxX, int maxY, int defX, int defY)
        {
            sb.Draw(backgroundTexture, bounds, Color.White);
            //SeletedTextureColor qwe repair
            if (window.Selection.IsValid)
                this.selectedTexture = GlobalSettings.SelectedTextureValid;
            else
                this.selectedTexture = GlobalSettings.SelectedTextureInValid;

            //Draw vertical lines.
            Rectangle spacingRect = new Rectangle(bounds.X + defX, bounds.Y + defY, tileSpacing, (maxY - minY) * (tileSpacing + tileWidth));
            for (int x = minX; x <= maxX; x++)
            {
                sb.Draw(spacingTexture, spacingRect, Color.White);
                spacingRect.X += tileWidth + tileSpacing;
            }
            //Draw horizontal lines.
            spacingRect = new Rectangle(bounds.X + defX, bounds.Y + defY, (maxX - minX) * (tileSpacing + tileWidth), tileSpacing);
            for (int y = minY; y < maxY + 1; y++)
            {
                sb.Draw(spacingTexture, spacingRect, Color.White);
                spacingRect.Y += tileWidth + tileSpacing;
            }
            //Init variabiles.
            Rectangle tileRect = new Rectangle(0, bounds.Y + defY + tileSpacing, tileWidth, tileWidth);
            Rectangle joinRect = new Rectangle(0, 0, joinWidth, joinWidth);
            Rectangle vireRect = new Rectangle();
            Tile tile;
            TileData data;
            TileInfoItem info = null;
            Color horzColor = Color.White;
            Color vertColor = Color.White;
            Vector2 textOffset;
            string centerText = null;
            List<int> visibleBugs = new List<int>();
            List<DrawnText> visibleText = new List<DrawnText>();

            //Draw scene
            for (int y = minY; y < maxY; y++)
            {
                tileRect.X = bounds.X + defX + tileSpacing;

                for (int x = minX; x < maxX; x++)
                {
                    tile = window.Scheme.Get_Tile(new Point(x, y));
                    if (tile != null)
                    {
                        data = tile.Data;
                        joinRect.X = tileRect.X + joinOffset;
                        joinRect.Y = tileRect.Y + joinOffset;
                        textOffset = new Vector2(tileRect.X, tileRect.Y);

                        if (TilesInfo.IsBugType(data.Type))
                        {
                            if (visibleBugs.Contains(data.HorzWidth) == false)
                                visibleBugs.Add(data.HorzWidth);
                        }
                        else
                        {
                            info = TilesInfo.GetItem(data.Type);
                            if (info.IsSource())
                            {
                                List<SchemeSource> sources = window.Scheme.Sources.GetSources(new Point(x, y));
                                bool sourceValue = false;
                                foreach (SchemeSource sSource in sources)
                                {
                                    if (sSource.GetValue(window.PhysScheme))
                                    {
                                        sourceValue = true;
                                        break;
                                    }
                                }
                                if (sourceValue)
                                    horzColor = OnColor;
                                else
                                    horzColor = OffColor;

                                IODescription desc = window.Scheme.Bug.IODecription.Get_Description(new Point(x, y));
                                if (desc != null)
                                    centerText = (desc.Order + 1) + ". " + desc.DisplayText;
                            }
                            else
                            {
                                if (window.PhysScheme.GetHorizontalValue(tile))
                                    horzColor = OnColor;
                                else
                                    horzColor = OffColor;

                                if (window.PhysScheme.GetVerticalValue(tile))
                                    vertColor = OnColor;
                                else
                                    vertColor = OffColor;
                            }
                            //Draw background color.
                            if (data.ColorID > 0)
                                sb.Draw(GlobalSettings.TileColors[data.ColorID].Texture, tileRect, Color.White);

                            //if (horzColor != Color.Red)
                            //    horzColor = Color.Yellow;
                            //if (vertColor != Color.Red)
                            //    vertColor = Color.Yellow;

                            #region Draw switch
                            switch (data.Type)
                            {
                                case 1:
                                    // 1
                                    //1 0
                                    // 1
                                    vireRect.X = tileRect.X;
                                    vireRect.Width = tileWidth / 2;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);

                                    vireRect.Y = tileRect.Y;
                                    vireRect.Height = tileWidth;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    sb.Draw(this.vireTexture, joinRect, Color.Red);
                                    if (data.HorzWidth > 1 || data.VertWidth > 1)
                                        sb.DrawString(font, data.Offset.ToString(), textOffset, Color.Black);
                                    break;
                                case 2:
                                    // 1
                                    //1 1
                                    // 0
                                    vireRect.X = tileRect.X;
                                    vireRect.Width = tileWidth;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);

                                    vireRect.Y = tileRect.Y;
                                    vireRect.Height = tileWidth / 2;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    sb.Draw(this.vireTexture, joinRect, Color.Red);
                                    if (data.HorzWidth > 1 || data.VertWidth > 1)
                                        sb.DrawString(font, data.Offset.ToString(), textOffset, Color.Black);
                                    break;
                                case 3:
                                    // 1
                                    //0 1
                                    // 1
                                    vireRect.X = tileRect.X + tileWidth / 2;
                                    vireRect.Width = tileWidth / 2;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);

                                    vireRect.Y = tileRect.Y;
                                    vireRect.Height = tileWidth;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    sb.Draw(this.vireTexture, joinRect, Color.Red);
                                    if (data.HorzWidth > 1 || data.VertWidth > 1)
                                        sb.DrawString(font, data.Offset.ToString(), textOffset, Color.Black);
                                    break;
                                case 4:
                                    // 0
                                    //1 1
                                    // 1
                                    vireRect.X = tileRect.X;
                                    vireRect.Width = tileWidth;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);

                                    vireRect.Y = tileRect.Y + tileWidth / 2;
                                    vireRect.Height = tileWidth / 2;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    sb.Draw(this.vireTexture, joinRect, Color.Red);
                                    if (data.HorzWidth > 1 || data.VertWidth > 1)
                                        sb.DrawString(font, data.Offset.ToString(), textOffset, Color.Black);
                                    break;
                                case 5:
                                    // 0
                                    //1 1
                                    // 0
                                    vireRect.X = tileRect.X;
                                    vireRect.Width = tileWidth;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);
                                    break;
                                case 6:
                                    // 1
                                    //0 0
                                    // 1
                                    vireRect.Y = tileRect.Y;
                                    vireRect.Height = tileWidth;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    break;
                                case 7:
                                    // 1
                                    //1 1
                                    // 1
                                    vireRect.X = tileRect.X;
                                    vireRect.Width = tileWidth;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);

                                    vireRect.Y = tileRect.Y;
                                    vireRect.Height = tileWidth;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    break;
                                case 8:
                                    // 1
                                    //0 1
                                    // 0
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                        vireRect.Y = tileRect.Y;
                                        vireRect.Height = vireOffsetWide + vireWidthWide;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X + tileRect.Width / 2;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                        vireRect.Y = tileRect.Y;
                                        vireRect.Height = vireOffset + vireWidth;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X + tileRect.Width / 2;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    break;
                                case 9:
                                    // 0
                                    //0 1
                                    // 1
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = tileRect.Height - vireOffsetWide;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X + tileRect.Width / 2;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = tileRect.Height - vireOffset;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X + tileRect.Width / 2;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    break;
                                case 10:
                                    // 0
                                    //1 0
                                    // 1
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = tileRect.Height - vireOffsetWide;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = tileRect.Height - vireOffset;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    break;
                                case 11:
                                    // 1
                                    //1 0
                                    // 0
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                        vireRect.Y = tileRect.Y;
                                        vireRect.Height = vireOffsetWide + vireWidthWide;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                        vireRect.Y = tileRect.Y;
                                        vireRect.Height = vireOffset + vireWidth;
                                        sb.Draw(this.vireTexture, vireRect, vertColor);

                                        vireRect.X = tileRect.X;
                                        vireRect.Width = tileRect.Width / 2;
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                        sb.Draw(this.vireTexture, vireRect, horzColor);
                                    }
                                    break;
                                case 12:
                                    // 1
                                    //1x1
                                    // 1
                                    vireRect.X = tileRect.X;
                                    vireRect.Width = tileWidth;
                                    if (data.HorzWidth > 1)
                                    {
                                        vireRect.Y = tileRect.Y + vireOffsetWide;
                                        vireRect.Height = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.Y = tileRect.Y + vireOffset;
                                        vireRect.Height = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, horzColor);
                                    vireRect.Y = tileRect.Y;
                                    vireRect.Height = tileWidth;
                                    if (data.VertWidth > 1)
                                    {
                                        vireRect.X = tileRect.X + vireOffsetWide;
                                        vireRect.Width = vireWidthWide;
                                    }
                                    else
                                    {
                                        vireRect.X = tileRect.X + vireOffset;
                                        vireRect.Width = vireWidth;
                                    }
                                    sb.Draw(this.vireTexture, vireRect, vertColor);
                                    sb.Draw(this.vireTexture, joinRect, Color.Red);
                                    break;
                                case 13:
                                    sb.Draw(this.konvertorTexture[Sides.Top], tileRect, horzColor);
                                    break;
                                case 14:
                                    sb.Draw(this.konvertorTexture[Sides.Left], tileRect, horzColor);
                                    break;
                                case 15:
                                    sb.Draw(this.konvertorTexture[Sides.Bot], tileRect, horzColor);
                                    break;
                                case 16:
                                    sb.Draw(this.konvertorTexture[Sides.Right], tileRect, horzColor);
                                    break;
                                case 17:
                                    sb.Draw(this.diodeTexture[Sides.Top], tileRect, horzColor);
                                    break;
                                case 18:
                                    sb.Draw(this.diodeTexture[Sides.Left], tileRect, horzColor);
                                    break;
                                case 19:
                                    sb.Draw(this.diodeTexture[Sides.Bot], tileRect, horzColor);
                                    break;
                                case 20:
                                    sb.Draw(this.diodeTexture[Sides.Right], tileRect, horzColor);
                                    break;
                                case 21:
                                    sb.Draw(this.inputTexture, tileRect, horzColor);
                                    if (centerText != null)
                                        visibleText.Add(DrawText(sb, centerText, new Point(tileRect.X, tileRect.Y)));
                                    break;
                                case 22:
                                    sb.Draw(this.outputTexture, tileRect, horzColor);
                                    if (centerText != null)
                                        visibleText.Add(DrawText(sb, centerText, new Point(tileRect.X, tileRect.Y)));
                                    break;
                                default:
                                    break;
                            }
                            #endregion

                            //Draw breakpoint.
                            if (tile.BreakPoint != null)
                                sb.Draw(breakPointTexture, tileRect, Color.White);
                            //Draw selected effect.
                            if (window.Selection.Items.Contains(new Point(x, y)))
                                sb.Draw(selectedTexture, tileRect, Color.White);
                        }
                    }
                    tileRect.X += tileWidth + tileSpacing;
                    //end of x loop
                }
                tileRect.Y += tileWidth + tileSpacing;
                //end of y loop
            }
            foreach (int pBugID in visibleBugs)
            {
                PlacedBug pBug = window.Scheme.PlacedBugs.Get(pBugID);
                DrawBug(sb, pBug);
            }
            foreach (DrawnText dt in visibleText)
                dt.Draw(sb);
            border.DrawStatic(sb);
        }
        
        private DrawnText DrawText(SpriteBatch sb, string text, Point tileOffset)
        {
            if (text == null)
                return null;
            DrawnText dt = new DrawnText(this.font, text, HorizontalAligment.Center, VerticalAligment.Center);
            dt.Changed();
            tileOffset.X += window.RealTileWidth / 2;
            tileOffset.Y += window.RealTileWidth / 2;
            dt.Offset = tileOffset;
            return dt;
        }

        private void DrawBug(SpriteBatch sb, PlacedBug pBug)
        {
            Bug bug = pBug.Bug;
            Point position = window.CoordsToScreenPos(pBug.Coords);
            //Draw border.
            RectBorder border = new RectBorder(this.tileSpacing * 2, new Rectangle(position.X, position.Y, bug.GetBugWidth() * window.RealTileWidth, 2 * window.RealTileWidth), this.bugBorderTexture);
            border.DrawStatic(sb);
            //Draw background.
            Rectangle innerRect = border.GetInnerRectange();
            sb.Draw(GlobalSettings.BugColors[pBug.Bug.ColorID].Texture, border.GetInnerRectange(), Color.White);

            if (GlobalSettings.ShowIOputs)
            {
                Vector2 baseTextOffset = position.ToVector2();
                Vector2 textOffset = baseTextOffset;
                int index = 0;

                foreach (IODescription current in bug.IODecription.Get_Inputs())
                {
                    textOffset.X = (this.window.RealTileWidth * index + this.window.RealTileWidth / 2) + baseTextOffset.X - font.MeasureString(current.DisplayText).X / 2;
                    sb.DrawString(this.font, current.DisplayText, textOffset, Color.Black);
                    index++;
                }

                index = 0;
                textOffset = baseTextOffset;
                textOffset.Y += 2 * window.RealTileWidth - this.font.LineSpacing;
                foreach (IODescription current in bug.IODecription.Get_Outputs())
                {
                    textOffset.X = (this.window.RealTileWidth * index + this.window.RealTileWidth / 2) + baseTextOffset.X - font.MeasureString(current.DisplayText).X / 2;
                    sb.DrawString(this.font, current.DisplayText, textOffset, Color.Black);
                    index++;
                }
            }
            //Display text
            string displayText = "";
            if (pBug.Order >= 0)
                displayText = (pBug.Order + 1) + ". ";
            if (pBug.Bug.IsUserCreated())
                displayText += pBug.Bug.GetDisplayText(window.PhysScheme, pBug);
            else
                displayText += pBug.Bug.GetDisplayText(window.PhysScheme, pBug);

            Point centralPosition = position;
            centralPosition.Y += (int)(this.window.RealTileWidth * 2) / 2;
            centralPosition.X += (int)(this.window.RealTileWidth * bug.GetBugWidth()) / 2;

            DrawnText dt = new DrawnText(this.font, displayText, HorizontalAligment.Center, VerticalAligment.Center);
            dt.Changed();
            dt.Draw(sb, centralPosition);
        }
    }
}
