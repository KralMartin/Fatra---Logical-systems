using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class InsertBitsForm
    {
        TextInputMenuPanel input;
        WorkPlace workplace;
        PhysScheme pScheme;

        internal InsertBitsForm(WorkPlace workplace, PhysScheme pScheme)
        {
            this.workplace = workplace;
            this.pScheme = pScheme;

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
            lbl.Text = "Value:";
            content.Children.Add(lbl);

            input = new TextInputMenuPanel(inputSettings);
            input.Text = workplace.WidthAssistant.GetSelectionWidth() + "";
            content.Children.Add(input);

            content.Changed();
            Form form = DefaultUI.CreateDefaultForm("Insert bits", content);
            form.AfterClose += Form_Closed;
            input.SimulateClick();
        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result)
            {
                Queue<bool> bits = GetBits();
                workplace.Simulation.StopThread();
                pScheme.PlacedBug.Bug.SetMemmoryValue(bits, pScheme, workplace.Simulation);
                workplace.Simulation.StartThread();
            }
        }

        private Queue<bool> GetBits()
        {
            Queue<bool> toReturn = new Queue<bool>();
            for (int i = input.Text.Length - 1; i >=0; i--)
            {
                if (input.Text[i] == '1')
                    toReturn.Enqueue(true);
                else if (input.Text[i] == '0')
                    toReturn.Enqueue(false);
                else
                    return null;
            }
            return toReturn;
        }

    }
}
