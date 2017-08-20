using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextMenu_Mono.Menu
{
    public sealed class NumericInputMenuPanel : TextInputMenuPanel
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Step { get; set; }

        public NumericInputMenuPanel(MenuPanelSettings settings, double minValue, double maxValue, double step) : base(settings)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Step = step;
        }

        internal override void ExecuteAction()
        {
            if (Settings.IgnoreEffects == false)
            {
                this.DrawnText.HasCursor = true;
                editor = new NumericInputEditor(this, Settings.Font, MinValue, MaxValue, Step);
            }
        }

        public double GetValue()
        {
            double number;
            if (double.TryParse(Text, out number))
            {
                if (number < MinValue)
                    return MinValue;
                else if (number > MaxValue)
                    return MaxValue;
                return number;
            }
            return MinValue;
        }
    }
}
