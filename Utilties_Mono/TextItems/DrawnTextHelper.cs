using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilties_Mono
{
    class DrawnTextHelper
    {
        private float rowWidth, maxRowWidth;
        private StringBuilder sbRow, sbWord;
        private List<DrawnTextRow> rowList;
        private SpriteFont font;

        internal static List<DrawnTextRow> GetRows(string text, int maxRowWidth, SpriteFont font)
        {
            DrawnTextHelper helper = new DrawnTextHelper();
            return helper.GetRows_Private(text, maxRowWidth, font);
        }

        private DrawnTextHelper() { }

        private List<DrawnTextRow> GetRows_Private(string text, int maxRowWidth, SpriteFont font)
        {
            if (text == null)
                return new List<DrawnTextRow>();
            this.font = font;
            this.maxRowWidth = maxRowWidth;
            rowList = new List<DrawnTextRow>();
            sbRow = new StringBuilder();
            sbWord = new StringBuilder();
            rowWidth = 0;

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case ' ':
                        AppendWord();
                        sbWord.Append(' ');
                        AppendWord();
                        break;
                    case '\n':
                        AppendWord();
                        NewLine();
                        break;
                    default:
                        sbWord.Append(text[i]);
                        break;
                }
            }
            AppendWord();
            DrawnTextRow row = new DrawnTextRow(sbRow.ToString(), rowWidth);
            rowList.Add(row);
            return rowList;
        }

        private void NewLine()
        {
            sbRow.Append('\n');
            DrawnTextRow row = new DrawnTextRow(sbRow.ToString(), rowWidth);
            rowList.Add(row);

            sbRow.Clear();
            rowWidth = 0;
        }

        private void AppendWord()
        {
            string s = sbWord.ToString();
            float wordWidth = font.MeasureString(sbWord.ToString()).X;
            if (rowWidth <= 0 || wordWidth + rowWidth <= maxRowWidth)
            {
                sbRow.Append(sbWord.ToString());
                rowWidth += wordWidth;
            }
            else
            {
                DrawnTextRow row = new DrawnTextRow(sbRow.ToString(), rowWidth);
                rowList.Add(row);
                rowWidth = wordWidth;
                sbRow.Clear();
                sbRow.Append(sbWord.ToString());
            }
            sbWord.Clear();
        }

    }
}
