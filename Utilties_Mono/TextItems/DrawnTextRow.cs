using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilties_Mono
{
    public class DrawnTextRow
    {
        public string Text { get; private set; }
        public float Width { get; private set; }
        public int OffsetX { get; set; }

        internal DrawnTextRow(string text, float width)
        {
            this.Text = text;
            this.Width = width;
        }
    }
}
