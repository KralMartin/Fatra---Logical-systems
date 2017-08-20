using Microsoft.Xna.Framework;
using System;
using Utilities_Mono;
using Microsoft.Xna.Framework.Graphics;

namespace Utilties_Mono
{
    internal enum CursorPositions { BeginRow, EndRow, BasicPosition }
    internal class TextCursor
    {
        /// <summary>
        /// Cursor position within row.
        /// </summary>
        internal int PositionInRow { get; private set; }

        /// <summary>
        /// Row on which cursor is placed.
        /// </summary>
        internal int Row { get; private set; }

        int nextRowOffset;
        int currentRowOffset;
        int prevRowOffset;

        DrawnText drawnText;
        public CursorPositions stayAtEndOfRow;
        Tect cursor;

        internal TextCursor(DrawnText drawnText)
        {
            this.cursor = new Tect(TextureLoader.Instance.CreateSimpleTexture(Color.Black), new Rectangle(0, 0, (int)Math.Max(drawnText.Font.Spacing, 2), drawnText.Font.LineSpacing));
            this.drawnText = drawnText;
            this.stayAtEndOfRow = CursorPositions.BasicPosition;
        }

        /// <summary>
        /// Calculates new position of cursor,
        /// same way as you would press UP-ARROW in rtf editor.
        /// </summary>
        /// <param name="up_down"></param>
        internal void GoUpDown(bool up_down)
        {
            if (up_down)
            {
                if (this.Row == 0)
                    return;
                int delta = Math.Min(drawnText.Rows[this.Row - 1].Text.Length, this.PositionInRow);
                if (delta > 0 && drawnText.Rows[this.Row - 1].Text[delta - 1] == '\n')
                    delta--;
                if (delta == 0)
                    stayAtEndOfRow = CursorPositions.BeginRow;
                else
                    stayAtEndOfRow = CursorPositions.EndRow;
                drawnText.CursorPosition = this.prevRowOffset + delta;
            }
            else
            {
                if (this.Row + 1 == drawnText.Rows.Length)
                    return;
                int delta = Math.Min(drawnText.Rows[this.Row + 1].Text.Length, this.PositionInRow);
                if (delta > 0 && drawnText.Rows[this.Row + 1].Text[delta - 1] == '\n')
                    delta--;
                if (delta == 0)
                    stayAtEndOfRow = CursorPositions.BeginRow;
                else
                    stayAtEndOfRow = CursorPositions.EndRow;
                drawnText.CursorPosition = this.nextRowOffset + delta;
            }
        }

        /// <summary>
        /// Calculates all other properties using CursorPosition and Text properties.
        /// </summary>
        internal void Calculate()
        {
            nextRowOffset = 0;
            currentRowOffset = 0;
            prevRowOffset = 0;
            PositionInRow = 0;
            Row = -1;
            for (int i = 0; i < drawnText.Rows.Length; i++)
            {
                AddRow(1);

                if (drawnText.CursorPosition < nextRowOffset)
                {
                    PositionInRow = drawnText.CursorPosition - currentRowOffset;
                    if (PositionInRow == 0 && Row > 0 && stayAtEndOfRow == CursorPositions.EndRow)
                    {
                        AddRow(-1);
                        PositionInRow = drawnText.Rows[Row].Text.Length;

                    }
                    break;
                }
                if (nextRowOffset == drawnText.CursorPosition)
                {
                    if (stayAtEndOfRow == CursorPositions.BeginRow)
                    {
                        AddRow(1);
                        PositionInRow = 0;
                        break;
                    }
                    if (stayAtEndOfRow == CursorPositions.BasicPosition && drawnText.Rows[Row].Text.EndsWith("\n"))
                    {
                        AddRow(1);
                        PositionInRow = 0;
                        break;
                    }
                    PositionInRow = drawnText.CursorPosition - currentRowOffset;
                    break;
                }
            }
            string text = drawnText.Rows[this.Row].Text.Substring(0, this.PositionInRow);
            float textLen = drawnText.Font.MeasureString(text).X;
            cursor.Set_X(drawnText.Rows[this.Row].OffsetX + (int)textLen);
        }

        private void AddRow(int delta)
        {
            Row += delta;
            prevRowOffset = currentRowOffset;
            currentRowOffset = nextRowOffset;
            if (Row < drawnText.Rows.Length)
                nextRowOffset += drawnText.Rows[Row].Text.Length;
        }

        internal void ResizeCursor()
        {
            if (cursor.Rectangle.Height != drawnText.Font.LineSpacing)
                this.cursor = new Tect(TextureLoader.Instance.CreateSimpleTexture(Color.Black), new Rectangle(0, 0, (int)Math.Max(drawnText.Font.Spacing, 2), drawnText.Font.LineSpacing));
        }

        internal void Draw(SpriteBatch sb, Point offset, int showFromIndex, int offsetY)
        {
            cursor.Set_Y(offsetY + drawnText.Font.LineSpacing * (this.Row - showFromIndex));
            cursor.DrawDynamic(sb, offset);
        }
    }
}
