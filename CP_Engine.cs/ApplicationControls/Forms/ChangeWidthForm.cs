using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class ChangeWidthForm
    {
        NumericInputMenuPanel input;
        WorkPlace workplace;

        internal ChangeWidthForm(WorkPlace workplace)
        {
            this.workplace = workplace;

            MenuPanelSettings s = new MenuPanelSettings();
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Size = new Point(4, 4);
            s.AdjustHeightToContent = true;
            s.AdjustWidthToContent = true;
            s.IgnoreEffects = true;
            MenuPanel content = new MenuPanel(s);

            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();
            MenuPanelSettings inputSettings = DefaultUI.DefaultNumericInput();

            //Title
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Width:";
            content.Children.Add(lbl);

            input = new NumericInputMenuPanel(inputSettings, 1, 16, 1);
            input.Text = workplace.WidthAssistant.GetSelectionWidth() + "";
            content.Children.Add(input);

            content.Changed();
            Form form = DefaultUI.CreateDefaultForm("Change width", content);
            form.AfterClose += Form_Closed;
            input.SimulateClick();
        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result)
            {
                int value = (int)input.GetValue();
                workplace.WidthAssistant.SetWidth(value);
            }
        }
    }
}
