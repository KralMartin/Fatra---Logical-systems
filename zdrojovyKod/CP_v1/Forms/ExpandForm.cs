using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_v1
{
    class ExpandForm
    {
        Engine engine;
        NumericInputMenuPanel input;
        WorkPlace workplace;
        int side;

        internal ExpandForm(WorkPlace workplace, int side)
        {
            this.workplace = workplace;
            this.side = side;

            MenuPanelSettings s = new MenuPanelSettings();
            s.ChildrenLayout = ChildrenLayouts.Normal;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Size = new Point(300, 80);
            s.IgnoreEffects = true;
            MenuPanel content = new MenuPanel(s);

            MenuPanelSettings inputSettings = DefaultUI.DefaultNumericInput();
            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();

            int axisY = -30;
            //========================== Title
            //Label
            axisY += 40;
            lblSettings.Margin = new Point(10, axisY);
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Expand by:";
            content.Children.Add(lbl);
            //Input
            axisY += 25;
            inputSettings.Margin = new Point(10, axisY);
            input = new NumericInputMenuPanel(inputSettings, 1, 100, 5);
            content.Children.Add(input);
            //==========================

            Form form = DefaultUI.CreateDefaultForm("expand side " + side, content);
            form.AfterClose += Form_Closed;
        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result == false)
                return;
            workplace.CurrentWindow.ExpandScheme(side, (int)this.input.GetValue());
        }
    }
}
