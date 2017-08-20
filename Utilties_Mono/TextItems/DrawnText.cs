using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Utilties_Mono
{
    public class DrawnText
    {
        /// <summary>
        /// Help varibile. This variabile is only used when you call Draw function without offset parameter.
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        /// Cursor position within text.
        /// </summary>
        public int CursorPosition { get; set; }

        public HorizontalAligment TextHalign { get; set; }
        public VerticalAligment TextValign { get; set; }
        public Point TextMargin { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }

        /// <summary>
        /// Minimal size of rect.
        /// </summary>
        public Point MaxSize { get; set; }

        /// <summary>
        /// Maximal size of rect.
        /// </summary>
        public Point MinSize { get; set; }

        /// <summary>
        /// Background color of rectangle behind text.
        /// </summary>
        public Texture2D Background { get; set; }

        /// <summary>
        /// Color of text.
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// TRUE: show cursor in text. You can move it by changing value of CursorPosition.
        /// </summary>
        public bool HasCursor { get; set; }

        /// <summary>
        /// Determines what line of text will be first visible. 
        /// (example: There are 10 lines of text in total, only 3 of them can be shown at time. When ShowFromIndex is set to 6, visible lines will be:6,7,8)
        /// </summary>
        public int ShowFromIndex { get; private set; }

        /// <summary>
        /// Maximal count of rows, that can be shown at time.
        /// </summary>
        public int MaxShownRows { get; private set; }

        /// <summary>
        /// Text divided to rows.
        /// </summary>
        internal DrawnTextRow[] Rows { get; private set; }

        int offsetY;                //Y position of first visible row.
        TextCursor textCursor;
        Point size;                 //Real size of this instance.

        public DrawnText(SpriteFont font, string text, HorizontalAligment textHalign, VerticalAligment textValign)
        {
            this.ForeColor = Color.Black;
            this.Font = font;
            this.Text = text;
            this.TextHalign = textHalign;
            this.TextValign = textValign;
            this.MaxSize = new Point(int.MaxValue);
            this.MinSize = new Point(0);
            if (font != null)
                this.textCursor = new TextCursor(this);
        }
        #region RichTextFormat functions
        public void AddText(string text)
        {
            this.Text = Text.Substring(0, CursorPosition) + text + Text.Substring(CursorPosition, Text.Length - CursorPosition);
            textCursor.stayAtEndOfRow = CursorPositions.BasicPosition;
            CursorPosition += text.Length;
        }

        public void BackSpace()
        {
            textCursor.stayAtEndOfRow = CursorPositions.BasicPosition;
            if (CursorPosition > 0)
            {
                CursorPosition--;
                DeleteChar();
            }
        }

        public void GoUp()
        {
            textCursor.GoUpDown(true);
        }

        public void GoDown()
        {
            textCursor.GoUpDown(false);
        }

        public void GoRight()
        {
            textCursor.stayAtEndOfRow = CursorPositions.BasicPosition;
            CursorPosition++;
            if (CursorPosition > Text.Length)
                CursorPosition = Text.Length;
        }

        public void GoLeft()
        {
            textCursor.stayAtEndOfRow = CursorPositions.BasicPosition;
            CursorPosition--;
            if (CursorPosition < 0)
                CursorPosition = 0;
        }

        public void DeleteChar()
        {
            if (CursorPosition < Text.Length)
            {
                textCursor.stayAtEndOfRow = CursorPositions.BasicPosition;
                Text = Text.Substring(0, CursorPosition) + Text.Substring(CursorPosition + 1, Text.Length - CursorPosition - 1);
            }
        }
        #endregion

        /// <summary>
        /// Call this after any property has been changed.
        /// </summary>
        public void Changed()
        {
            if (HasCursor)
            {
                if (textCursor == null)
                    textCursor = new TextCursor(this);
                else
                    textCursor.ResizeCursor();
                textCursor.ResizeCursor();
            }
            else
                textCursor = null;

            //Create rows
            this.Rows = DrawnTextHelper.GetRows(Text, MaxSize.X - 2 * TextMargin.X, Font).ToArray();

            size.X = 0;
            for (int i = 0; i < Rows.Length; i++)
                size.X = Math.Max(size.X, (int)Rows[i].Width);
            size.X += 2 * TextMargin.X;

            MaxShownRows = (MaxSize.Y - 2 * TextMargin.Y) / Font.LineSpacing;
            int maxRows = Math.Min(MaxShownRows, Rows.Length);
            size.Y = maxRows * Font.LineSpacing + 2 * TextMargin.Y;
            //Repair size
            size.X = Math.Max(MinSize.X, size.X);
            size.X = Math.Min(MaxSize.X, size.X);
            size.Y = Math.Max(MinSize.Y, size.Y);
            size.Y = Math.Min(MaxSize.Y, size.Y);
            for (int i = 0; i < Rows.Length; i++)
            {
                switch (TextHalign)
                {
                    case HorizontalAligment.Left:
                        Rows[i].OffsetX = TextMargin.X;
                        break;
                    case HorizontalAligment.Right:
                        Rows[i].OffsetX = (int)(size.X - TextMargin.X - Rows[i].Width);
                        break;
                    case HorizontalAligment.Center:
                        Rows[i].OffsetX = (int)(size.X - Rows[i].Width) / 2;
                        break;
                }
            }

            switch (TextValign)
            {
                case VerticalAligment.Top:
                    offsetY = TextMargin.Y;
                    break;
                case VerticalAligment.Bottom:
                    offsetY = size.Y - TextMargin.Y - maxRows * Font.LineSpacing;
                    break;
                case VerticalAligment.Center:
                    offsetY = (size.Y - maxRows * Font.LineSpacing) / 2;
                    break;
            }
            if (textCursor != null)
            {
                textCursor.Calculate();
                CalculateShowFromIndex();
            }
            RepairShowFromIndex();
        }

        /// <summary>
        /// Returns required size to show all text.
        /// </summary>
        /// <returns></returns>
        public Point GetSize()
        {
            return size;
        }

        /// <summary>
        /// Calculates position from which row text is visible, so
        /// cursor is always shown.
        /// </summary>
        private void CalculateShowFromIndex()
        {
            if (ShowFromIndex + MaxShownRows <= textCursor.Row)
            {
                ShowFromIndex = textCursor.Row - MaxShownRows + 1;
                return;
            }
            if (ShowFromIndex > textCursor.Row)
                ShowFromIndex = textCursor.Row;
        }

        private void RepairShowFromIndex()
        {
            if (ShowFromIndex < 0)
            {
                ShowFromIndex = 0;
                return;
            }
            if (MaxShownRows >= Rows.Length)
            {
                ShowFromIndex = 0;
                return;
            }
            if (Rows.Length - ShowFromIndex < MaxShownRows)
                ShowFromIndex = Rows.Length - MaxShownRows;
        }

        public void Draw(SpriteBatch sb)
        {
            Draw(sb, this.Offset);
        }

        public void Draw(SpriteBatch sb, Point offset)
        {
            switch (TextHalign)
            {
                case HorizontalAligment.Center:
                    offset.X -= size.X / 2;
                    break;
                case HorizontalAligment.Right:
                    offset.X -= size.X;
                    break;
            }
            switch (TextValign)
            {
                case VerticalAligment.Center:
                    offset.Y -= size.Y / 2;
                    break;
                case VerticalAligment.Bottom:
                    offset.Y -= size.Y;
                    break;
            }
            DrawTopLeftCorner(sb, offset);
        }

        public void DrawTopLeftCorner(SpriteBatch sb, Point offset)
        {
            if (this.Text == null || this.Font == null)
                return;
            //Draw texture.
            if (this.Background != null)
            {
                Rectangle rect = new Rectangle(offset.X, offset.Y, size.X, size.Y);
                sb.Draw(this.Background, rect, Color.White);
            }
            //Draw rows.
            Vector2 vector = offset.ToVector2();
            float curOffsetY = offsetY;
            int max = Math.Min(Rows.Length, ShowFromIndex + MaxShownRows);
            for (int i = ShowFromIndex; i < max; i++)
            {
                sb.DrawString(Font, Rows[i].Text, vector + new Vector2(Rows[i].OffsetX, curOffsetY), ForeColor);
                curOffsetY += Font.LineSpacing;
            }
            //Draw cursor
            if (textCursor != null)
                textCursor.Draw(sb, offset, ShowFromIndex, offsetY);
        }
    }

}
