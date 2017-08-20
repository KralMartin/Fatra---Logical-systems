using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ContextMenu_Mono.Menu
{
    class NumericInputEditor : TextInputEditor
    {
        double minValue, maxValue, step;

        public NumericInputEditor(TextInputMenuPanel panel, SpriteFont font, double minValue, double maxValue, double step) : base(panel, font, false)
        {
            this.DebugValue = "NUMERIC INPUT";
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }

        protected override bool ValidateAddedChar(char character)
        {
            int decimalKeyRepresentation = (int)character;
            if (decimalKeyRepresentation >= 48 && decimalKeyRepresentation <= 57)
                return true;
            return false;
        }

        public override bool MouseWheel(int delta)
        {
            double number;
            if (double.TryParse(panel.Text, out number))
            {
                if (delta > 0)
                    number += step;
                else
                    number -= step;
                if (number < minValue)
                    number = minValue;
                else if (number > maxValue)
                    number = maxValue;
            }
            else
                number = minValue;
            panel.Text = number.ToString();
            panel.DrawnText.CursorPosition = panel.DrawnText.Text.Length;
            panel.TextChanged();
            return true;
        }
    }
}
